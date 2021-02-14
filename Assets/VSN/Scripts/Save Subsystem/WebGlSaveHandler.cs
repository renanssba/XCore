using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class WebGlSaveHandler : IVsnSaveHandler {

  private static readonly string savePrefix = "file_";


  public static string GetSaveFileName(int saveSlot) {
    return savePrefix + saveSlot + ".sav";
  }

  void IVsnSaveHandler.Save(Dictionary<string, string> dictionary, int saveSlot, Action<bool> callback) {
    bool success;

    Dictionary<string, string> savedDictionary = dictionary;
    string savedVariables = GenerateSavedVariables(savedDictionary);
    //Debug.Log("Setting to playerprefs, string: " + GetSaveSlotPrefix(saveSlot) + ", value: " + savedVariables);
    //File.WriteAllText(GetFullSaveFilePath(saveSlot), savedVariables);
    PlayerPrefs.SetString(GetSaveFileName(saveSlot), savedVariables);

    success = true;
    callback(success);
  }

  void IVsnSaveHandler.Load(Dictionary<string, string> dictionary, int saveSlot, Action<Dictionary<string, string>> callback) {
    Dictionary<string, string> loadedDictionary = LoadSavedVariables(saveSlot);
    callback(loadedDictionary);
  }

  bool IVsnSaveHandler.IsSaveSlotBusy(int saveSlot) {
    //return File.Exists(GetFullSaveFilePath(saveSlot));
    return PlayerPrefs.HasKey(GetSaveFileName(saveSlot));
  }

  Dictionary<string, string> IVsnSaveHandler.GetSavedDictionary(int saveSlot) {
    return LoadSavedVariables(saveSlot);
  }

  /// <summary>
  /// Generates the saved variables in format: "varname=value@varname=value@varname=value@(...)"
  /// </summary>
  /// <returns>The saved variables.</returns>
  /// <param name="dictionary">Dictionary.</param>
  string GenerateSavedVariables(Dictionary<string, string> dictionary) {
    string savedVariables = "";

    foreach(KeyValuePair<string, string> entry in dictionary) {
      savedVariables += entry.Key + "=" + entry.Value;
      savedVariables += "@";
    }

    return savedVariables;
  }

  Dictionary<string, string> LoadSavedVariables(int saveSlot) {
    Dictionary<string, string> returnedDictionary = new Dictionary<string, string>();

    //string loadedVariables = File.ReadAllText(GetFullSaveFilePath(saveSlot));
    string loadedVariables = PlayerPrefs.GetString(GetSaveFileName(saveSlot));

    string[] separatedVariablePairs = loadedVariables.Split('@');
    foreach(string variablePair in separatedVariablePairs) {
      string[] pair = variablePair.Split('=');
      if(pair[0] == "") { //end of variables
        break;
      }
      if(pair.Length != 2) {
        Debug.Log("<color=red>ERROR: Invalid saved variables string: " + variablePair.ToString() + ". Expected \"varname=value\"</color>");
      }

      string variableName = pair[0];
      string variableValue = pair[1];
      returnedDictionary.Add(variableName, variableValue);
    }

    return returnedDictionary;
  }

  private Dictionary<string, string> PrefixDictionary(Dictionary<string, string> dictionary, int saveSlot) {
    Dictionary<string, string> returnedDictionary = new Dictionary<string, string>();

    foreach(KeyValuePair<string, string> entry in dictionary) {
      string prefixedKey = saveSlot.ToString() + "_" + entry.Key;
      returnedDictionary.Add(prefixedKey, entry.Value);
    }

    return returnedDictionary;
  }

}
