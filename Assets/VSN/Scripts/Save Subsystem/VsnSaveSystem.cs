using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VsnSaveSystem {

  public static Dictionary<string, string> savedDataDictionary;

  static readonly string varNumberPrefix = "VARNUMBER";
  static readonly string varStringPrefix = "VARSTRING";
  static readonly string varBoolPrefix = "VARBOOL";

  private static int saveSlot;
  private static IVsnSaveHandler saveHandler;

  #region getters/setters

  /// <summary>
  /// Gets or sets the current save slot. Starting slot is 1.
  /// Use 1 or more for actual save slots.
  /// </summary>
  /// <value>The save file.</value>
  public static int SaveSlot {
    get {
      return saveSlot;
    }
    set {
      if(value >= 1) {
        saveSlot = value;	
      }
    }
  }

  public static IVsnSaveHandler SaveHandler {
    get;
    set;
  }

  #endregion

  static VsnSaveSystem() {
    SaveSlot = 1;
    SaveHandler = new FileSaveHandler();

    savedDataDictionary = new Dictionary<string, string>();
  }

  #region Prefixes

  static string GetVariableNumberPrefix(string key) {
    return varNumberPrefix + "_" + key;
  }

  static string GetVariableBoolPrefix(string key) {
    return varBoolPrefix + "_" + key;
  }

  static string GetVariableStringPrefix(string key) {
    return varStringPrefix + "_" + key;
  }

  #endregion

  #region Variables (sets, adds, gets)

  public static void SetVariable(string key, int value) {
    SetVariable(key, (float)value);
  }

  public static void SetVariable(string key, float value) {
    Debug.Log("[VAR] " + key + " = " + value);
    string savedKey = GetVariableNumberPrefix(key);

    if(savedDataDictionary.ContainsKey(GetVariableStringPrefix(key))) {
      savedDataDictionary.Remove(GetVariableStringPrefix(key));
    }
    if(savedDataDictionary.ContainsKey(GetVariableBoolPrefix(key))) {
      savedDataDictionary.Remove(GetVariableBoolPrefix(key));
    }

    if(savedDataDictionary.ContainsKey(savedKey)) {
      savedDataDictionary[savedKey] = value.ToString();
    } else {
      savedDataDictionary.Add(savedKey, value.ToString());
    }
    //Save(0);
  }

  public static void SetVariable(string key, bool value) {
    Debug.Log("[VAR] " + key + " = " + value);
    string savedKey = GetVariableBoolPrefix(key);

    if(savedDataDictionary.ContainsKey(GetVariableStringPrefix(key))) {
      savedDataDictionary.Remove(GetVariableStringPrefix(key));
    }
    if(savedDataDictionary.ContainsKey(GetVariableNumberPrefix(key))) {
      savedDataDictionary.Remove(GetVariableNumberPrefix(key));
    }

    if (savedDataDictionary.ContainsKey(savedKey)) {
      savedDataDictionary[savedKey] = value.ToString();
    } else {
      savedDataDictionary.Add(savedKey, value.ToString());
    }
    //Save(0);
  }

  public static void SetVariable(string key, string value) {
    Debug.Log("[VAR] " + key + " = " + value);
    string savedKey = GetVariableStringPrefix(key);

    if(savedDataDictionary.ContainsKey(GetVariableNumberPrefix(key))) {
      savedDataDictionary.Remove(GetVariableNumberPrefix(key));
    }
    if(savedDataDictionary.ContainsKey(GetVariableBoolPrefix(key))) {
      savedDataDictionary.Remove(GetVariableBoolPrefix(key));
    }

    if(savedDataDictionary.ContainsKey(savedKey)) {
      savedDataDictionary[savedKey] = value;
    } else {
      savedDataDictionary.Add(savedKey, value);
    }
    //Save(0);
  }


  public static void AddVariable(string key, float amount) {
    string savedKey = GetVariableNumberPrefix(key);

    if(savedDataDictionary.ContainsKey(savedKey)) {			
      float currentValue;
      if(float.TryParse(savedDataDictionary[savedKey], out currentValue)) {
        savedDataDictionary[savedKey] = (currentValue + amount).ToString();
      }

    } else {
      savedDataDictionary.Add(savedKey, amount.ToString());
    }
    //Save(0);
  }

  public static int GetIntVariable(string key, int defaultValue = 0) {
    return (int)GetFloatVariable(key, (float)defaultValue);
  }

  public static float GetFloatVariable(string key, float defaultValue = 0f) {
    string savedKey = GetVariableNumberPrefix(key);

    if(savedDataDictionary.ContainsKey(savedKey)) {
      float currentValue;
      if(float.TryParse(savedDataDictionary[savedKey], out currentValue)) {	
        return currentValue;
      }
    }

    return defaultValue;
  }

  public static bool GetBoolVariable(string key, bool defaultValue = false) {
    string savedKey = GetVariableBoolPrefix(key);

    if(savedDataDictionary.ContainsKey(savedKey)) {
      if(savedDataDictionary[savedKey] == "True") {
        return true;
      }
      return false;
    }

    return defaultValue;
  }

  public static string GetStringVariable(string key, string defaultValue = "") {
    string savedKey = GetVariableStringPrefix(key);

    if(savedDataDictionary.ContainsKey(savedKey)) {
      return savedDataDictionary[savedKey];
    }

    return defaultValue;
  }

  public static VsnArgType GetVsnVariableType(string variablekey) {
    if(savedDataDictionary.ContainsKey(GetVariableStringPrefix(variablekey))) {
      return VsnArgType.stringArg;
    } else if(savedDataDictionary.ContainsKey(GetVariableNumberPrefix(variablekey))) {
      return VsnArgType.numberArg;
    } else if(savedDataDictionary.ContainsKey(GetVariableBoolPrefix(variablekey))) {
      return VsnArgType.booleanArg;
    }
    return VsnArgType.numberArg;
  }

  #endregion

  #region save/load

  public static void Save(int saveSlot) {		
    SaveHandler.Save(savedDataDictionary, saveSlot, (bool success) => {
      if(success) {
        Debug.LogWarning("VSN SAVE success! Slot: " + saveSlot);
      }
    });
  }

  public static void Load(int saveSlot) {
    SaveHandler.Load(savedDataDictionary, saveSlot, (Dictionary<string,string> dictionary) => {
      if(dictionary != null) {
        savedDataDictionary = dictionary;
        Debug.LogWarning("VSN LOAD success! Slot: " + saveSlot);
      }
    });
  }

  public static bool IsSaveSlotBusy(int saveSlot) {
    return SaveHandler.IsSaveSlotBusy(saveSlot);
  }

  public static Dictionary<string, string> GetSavedDictionary(int saveSlot) {
    return SaveHandler.GetSavedDictionary(saveSlot);
  }

  public static void CleanAllData() {
    savedDataDictionary = new Dictionary<string, string>();
  }

  #endregion

  }
