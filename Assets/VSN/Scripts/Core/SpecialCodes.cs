using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCodes {

  public static string InterpretStrings(string initialString){
    string currentString = initialString;
    int currentCouple = GlobalData.instance.currentCouple;

    if (initialString == null) {
      return "";
    }

    if(!initialString.Contains("\\")){
      return initialString;
    }

    do {
      initialString = currentString;
      
      currentString = currentString.Replace("\\n", "\n");
      currentString = currentString.Replace("\\couple", GlobalData.instance.CurrentCoupleName());
      currentString = currentString.Replace("\\currentEventName", GameController.instance.date[GameController.instance.currentDateEvent].scriptName);
      currentString = currentString.Replace("\\boy", GlobalData.instance.people[currentCouple * 2].name);
      currentString = currentString.Replace("\\girl", GlobalData.instance.people[currentCouple * 2+1].name);

    } while (currentString != initialString);

    return currentString;
  }


  public static float InterpretFloat(string keycode){
    if(!keycode.Contains("#")){
      return 0f;
    }

    return InterpretSpecialNumber(keycode);
  }

  static float InterpretSpecialNumber(string keycode){
    switch (keycode){
      case "#random100":
        return Random.Range(0, 100);
      case "#currentDateEvent":
        return GameController.instance.currentDateEvent;
      default:
        return 0f;
    }
    return 0f;
  }
}
