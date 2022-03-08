using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using Command;
using System.IO;


[System.Serializable]
public class VsnScriptReader {

  public string loadedScriptName;
  public List<VsnCommand> vsnCommands;
  public List<VsnWaypoint> waypoints;
  public VsnArgument[] args;
  public int currentCommandIndex;

  public string[] readableLines;

  public VsnScriptReader(){
    currentCommandIndex = 0;
    loadedScriptName = "custom";
  }


  public void LoadScriptContent(string content, string scriptName, VsnArgument[] newArgs) {
    //AnalyzeString(content);

    loadedScriptName = scriptName;
    string[] lines = content.Split('\n');

    readableLines = lines;

    ResetWaypoints();
    vsnCommands = ParseVSNCommands(lines);
    args = newArgs;
  }

  public void AnalyzeString(string text) {
    for(int i=0; i<text.Length; i++) {
      if(text[i] == '\r') {
        Debug.LogError("i: " + i+ ", char[i]: \\r");
      }
      if(text[i] == '\n') {
        Debug.LogError("i: " + i + ", char[i]: \\n");
      }
    }
  }

  public void SetArgs(){
    if(args != null) {
      //Debug.LogWarning("Executing set args");
      VsnSaveSystem.SetVariable("argsCount", args.Length);
      for (int i = 0; i < args.Length; i++) {
        if(args[i].GetType() == typeof(VsnNumber)) {
          VsnSaveSystem.SetVariable("arg" + (i + 1), args[i].GetNumberValue());
          Debug.Log("[ARG" + (i + 1) + "] set to: " + args[i].GetNumberValue());
        } else if(args[i].GetType() == typeof(VsnString)) {
          VsnSaveSystem.SetVariable("arg" + (i + 1), args[i].GetStringValue());
          Debug.Log("[ARG" + (i+1) + "] set to: " + args[i].GetStringValue());
        } else {
          VsnSaveSystem.SetVariable("arg" + (i + 1), args[i].GetReference());
          Debug.Log("[ARG" + (i + 1) + "] set to: " + args[i].GetReference());
        }
      }
    } else {
      VsnSaveSystem.SetVariable("argsCount", 0);
    }
  }


  public void ExecuteCurrentCommand(){
    VsnCommand currentCommand = vsnCommands[currentCommandIndex];
    Debug.Log(">> " + loadedScriptName + " line " + currentCommand.fileLineId);
    currentCommandIndex++;
    currentCommand.Execute();
  }

  public bool HasFinished(){
    //Debug.Log("currentCommandIndex: "+ currentCommandIndex+ ", vsnCommands.Count: "+ vsnCommands.Count);
    return currentCommandIndex >= vsnCommands.Count;
  }


  public void GotoWaypoint(string waypointName){
    VsnWaypoint waypoint = GetWaypointFromLabel(waypointName);

    if(waypoint != null) {
      currentCommandIndex = waypoint.commandNumber;
    } else {
      Debug.Log("Invalid waypoint with label: " + waypointName);
    }
  }

  public void GotoCommandId(int commandId) {
    currentCommandIndex = commandId;
  }


  public void GotoNextElseOrEndif(){
    int commandIndex = FindNextElseOrEndifCommand();

    if(commandIndex == -1) {
      Debug.LogError("Invalid if/else/endif structure. Please check the command number " + commandIndex);
      currentCommandIndex = 999999999;
    } else {
      currentCommandIndex = commandIndex+1;
    }
  }

  public int FindNextElseOrEndifCommand() {
    int index = currentCommandIndex;
    int nestedIfCommandsFound = 0;

    for(int i = index; i < vsnCommands.Count; i++) {
      VsnCommand command = vsnCommands[i];

      if(command.GetType() == typeof(IfCommand)){
        nestedIfCommandsFound++;
      }

      if(command.GetType() == typeof(EndIfCommand)){
        if(nestedIfCommandsFound == 0) {
          return command.commandIndex;
        } else {
          nestedIfCommandsFound -= 1;
        }
      }

      if(command.GetType() == typeof(ElseCommand) &&
         nestedIfCommandsFound == 0){
        return command.commandIndex;
      }
    }
    return -1;
  }


