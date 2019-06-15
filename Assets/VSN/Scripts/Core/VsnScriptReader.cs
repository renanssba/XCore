using System.Collections;
using System.Collections.Generic;
using System;
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

  public VsnScriptReader(){
    currentCommandIndex = 0;
    loadedScriptName = "custom";
  }


  public void LoadScriptContent(string content, string scriptName, VsnArgument[] newArgs) {
    loadedScriptName = scriptName;
    string[] lines = content.Split('\n');

    ResetWaypoints();
    vsnCommands = ParseVSNCommands(lines);
    args = newArgs;
  }

  public void SetArgs(){
    if(args != null) {
      //Debug.LogWarning("Executing set args");
      VsnSaveSystem.SetVariable("argsCount", args.Length);
      for (int i = 0; i < args.Length; i++) {
        if(args[i].GetType() == typeof(VsnNumber)) {
          VsnSaveSystem.SetVariable("arg" + (i + 1), args[i].GetNumberValue());
          Debug.Log("Setting variable arg" + (i+1) + " to value: " + args[i].GetNumberValue());
        } else if(args[i].GetType() == typeof(VsnString)) {
          VsnSaveSystem.SetVariable("arg" + (i + 1), args[i].GetStringValue());
          Debug.Log("Setting variable arg" + (i+1) + " to value: " + args[i].GetStringValue());
        } else {
          VsnSaveSystem.SetVariable("arg" + (i + 1), args[i].GetReference());
          Debug.Log("Setting variable arg" + (i + 1) + " to value: " + args[i].GetReference());
        }
      }
    }
  }


  public void ExecuteCurrentCommand(){
    VsnCommand currentCommand = vsnCommands[currentCommandIndex];
    Debug.Log(">> " + loadedScriptName + " line " + currentCommand.fileLineId);
    currentCommandIndex++;
    currentCommand.Execute();
  }

  public bool HasFinished(){
    Debug.Log("currentCommandIndex: "+ currentCommandIndex+ ", vsnCommands.Count: "+ vsnCommands.Count);
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


  public static string ConvertVSNCommands(string vsnPath, string[] lines) {
    Debug.LogWarning("Calling ConvertVSNCommands. path: " + vsnPath);

    string newFilePath = GetConvertedPath(vsnPath);

    int lastBar = vsnPath.LastIndexOf('/');
    string filename = vsnPath.Substring(lastBar + 1, vsnPath.Length - (lastBar + 1));

    string metadataPath = vsnPath.Substring(12, vsnPath.Length-12);

    string content = "";
    string metaContent = "";
    string metaContentNames = "";

    Debug.LogWarning("new path: " + newFilePath);

    int sayTexts = 0;
    List<string> charNamesList = new List<string>();

    for(int i = 0; i < lines.Length; i++) {
      string line = lines[i].TrimStart();

      if(line == "\r" || String.IsNullOrEmpty(line)) {
        content += lines[i];
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
        if(commandName == "say" || commandName == "say_auto") {
          string textKey = metadataPath + "/say_" + sayTexts;

          content += lines[i].Replace(lines[i].TrimStart(), "");
          content += commandName;

          if(vsnArguments.Count > 1) {
            string charNameKey;
            int charNamePosInList = GetCharName(charNamesList, args[0].Substring(1, args[0].Length-2) );
            if(charNamePosInList != -1) {
              charNameKey = metadataPath + "/char_name_" + charNamePosInList;
            } else {
              charNameKey = metadataPath + "/char_name_" + charNamesList.Count;
              charNamesList.Add( args[0].Substring(1, args[0].Length - 2) );
              metaContentNames += charNameKey + ", \"" + args[0].Substring(1, args[0].Length - 2) + "\"\n";
            }

            content += " \"" + charNameKey+"\" \""+ textKey + "\"";
            metaContent += textKey + ", \"" + args[1].Substring(1, args[1].Length - 2) + "\"";

          } else {
            content += " \"" + textKey + "\"";
            metaContent += textKey + ", \"" + args[0].Substring(1, args[0].Length - 2) + "\"";
          }
          content += "\n";
          metaContent += "\n";

          sayTexts++;
        } else {
          content += lines[i];
        }
      }
    }

    // save new, converted file
    if(File.Exists(newFilePath)) {
      File.Delete(newFilePath);
    }
    File.Create(newFilePath).Close();
    File.WriteAllText(newFilePath, content);


    return metaContentNames+metaContent;
  }

  public static string GetConvertedPath(string path) {
    string newPath = "";
    string filename = "";
    int lastBar = path.LastIndexOf('/');
    Debug.Log("path: "+path+", lastbar: " + lastBar);
    newPath = path.Substring(0, lastBar);

    //Debug.LogWarning("new path first: " + newPath);
    filename = path.Substring(lastBar+1, path.Length - (lastBar + 1));
    //Debug.LogWarning("filename: " + filename);
    newPath += "/converted/" + filename+".txt";

    return "Assets/Resources/" + newPath;
  }

  public static int GetCharName(List<string> charNamesList, string name) {
    for(int i=0; i<charNamesList.Count; i++) {
      if(charNamesList[i] == name) {
        return i;
      }
    }
    return -1;
  }


  /// <summary>
  /// Iterates through all command classes searching for one with the correct CommandAttribute matching the commandName
  /// </summary>
  /// <returns>The vsn command.</returns>
  /// <param name="commandName">Command name.</param>
  /// <param name="vsnArguments">Vsn arguments.</param>
  private static VsnCommand InstantiateVsnCommand(string commandName, List<VsnArgument> vsnArguments) {
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
                line += c.GetStringValue() + "/" + c.GetReference() + "/" + c.GetNumberValue()+" - ";
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
  private static VsnArgument ParseArgument(string arg) {

    if(arg.StartsWith("\"") && arg.EndsWith("\"")) {
      return new VsnString(arg.Substring(1, arg.Length - 2));
    }

    if(StringIsNumber(arg)) {
      float value = float.Parse(arg);
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



  public static void PrepareFilesForLocalization(string generalPath) {
    string[] filePaths = Directory.GetFiles(generalPath);
    string metadataFilePath = generalPath+"/converted/strings_table.csv";
    string metaContent = "";

    Debug.LogWarning("GENERAL PATH: " + generalPath);
    Debug.LogWarning("METADATA PATH: " + metadataFilePath);

    foreach(string filePath in filePaths) {
      if(filePath.EndsWith(".meta")) {
        continue;
      }

      int start = filePath.LastIndexOf("/VSN Scripts") +1;
      string filename = filePath.Substring(start, filePath.Length-start-4);

      filename = filename.Replace("\\", "/");

      Debug.Log("filenames: " + filename);

      TextAsset textContent = Resources.Load<TextAsset>(filename);
      if(textContent == null) {
        Debug.LogWarning("Error loading VSN Script: " + filePath + ". Please verify the provided path.");
      }
      string[] lines = textContent.text.Split('\n');
      metaContent += ConvertVSNCommands(filename, lines);
    }


    // save metadata file
    if(File.Exists(metadataFilePath)) {
      File.Delete(metadataFilePath);
    }
    File.Create(metadataFilePath).Close();
    File.WriteAllText(metadataFilePath, metaContent);
  }
}
