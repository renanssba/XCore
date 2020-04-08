using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VsnVariableEntry : MonoBehaviour {

  public TextMeshProUGUI variableNameText;
  public string variableName;
  public TextMeshProUGUI variableValueText;


  public void Initialize(string keycode) {
    variableName = keycode;
    variableNameText.text = VsnVariablesDebugPanel.GetPrintableName(variableName);

    switch(VsnVariablesDebugPanel.GetPrefix(keycode)) {
      case "VARNUMBER":
        variableValueText.text = VsnSaveSystem.GetFloatVariable(VsnVariablesDebugPanel.GetPrintableName(keycode)).ToString();
        break;
      case "VARSTRING":
        variableValueText.text = VsnSaveSystem.GetStringVariable(VsnVariablesDebugPanel.GetPrintableName(keycode));
        break;
      case "VARBOOL":
        variableValueText.text = VsnSaveSystem.GetBoolVariable(VsnVariablesDebugPanel.GetPrintableName(keycode)).ToString();
        break;
    }
  }

  public void ClickedButton() {
    Debug.LogWarning("Clicked button");
    VsnVariablesDebugPanel debugPanel = VsnUIManager.instance.variablesDebugPanel.GetComponent<VsnVariablesDebugPanel>();
    debugPanel.CustomizeVariable(variableName);
  }
}
