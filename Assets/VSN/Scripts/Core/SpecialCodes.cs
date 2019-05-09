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
      currentString = currentString.Replace("\\currentEventName", "date/" + GameController.instance.GetCurrentDateEventName());
      currentString = currentString.Replace("\\currentObservationEventName", "observation/" + GameController.instance.GetCurrentObservationEventName());
      if (GlobalData.instance.GetCurrentBoy() != null) {
        currentString = currentString.Replace("\\boy", GlobalData.instance.GetCurrentBoy().name);
        currentString = currentString.Replace("\\girl", GlobalData.instance.GetCurrentGirl().name);
        currentString = currentString.Replace("\\guts", GlobalData.instance.EventSolvingAttributeLevel((int)Attributes.guts).ToString());
        currentString = currentString.Replace("\\intelligence", GlobalData.instance.EventSolvingAttributeLevel((int)Attributes.intelligence).ToString());
        currentString = currentString.Replace("\\charisma", GlobalData.instance.EventSolvingAttributeLevel((int)Attributes.charisma).ToString());
      }
      currentString = currentString.Replace("\\item_name", Item.GetName(VsnSaveSystem.GetIntVariable("item_id")));
      currentString = currentString.Replace("\\item_price", VsnSaveSystem.GetIntVariable("item_price").ToString());
      currentString = currentString.Replace("\\minigame_score", VsnSaveSystem.GetIntVariable("minigame_score").ToString());
      currentString = currentString.Replace("\\objective", VsnSaveSystem.GetIntVariable("objective").ToString());
      currentString = currentString.Replace("\\max_days", VsnSaveSystem.GetIntVariable("max_days").ToString());
      currentString = currentString.Replace("\\progress", GlobalData.instance.shippedCouples.Count.ToString());
      currentString = currentString.Replace("\\day", GlobalData.instance.day.ToString());
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
        return GameController.instance.dateSegments.Length;
      case "#currentDateEvent":
        return VsnSaveSystem.GetIntVariable("currentDateEvent");
      case "#observationLength":
        return GameController.instance.observationSegments.Length;
      case "#observationEventType":
        return (int)GameController.instance.GetCurrentObservationEvent().eventType;
      case "#ap":
        return GlobalData.instance.ap;
      case "#max_ap":
        return GlobalData.instance.maxAp;
      case "#day":
        return GlobalData.instance.day;
      case "#max_days":
        return GlobalData.instance.maxDays;
      case "#progress":
        return GlobalData.instance.shippedCouples.Count;
      case "#currentPersonality":
        return (int)GlobalData.instance.CurrentPersonPersonality();
      case "#currentEventInteractionType":
        return (int)GameController.instance.GetCurrentDateEvent().interactionType;
      case "#inventory_empty":
        return GlobalData.instance.inventory.IsEmpty() ? 1f : 0f;
      default:
        return 0f;
    }
    return 0f;
  }
}
