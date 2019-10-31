using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCodes {

  public static string InterpretStrings(string initialString){
    string currentString = initialString;

    if (initialString == null) {
      return "";
    }

    if(!initialString.Contains("\\")){
      return initialString;
    }

    do {
      initialString = currentString;

      currentString = InterpretVariableValue(currentString);
      
      currentString = currentString.Replace("\\couple", GlobalData.instance.CurrentCoupleName());
      currentString = currentString.Replace("\\currentEventName", "date/" + GameController.instance.GetCurrentDateEventName());
      if(GlobalData.instance.ObservedPerson() != null) {
        currentString = currentString.Replace("\\observedPerson", GlobalData.instance.ObservedPerson().name);
        currentString = currentString.Replace("\\favoriteMatter1", GlobalData.instance.ObservedPerson().favoriteMatter);
      }
      if (GlobalData.instance.EncounterPerson() != null) {
        currentString = currentString.Replace("\\encounterPerson", GlobalData.instance.EncounterPerson().name);
        currentString = currentString.Replace("\\favoriteMatter2", GlobalData.instance.EncounterPerson().favoriteMatter);
      }
      if (GlobalData.instance.CurrentBoy() != null) {
        currentString = currentString.Replace("\\boy", GlobalData.instance.CurrentBoy().name);
        currentString = currentString.Replace("\\guts", GlobalData.instance.EventSolvingAttributeLevel((int)Attributes.guts).ToString());
        currentString = currentString.Replace("\\intelligence", GlobalData.instance.EventSolvingAttributeLevel((int)Attributes.intelligence).ToString());
        currentString = currentString.Replace("\\charisma", GlobalData.instance.EventSolvingAttributeLevel((int)Attributes.charisma).ToString());
      }
      if(GlobalData.instance.CurrentGirl() != null) {
        currentString = currentString.Replace("\\girl", GlobalData.instance.CurrentGirl().name);
      }
      currentString = currentString.Replace("\\progress", GlobalData.instance.shippedCouples.Count.ToString());
      currentString = currentString.Replace("\\day", GlobalData.instance.day.ToString());
      currentString = currentString.Replace("\\item_name", Item.GetName(VsnSaveSystem.GetIntVariable("item_id")));
      currentString = currentString.Replace("\\n", "\n");
      currentString = currentString.Replace("\\q", "\"");
    } while (currentString != initialString);

    return currentString;
  }

  public static string InterpretVariableValue(string initial){
    int start = initial.IndexOf("\\vsn[");
    int end = initial.IndexOf("]");

    if(start == -1 || end == -1){
      return initial;
    }

    string varName = initial.Substring(start+5, (end-start-5));
    string varString = GetPrintableVariableValue(varName);

    //Debug.LogWarning("VAR NAME IS: " + varName +", its value is " + varString);

    string final = initial.Substring(0, start);
    final += varString + initial.Substring(end+1, initial.Length-end-1);

    //Debug.LogWarning("VARIABLE INTERPRETATION:\nFrom: "+initial+"\nTo: "+final);

    return final;
  }

  static string GetPrintableVariableValue(string varName){
    int intValue = VsnSaveSystem.GetIntVariable(varName);
    string stringValue = VsnSaveSystem.GetStringVariable(varName);

    if(stringValue != ""){
      return stringValue;
    }else{
      return intValue.ToString();
    }
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
      case "#isEncounterPersonUnrevealed":
        if(GlobalData.instance.EncounterPerson()!=null) {
          return GlobalData.instance.EncounterPerson().state == PersonState.unrevealed ? 1 : 0;
        } else {
          return -1;
        }
      case "#day":
        return GlobalData.instance.day;
      case "#max_days":
        return GlobalData.instance.maxDays;
      case "#progress":
        return GlobalData.instance.shippedCouples.Count;
      case "#isCurrentPersonMale":
        return GlobalData.instance.ObservedPerson().isMale ? 1 : 0;
      case "#currentPersonality":
        return (int)GlobalData.instance.CurrentPersonPersonality();
      case "#currentEventInteractionType":
        return (int)GameController.instance.GetCurrentDateEvent().interactionType;
      case "#inventory_empty":
        return GlobalData.instance.inventory.IsEmpty() ? 1f : 0f;
      case "#availableCouples":
        if(GameController.instance != null) {
          return GameController.instance.couplesPanelContent.childCount;
        } else {
          return 0;
        }
      default:
        return 0f;
    }
    return 0f;
  }
}
