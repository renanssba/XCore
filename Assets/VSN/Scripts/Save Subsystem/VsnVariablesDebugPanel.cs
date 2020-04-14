using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class VsnVariablesDebugPanel : MonoBehaviour {

  public Transform variableEntriesContent;
  public GameObject entryPrefab;
  public TMP_InputField searchInputField;

  public GameObject customizeVariablePanel;
  public TextMeshProUGUI titleText;
  public Toggle[] typeToggles;
  public TMP_InputField inputField;
  public Toggle[] boolToggles;

  public List<VsnVariableEntry> varEntries;

  public TMP_InputField loadVsnInputField;

  public string selectedVariable;


  public void Initialize() {
    if(gameObject.activeSelf) {
      gameObject.SetActive(false);
    } else {
      gameObject.SetActive(true);
      Utils.SelectUiElement(searchInputField.gameObject);
      searchInputField.text = "";
      UpdateEntries();
    }
  }

  public void Update() {
    if((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
       && customizeVariablePanel.gameObject.activeSelf) {
      ConfirmVariableCustomization();
    }
  }

  public void UpdateEntries() {
    ClearEntries();
    foreach(KeyValuePair<string, string> entry in VsnSaveSystem.savedDataDictionary) {
      if(string.IsNullOrEmpty(searchInputField.text) || GetPrintableName(entry.Key).Contains(searchInputField.text)) {
        CreateEntry(entry.Key);
      }
    }
  }

  public void ClearEntries() {
    int childCount = variableEntriesContent.childCount;

    for(int i = 0; i < childCount; i++) {
      Destroy(variableEntriesContent.GetChild(i).gameObject);
    }
    varEntries = new List<VsnVariableEntry>();
  }

  public GameObject CreateEntry(string varName) {
    GameObject obj = Instantiate(entryPrefab, variableEntriesContent);
    obj.GetComponent<VsnVariableEntry>().Initialize(varName);
    return obj;
  }


  public void CustomizeVariable(string keycode) {
    selectedVariable = keycode;
    titleText.text = "Set value for: " + GetPrintableName(selectedVariable);
    customizeVariablePanel.SetActive(true);

    //SetVariableType();

    switch(GetPrefix(selectedVariable)) {
      case "VARNUMBER":
        typeToggles[0].isOn = true;
        SetVariableType();
        inputField.text = VsnSaveSystem.GetFloatVariable(GetPrintableName(selectedVariable)).ToString();
        Utils.SelectUiElement(inputField.gameObject);
        break;
      case "VARSTRING":
        typeToggles[1].isOn = true;
        SetVariableType();
        inputField.text = VsnSaveSystem.GetStringVariable(GetPrintableName(selectedVariable));
        Utils.SelectUiElement(inputField.gameObject);
        break;
      case "VARBOOL":
        typeToggles[2].isOn = true;
        SetVariableType();
        boolToggles[0].isOn = VsnSaveSystem.GetBoolVariable(GetPrintableName(selectedVariable));
        boolToggles[1].isOn = !VsnSaveSystem.GetBoolVariable(GetPrintableName(selectedVariable));
        break;
    }
  }

  public void SetVariableType() {
    Debug.LogWarning("SetVariableType");

    if(typeToggles[0].isOn) {
      inputField.gameObject.SetActive(true);
      inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
      boolToggles[0].gameObject.SetActive(false);
      boolToggles[1].gameObject.SetActive(false);
      Utils.SelectUiElement(inputField.gameObject);
      inputField.text = "";
    } else if(typeToggles[1].isOn) {
      inputField.gameObject.SetActive(true);
      inputField.contentType = TMP_InputField.ContentType.Standard;
      boolToggles[0].gameObject.SetActive(false);
      boolToggles[1].gameObject.SetActive(false);
      Utils.SelectUiElement(inputField.gameObject);
      inputField.text = "";
    } else if(typeToggles[2].isOn) {
      inputField.gameObject.SetActive(false);
      boolToggles[0].gameObject.SetActive(true);
      boolToggles[1].gameObject.SetActive(true);
    }
  }


  public void ConfirmVariableCustomization() {
    float value;

    if(typeToggles[0].isOn) {
      if(float.TryParse(inputField.text, out value)) {
        VsnSaveSystem.SetVariable(GetPrintableName(selectedVariable), value);
      }
    } else if(typeToggles[1].isOn) {
      VsnSaveSystem.SetVariable(GetPrintableName(selectedVariable), inputField.text);
    } else if(typeToggles[2].isOn) {
      VsnSaveSystem.SetVariable(GetPrintableName(selectedVariable), boolToggles[0].isOn);
    }
    UpdateEntries();
    customizeVariablePanel.SetActive(false);
    Utils.SelectUiElement(searchInputField.gameObject);
  }

  public void ClickLoadVsnScriptButton() {
    if(string.IsNullOrEmpty(loadVsnInputField.text)) {
      Debug.LogError("Error: load Vsn Input Field is empty");
      return;
    }
    VsnController.instance.StartVSN(loadVsnInputField.text);
    gameObject.SetActive(false);
  }



  public static string GetPrintableName(string baseName) {
    int prefixSize = baseName.IndexOf("_");
    prefixSize++;
    return baseName.Substring(prefixSize, baseName.Length - prefixSize);
  }

  public static string GetPrefix(string baseName) {
    if(baseName.StartsWith("VARNUMBER")) {
      return "VARNUMBER";
    }
    if(baseName.StartsWith("VARSTRING")) {
      return "VARSTRING";
    }
    if(baseName.StartsWith("VARBOOL")) {
      return "VARBOOL";
    }
    return "";
  }
}
