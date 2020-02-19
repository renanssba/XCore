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
      currentString = currentString.Replace("\\currentEventName", "date enemies/" + BattleController.instance.GetCurrentDateEventName());
      if(BattleController.instance.GetCurrentPlayer() != null) {
        currentString = currentString.Replace("\\active", BattleController.instance.GetCurrentPlayer().name);
      }
      if(BattleController.instance.GetCurrentTarget() != null) {
        currentString = currentString.Replace("\\target", BattleController.instance.GetCurrentTarget().name);
      }
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
        currentString = currentString.Replace("\\guts", GlobalData.instance.CurrentCharacterAttribute((int)Attributes.guts).ToString());
        currentString = currentString.Replace("\\intelligence", GlobalData.instance.CurrentCharacterAttribute((int)Attributes.intelligence).ToString());
        currentString = currentString.Replace("\\charisma", GlobalData.instance.CurrentCharacterAttribute((int)Attributes.charisma).ToString());
        currentString = currentString.Replace("\\magic", GlobalData.instance.CurrentCharacterAttribute((int)Attributes.magic).ToString());
      }
      if(GlobalData.instance.CurrentGirl() != null) {
        currentString = currentString.Replace("\\girl", GlobalData.instance.CurrentGirl().name);
      }
      currentString = currentString.Replace("\\day", GlobalData.instance.day.ToString());
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
    if(keycode.Contains("#char[") && keycode.Contains("]item_count[") &&
       keycode.Contains("]from[")) {
      int result = InterpretIfCharactersHasItemFromOtherChar(keycode);
      if(result == -1) {
        Debug.LogError("Error parsing item check! Keycode: " + keycode);
      }
      return result;
    }

    if(keycode.Contains("#char[") && keycode.Contains("]item_count[")) {
      int result = InterpretIfCharactersHasItem(keycode);
      if(result == -1) {
        Debug.LogError("Error parsing item check! Keycode: " + keycode);
      }
      return result;
    }

    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();

    switch(keycode){
      case "#random100":
        return Random.Range(0, 100);
      case "#dateLength":
        return BattleController.instance.dateLength;
      case "#partyLength":
        return BattleController.instance.partyMembers.Length;
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
      case "#currentChallengeHp":
        return BattleController.instance.GetCurrentDateEvent().hp;
      case "#currentDateId":
        return BattleController.instance.currentDateId;
      case "#currentDateLocation":
        return (int)BattleController.instance.currentDateLocation;
      case "#currentHp":
        return BattleController.instance.hp;
      case "#isCurrentPersonMale":
        return GlobalData.instance.ObservedPerson().isMale ? 1 : 0;
      case "#inventory_empty":
        return GlobalData.instance.CurrentBoy().inventory.IsEmpty() ? 1f : 0f;
      case "#currentGirlId":
        Person girl = GlobalData.instance.CurrentGirl();
        if(girl != null) {
          return girl.id;
        } else {
          return -1;
        }
      case "#currentBoyId":
        Person boy = GlobalData.instance.CurrentBoy();
        if(boy != null) {
          return boy.id;
        } else {
          return -1;
        }
      case "#currentRelationshipLevel":
        if(currentRelationship != null) {
          return currentRelationship.level;
        } else {
          return -1;
        }
      case "#currentRelationshipHeartLocksOpened":
        if(currentRelationship != null) {
          return currentRelationship.heartLocksOpened;
        } else {
          return -1;
        }
      case "#currentPlayerStatusConditionsCount":
        int currentPlayerId = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
        if(BattleController.instance.partyMembers.Length >= currentPlayerId) {
          Person currentPlayer = BattleController.instance.partyMembers[currentPlayerId];
          return currentPlayer.statusConditions.Count;
        }
        break;
      default:
        return 0f;
    }
    return 0f;
  }

  static int InterpretIfCharactersHasItem(string keycode) {
    string[] divisors = { "#char[", "]item_count[", "]" };
    string[] parts = keycode.Split(divisors, System.StringSplitOptions.RemoveEmptyEntries);

    if(parts.Length < 2) {
      return -1;
    }
    int id;
    int itemId;
    Item toCheck;
    if(int.TryParse(parts[0], out id) == false) {
      return -1;
    }
    if(int.TryParse(parts[1], out itemId)) {
      toCheck = ItemDatabase.instance.GetItemById(itemId);
    } else {
      toCheck = ItemDatabase.instance.GetItemByName(parts[1]);
    }
    if(toCheck == null) {
      return -1;
    }

    return GlobalData.instance.people[id].inventory.ItemCount(toCheck.id);
  }

  static int InterpretIfCharactersHasItemFromOtherChar(string keycode) {
    string[] divisors = { "#char[", "]item_count[", "]from[", "]" };
    string[] parts = keycode.Split(divisors, System.StringSplitOptions.RemoveEmptyEntries);

    if(parts.Length < 3) {
      return -1;
    }
    int personId, itemId, ownerId;
    Item toCheck;
    if(int.TryParse(parts[0], out personId) == false) {
      return -1;
    }
    if(int.TryParse(parts[2], out ownerId) == false) {
      return -1;
    }
    if(int.TryParse(parts[1], out itemId)) {
      toCheck = ItemDatabase.instance.GetItemById(itemId);
    } else {
      toCheck = ItemDatabase.instance.GetItemByName(parts[1]);
    }
    if(toCheck == null) {
      return -1;
    }

    return GlobalData.instance.people[personId].inventory.ItemCountFromOwner(toCheck.id, ownerId);
  }
}