  public void GotoNextEndwhile() {
    int commandIndex = FindNextEndwhileCommand();

    if(commandIndex == -1) {
      Debug.LogError("Invalid while/endwhile structure. Please check the command number " + commandIndex);
      currentCommandIndex = 999999999;
    } else {
      currentCommandIndex = commandIndex + 1;
    }
  }

  public int FindNextEndwhileCommand() {
    int index = currentCommandIndex;
    int nestedWhileCommandsFound = 0;

    for(int i = index; i < vsnCommands.Count; i++) {
      VsnCommand command = vsnCommands[i];

      if(command.GetType() == typeof(WhileCommand)) {
        nestedWhileCommandsFound++;
      }

      if(command.GetType() == typeof(EndWhileCommand)) {
        if(nestedWhileCommandsFound == 0) {
          return command.commandIndex;
        } else {
          nestedWhileCommandsFound -= 1;
        }
      }
    }
    return -1;
  }

  public void GotoPreviousWhile() {
    int commandIndex = FindPreviousWhileCommand();

    if(commandIndex == -1) {
      Debug.LogError("Invalid while/endwhile structure. Please check the command number " + commandIndex);
      currentCommandIndex = 999999999;
    } else {
      currentCommandIndex = commandIndex;
    }
  }

  public int FindPreviousWhileCommand() {
    int index = currentCommandIndex-2;
    int nestedWhileCommandsFound = 0;

    for(int i = index; i >= 0; i--) {
      VsnCommand command = vsnCommands[i];

      if(command.GetType() == typeof(EndWhileCommand)) {
        nestedWhileCommandsFound++;
      }

      if(command.GetType() == typeof(WhileCommand)) {
        if(nestedWhileCommandsFound == 0) {
          return command.commandIndex;
        } else {
          nestedWhileCommandsFound -= 1;
        }
      }
    }
    return -1;
  }



  public void GotoNextEndfor() {
    int commandIndex = FindNextEndforCommand();

    if(commandIndex == -1) {
      Debug.LogError("Invalid for/endfor structure. Please check the command number " + commandIndex);
      currentCommandIndex = 999999999;
    } else {
      currentCommandIndex = commandIndex + 1;
    }
  }

  public int FindNextEndforCommand() {
    int index = currentCommandIndex;
    int nestedForCommandsFound = 0;

    for(int i = index; i < vsnCommands.Count; i++) {
      VsnCommand command = vsnCommands[i];

      if(command.GetType() == typeof(ForCommand)) {
        nestedForCommandsFound++;
      }

      if(command.GetType() == typeof(EndForCommand)) {
        if(nestedForCommandsFound == 0) {
          return command.commandIndex;
        } else {
          nestedForCommandsFound -= 1;
        }
      }
    }
    return -1;
  }

  public VsnCommand ReturnPreviousForCommand() {
    int index = currentCommandIndex - 2;
    int nestedForCommandsFound = 0;

    for(int i = index; i >= 0; i--) {
      VsnCommand command = vsnCommands[i];

      if(command.GetType() == typeof(EndForCommand)) {
        nestedForCommandsFound++;
      }

      if(command.GetType() == typeof(ForCommand)) {
        if(nestedForCommandsFound == 0) {
          return command;
        } else {
          nestedForCommandsFound -= 1;
        }
      }
    }
    return null;
  }


  public VsnCommand FindNextEndwhileOrEndforCommand() {
    int index = currentCommandIndex;

    for(int i = index; i < vsnCommands.Count; i++) {
      VsnCommand command = vsnCommands[i];

      if(command.GetType() == typeof(EndForCommand)) {
        return command;
      }
      if(command.GetType() == typeof(EndWhileCommand)) {
        return command;
      }
    }
    return null;
  }

  public void GotoEnd(){
    currentCommandIndex = vsnCommands.Count;
  }


  public void ResetWaypoints() {
    waypoints = new List<VsnWaypoint>();
  }

  public void RegisterWaypoint(VsnWaypoint vsnWaypoint) {   
    if(waypoints.Contains(vsnWaypoint) == false) {
      waypoints.Add(vsnWaypoint);
    }
  }

  public VsnWaypoint GetWaypointFromLabel(string label) {
    foreach(VsnWaypoint waypoint in waypoints) {
      if(waypoint.label == label) {
        return waypoint;
      }
    }

    return null;
  }

