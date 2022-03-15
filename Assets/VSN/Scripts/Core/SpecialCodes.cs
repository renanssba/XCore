using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCodes {

  public static string InterpretStrings(string initialString) {
    string currentString = initialString;

    if(initialString == null) {
      return "";
    }

    if(!initialString.Contains("\\")) {
      return initialString;
    }

    do {
      initialString = currentString;

      currentString = InterpretVariableValue(currentString);
      currentString = InterpretEnemyName(currentString);
      currentString = InterpretLeanText(currentString);

      if(VsnAudioManager.instance.musicPlayer.loopSource.clip != null) {
        currentString = currentString.Replace("\\currentMusic", VsnAudioManager.instance.musicPlayer.loopSource.clip.name);
      }
      currentString = currentString.Replace("\\day", VsnSaveSystem.GetIntVariable("day").ToString());
      currentString = currentString.Replace("\\n", "\n");
      currentString = currentString.Replace("\\q", "\"");
    } while(currentString != initialString);

    return currentString;
  }

  public static string InterpretVariableValue(string initial) {
    int start = initial.IndexOf("\\vsn(");
    if(start == -1) {
      return initial;
    }

    int end = initial.Substring(start, initial.Length-start).IndexOf(")") + start;

    Debug.LogWarning("Initial string: " + initial);

    if(end == -1) {
      return initial;
    }

    Debug.LogWarning("Start pos is: " + start + ", end pos is: " + end);

    string varName = initial.Substring(start + 5, (end - start - 5));
    string varString = GetPrintableVariableValue(varName);

    //Debug.LogWarning("VAR NAME IS: " + varName +", its value is " + varString);

    string final = initial.Substring(0, start);
    final += varString + initial.Substring(end + 1, initial.Length - end - 1);

    //Debug.LogWarning("VARIABLE INTERPRETATION:\nFrom: "+initial+"\nTo: "+final);

    return final;
  }

  public static string InterpretEnemyName(string initial) {
    int start = initial.IndexOf("\\enemy_name(");
    int end = initial.IndexOf(")");

    if(start == -1 || end == -1) {
      return initial;
    }

    string enemyId = initial.Substring(start + 12, (end - start - 12));
    string enemyName = BattleController.instance.allEnemies[int.Parse(enemyId)].GetName();

    string final = initial.Substring(0, start);
    final += enemyName + initial.Substring(end + 1, initial.Length - end - 1);

    return final;
  }

  public static string InterpretLeanText(string initial) {
    int start = initial.IndexOf("\\lean(");
    int end = initial.IndexOf(")");

    if(start == -1 || end == -1) {
      return initial;
    }

    string leanLocalizationKey = initial.Substring(start + 6, (end - start - 6));
    string leanLocalizationText = Lean.Localization.LeanLocalization.GetTranslationText(leanLocalizationKey);

    string final = initial.Substring(0, start);
    final += leanLocalizationText + initial.Substring(end + 1, initial.Length - end - 1);

    return final;
  }

  static string GetPrintableVariableValue(string varName) {
    int intValue = VsnSaveSystem.GetIntVariable(varName);
    string stringValue = VsnSaveSystem.GetStringVariable(varName);

    if(stringValue != "") {
      return stringValue;
    } else {
      return intValue.ToString();
    }
  }


  public static float InterpretFloat(string keycode) {
    if(!keycode.Contains("#")) {
      return 0f;
    }

    return InterpretSpecialNumber(keycode);
  }

  static float InterpretSpecialNumber(string keycode) {
    int count = 0;
    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();

    switch(keycode) {
      case "#random100":
        return Random.Range(0, 100);
      case "#battlersLength":
        if(GameController.instance != null && GameController.instance.currentCombat != null) {
          return GameController.instance.currentCombat.characters.Count;
        }
        return 0;
      case "#enemiesLength":
        return BattleController.instance.enemyMembers.Length;
      case "#heroesAlive":
        foreach(Pilot pilot in BattleController.instance.partyMembers) {
          if(pilot.hp > 0) {
            count++;
          }
        }
        return count;
      case "#enemiesAlive":
        foreach(Enemy en in BattleController.instance.enemyMembers) {
          if(en.hp > 0) {
            count++;
          }
        }
        return count;
      case "#inventory_empty":
        return GlobalData.instance.pilots[0].inventory.IsEmpty() ? 1f : 0f;
      case "#currentRelationshipLevel":
        if(currentRelationship != null) {
          return currentRelationship.level;
        } else {
          return -1;
        }
      case "#currentBattlerStatusConditionsCount":
        if(BattleController.instance.CurrentBattler != null) {
          return BattleController.instance.CurrentBattler.statusConditions.Count;
        }
        break;
      /// TODO: reimplement for passive skills
      //case "#currentCoupleSkillsCount":
      //  if(currentRelationship != null) {
      //    return currentRelationship.skilltree.skills.Length;
      //  } else {
      //    return -1;
      //  }
      //case "#enemySkillsCount":
      //  if(enemy != null) {
      //    return enemy.passiveSkills.Length;
      //  } else {
      //    return -1;
      //  }
      default:
        return 0f;
    }
    return 0f;
  }

}
