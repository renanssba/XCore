using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

public class VsnLocalizationConverter : MonoBehaviour {

  Dictionary<string, string> commonCharNames;
  Dictionary<string, string> commonChoiceTexts;

  public void PrepareFilesForLocalization() {
    string metadataFilePath = Application.dataPath + "/Resources/VSN Scripts/converted/strings_table.csv";
    string metaContent = "";

    commonCharNames = new Dictionary<string, string> {
      { "Fertiliel", "char_name/fertiliel" },
      { "\\boy", "char_name/boy" },
      { "\\girl", "char_name/girl" },
      { "", "char_name/none" }
    };

    commonChoiceTexts = new Dictionary<string, string> {
      { "Sim", "choices/yes" },
      { "Não", "choices/no" },
      { "Cancelar", "choices/cancel" }
    };

    metaContent += PrepareFilesForLocalization(Application.dataPath + "/Resources/VSN Scripts");
    metaContent += PrepareFilesForLocalization(Application.dataPath + "/Resources/VSN Scripts/date");
    metaContent += PrepareFilesForLocalization(Application.dataPath + "/Resources/VSN Scripts/observation");

    Debug.LogWarning("METADATA PATH: " + metadataFilePath);

    // save metadata file
    if(File.Exists(metadataFilePath)) {
      File.Delete(metadataFilePath);
    }
    File.Create(metadataFilePath).Close();
    File.WriteAllText(metadataFilePath, metaContent);
  }


  public string PrepareFilesForLocalization(string generalPath) {
    string[] filePaths = Directory.GetFiles(generalPath);
    string metaContent = "";

    Debug.LogWarning("GENERAL PATH: " + generalPath);

    foreach(string filePath in filePaths) {
      if(filePath.EndsWith(".meta")) {
        continue;
      }

      int start = filePath.LastIndexOf("/VSN Scripts") + 1;
      string filename = filePath.Substring(start, filePath.Length - start - 4);

      filename = filename.Replace("\\", "/");

      Debug.Log("filenames: " + filename);

      TextAsset textContent = Resources.Load<TextAsset>(filename);
      if(textContent == null) {
        Debug.LogWarning("Error loading VSN Script: " + filePath + ". Please verify the provided path.");
      }
      string[] lines = textContent.text.Split('\n');
      metaContent += ConvertVSNCommands(filename, lines);
    }
    return metaContent;
  }



  public string ConvertVSNCommands(string vsnPath, string[] lines) {
    Debug.LogWarning("Calling ConvertVSNCommands. path: " + vsnPath);

    string newFilePath = GetConvertedPath(vsnPath);

    int lastBar = vsnPath.LastIndexOf('/');
    string filename = vsnPath.Substring(lastBar + 1, vsnPath.Length - (lastBar + 1));

    string metadataPath = vsnPath.Substring(12, vsnPath.Length - 12);
    string content = "";
    string metaContent = "";
    string metaContentNames = "";

    Debug.LogWarning("new path: " + newFilePath);

    int sayCommands = 0;
    int choiceCommands = 0;
    List<string> charNamesList = new List<string>();

    for(int i = 0; i < lines.Length; i++) {
      string line = lines[i].TrimStart();

      if(line == "\r" || String.IsNullOrEmpty(line)) {
        content += lines[i].TrimEnd() + Environment.NewLine;
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
        VsnArgument vsnArgument = VsnScriptReader.ParseArgument(arg);
        vsnArguments.Add(vsnArgument);
      }

      VsnCommand vsnCommand = VsnScriptReader.InstantiateVsnCommand(commandName, vsnArguments);
      if(vsnCommand != null) {
        if(commandName == "say" || commandName == "say_auto") {
          string textKey = metadataPath + "/say_" + sayCommands;

          content += lines[i].Replace(lines[i].TrimStart(), "");
          content += commandName;

          if(vsnArguments.Count > 1) {
            string charNameKey;
            string charNameKeySaved = GetCharName(charNamesList, args[0].Substring(1, args[0].Length - 2), metadataPath);
            if(charNameKeySaved != null) {
              charNameKey = charNameKeySaved;
            } else {
              charNameKey = metadataPath + "/char_name_" + charNamesList.Count;
              charNamesList.Add(args[0].Substring(1, args[0].Length - 2));
              metaContentNames += charNameKey + ", \"" + args[0].Substring(1, args[0].Length - 2) + "\"" + Environment.NewLine;
            }

            content += " \"" + charNameKey + "\" \"" + textKey + "\"";
            metaContent += textKey + ", \"" + args[1].Substring(1, args[1].Length - 2) + "\"";

          } else {
            content += " \"" + textKey + "\"";
            metaContent += textKey + ", \"" + args[0].Substring(1, args[0].Length - 2) + "\"";
          }
          content += Environment.NewLine;
          metaContent += Environment.NewLine;

          sayCommands++;
        } else if(commandName == "choices") {
          Debug.LogWarning("TODO: IMPLEMENT THIS");
          string textKey = metadataPath + "/choices_" + choiceCommands;

          content += lines[i].Replace(lines[i].TrimStart(), "");
          content += commandName;

          for(int j=0; j<args.Count; j+=2) {

            string commonChoiceKey = GetCommonChoice(args[j].Substring(1, args[j].Length - 2));
            if(commonChoiceKey != null) {
              content += " \"" + commonChoiceKey + "\" " + args[j + 1];
            } else {
              content += " \"" + textKey + "_" + (j / 2) + "\" " + args[j + 1];
              metaContent += textKey + "_" + (j / 2) + ", \"" + args[j].Substring(1, args[j].Length - 2) + "\"" + Environment.NewLine;
            }
          }
          content += Environment.NewLine;

          choiceCommands++;
          //content += lines[i];
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


    return metaContentNames + metaContent;
  }


  public string GetConvertedPath(string path) {
    string newPath = "";
    string filename = "";
    int lastBar = path.LastIndexOf('/');
    Debug.Log("path: " + path + ", lastbar: " + lastBar);
    newPath = path.Substring(0, lastBar);

    //Debug.LogWarning("new path first: " + newPath);
    filename = path.Substring(lastBar + 1, path.Length - (lastBar + 1));
    //Debug.LogWarning("filename: " + filename);
    newPath += "/converted/" + filename + ".txt";

    return "Assets/Resources/" + newPath;
  }

  public string GetCharName(List<string> charNamesList, string name, string metadataPath) {
    if(commonCharNames.ContainsKey(name)) {
      return commonCharNames[name];
    }

    for(int i = 0; i < charNamesList.Count; i++) {
      if(charNamesList[i] == name) {
        return metadataPath + "/char_name_" + i;
      }
    }
    return null;
  }

  public string GetCommonChoice(string text) {
    if(commonChoiceTexts.ContainsKey(text)) {
      return commonChoiceTexts[text];
    }
    return null;
  }
}
