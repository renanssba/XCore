using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionRange {
  self,
  oneAlly,
  oneEnemy,
  allAllies,
  allEnemies,
  randomEnemy
}

public enum SkillEffect {
  sensor,
  giveStatusCondition,
  healStatusCondition,
  flee,
  gluttony,
  bondSkill,
  none
}

public enum SkillType {
  attack,
  active,
  passive
}


[System.Serializable]
public class ActionSkin{
  public string name;
  public string id;
  public string buttonName;
  public string sfxName;
}



[System.Serializable]
public class Skill {
  public string name;
  public int id;

  public string description;
  public Sprite sprite;
  public SkillType type;
  public ActionRange range;

  public Attributes attribute;
  public int power;

  public SkillEffect skillEffect;
  public string[] healsConditionNames;
  public string[] givesConditionNames;
  public int duration;
  public int healHp;

  public int spCost;


  public string GetPrintableName() {
    //return Lean.Localization.LeanLocalization.GetTranslationText("skill/name/" + nameKey);
    return SpecialCodes.InterpretStrings(name);
  }

  public string GetPrintableDescription() {
    //return Lean.Localization.LeanLocalization.GetTranslationText("skill/description/" + descriptionKey);
    return SpecialCodes.InterpretStrings(description);
  }
}


