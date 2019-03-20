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
      currentString = currentString.Replace("\\currentEventName", GameController.instance.GetCurrentEventName());
      currentString = currentString.Replace("\\boy", GlobalData.instance.people[currentCouple * 2].name);
      currentString = currentString.Replace("\\girl", GlobalData.instance.people[currentCouple * 2+1].name);
      currentString = currentString.Replace("\\item_name", Item.GetName(VsnSaveSystem.GetIntVariable("item_id")));
      currentString = currentString.Replace("\\item_price", VsnSaveSystem.GetIntVariable("item_price").ToString());
      currentString = currentString.Replace("\\progress", GlobalData.instance.shippedCouples.Count.ToString());
      currentString = currentString.Replace("\\day", GameController.instance.day.ToString());

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
      case "#dateLength":
        return GameController.instance.date.Length;
      case "#currentDateEvent":
        return VsnSaveSystem.GetIntVariable("currentDateEvent");
      case "#ap":
        return GameController.instance.ap;
      case "#max_ap":
        return GameController.instance.maxAp;
      case "#day":
        return GameController.instance.day;
      case "#max_days":
        return GameController.instance.maxDays;
      case "#progress":
        return GlobalData.instance.shippedCouples.Count;
      case "#currentEventInteractionType":
        return (int)GameController.instance.GetCurrentEvent().interactionType;
      default:
        return 0f;
    }
    return 0f;
  }
}