  public List<VsnCommand> ParseVSNCommands(string[] lines) {
    List<VsnCommand> vsnCommandsFromScript = new List<VsnCommand>();

    int commandNumber = 0;
    for(int i=0; i< lines.Length; i++) {
      string line = lines[i].TrimStart();

      if(line == "\r" || String.IsNullOrEmpty(line)) {
        continue;
      }

      List<VsnArgument> vsnArguments = new List<VsnArgument>();

      string commandName = Regex.Match(line, "^([\\w\\-]+)").Value;

      MatchCollection valuesMatch = Regex.Matches(line, "[^\\s\"']+|\"[^\"]*\"|'[^']*'");

      List<string> args = new List<string>();

      foreach(Match match in valuesMatch) {
        args.Add(match.Value);
      }

      args.RemoveAt(0); // Removes the first match, which is the "commandName"

      foreach(string arg in args) {
        VsnArgument vsnArgument = ParseArgument(arg);
        vsnArguments.Add(vsnArgument);
      }

      VsnCommand vsnCommand = InstantiateVsnCommand(commandName, vsnArguments);
      if(vsnCommand != null) {
        if(commandName == "waypoint") {
          RegisterWaypoint(new VsnWaypoint(vsnArguments[0].GetReference(), commandNumber));
        }

        vsnCommand.commandIndex = commandNumber;
        vsnCommand.fileLineId = i+1;
        commandNumber++;
        vsnCommandsFromScript.Add(vsnCommand);
      }
    }

    return vsnCommandsFromScript;
  }


  /// <summary>
  /// Iterates through all command classes searching for one with the correct CommandAttribute matching the commandName
  /// </summary>
  /// <returns>The vsn command.</returns>
  /// <param name="commandName">Command name.</param>
  /// <param name="vsnArguments">Vsn arguments.</param>
  public static VsnCommand InstantiateVsnCommand(string commandName, List<VsnArgument> vsnArguments) {
    foreach(Type type in VsnController.instance.possibleCommandTypes) {

      foreach(Attribute attribute in type.GetCustomAttributes(false)) {
        if(attribute is CommandAttribute) {
          CommandAttribute commandAttribute = (CommandAttribute)attribute;

          if(commandAttribute.CommandString == commandName) {
            VsnCommand vsnCommand = Activator.CreateInstance(type) as VsnCommand;
            vsnCommand.InjectArguments(vsnArguments);

            if(vsnCommand.CheckSyntax() == false){
              string line = commandName+": ";

              foreach(VsnArgument c in vsnArguments){
                line += c.GetPrintableValue()+", ";
              }
              Debug.LogError("Invalid syntax for this command: " + line);
              return new InvalidCommand();
            }

            // TODO add metadata like line number?...

            return vsnCommand;
          }
        }
      }
    }

    //Debug.Log("Got a null");
    return null;
  }

  /// <summary>
  /// Parses a string into one of three arguments: a string, a number (float) or a reference to a variable
  /// </summary>
  /// <returns>The argument.</returns>
  public static VsnArgument ParseArgument(string arg) {

    if(arg.StartsWith("\"") && arg.EndsWith("\"")) {
      return new VsnString(arg.Substring(1, arg.Length - 2));
    }

    if(StringIsNumber(arg)) {
      float value = float.Parse(arg, CultureInfo.InvariantCulture.NumberFormat);
      return new VsnNumber(value);
    }

    if(StringIsOperator(arg)) {
      return new VsnOperator(arg);
    }

    if(arg == "true" || arg == "false") {
      return new VsnBoolean(arg == "true");
    }

    if (arg[0] == '*') {
      return new VsnMetaReference(arg.Substring(1, arg.Length-1));
    }

    return new VsnReference(arg);
  }

  /// <summary>
  /// Returns true if string is made of digits only (0~9) and a point ('.') for float values.
  /// </summary>
  /// <returns><c>true</c>, if is string is only digits, <c>false</c> otherwise.</returns>
  /// <param name="str">String.</param>
  private static bool StringIsNumber(string str) {
    float exit;

    if( float.TryParse(str, out exit) ){
      return true;
    }
    return false;
  }

  private static bool StringIsOperator(string str){
    switch(str){
      case "+":
      case "-":
      case "/":
      case "*":
      case "==":
      case "!=":
      case ">=":
      case "<=":
      case ">":
      case "<":
        return true;
    }
    return false;
  }
}
