using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VsnLogWindow : MonoBehaviour {
  
  public ScreenTransitions logWindowPanel;
  public TextMeshProUGUI logText;
  public ScrollRect logScrollRect;
  public string logString;


  public void Awake(){
    //ResetLog();
    //UpdateText();
  }


  public void AddToLog(string text){
    AddToLog(null, text);
  }


  public void AddToLog(string speakerName, string text){
    if(logString != "") {
      logString += "\n\n";
    }

    if(!string.IsNullOrEmpty(speakerName)){
      logString += "<color=#E18F9D>" + speakerName + "</color>\n";
      logString += text;
    } else {
      logString += "<i>"+text+"</i>";
    }
    UpdateText();
  }

  public void ResetLog(){
    Debug.LogWarning("Clearing log");
    logString = "";
    UpdateText();    
  }

  void UpdateText(){
    logText.text = logString;
  }


  public void OpenLogWindow(){
    UpdateText();
    logWindowPanel.ShowPanel();
    logScrollRect.verticalNormalizedPosition = 0f;
  }

  public void CloseLogWindow(){
    logWindowPanel.HidePanel();
  }
}
