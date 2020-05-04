using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType {
  attack,
  active,
  passive
}

public enum ActionRange {
  self,
  one_ally,
  other_ally,
  one_enemy,
  anyone,
  all_allies,
  all_enemies,
  random_enemy,
  none
}

public enum SkillTarget {
  partyMember1 = 0,
  partyMember2 = 1,
  angel = 2,
  enemy1 = 3,
  enemy2 = 4,
  enemy3 = 5,
  allEnemies = 6,
  allHeroes = 7,
  none = 8
}

public enum SkillSpecialEffect {
  sensor,
  buffDebuff,
  fleeChanceBonus,
  healingItemBonus,
  healingSkillBonus,
  damageTakenBonus,
  damageGivenBonus,
  raiseMaxHp,
  raiseMaxSp,
  becomeEnemyTarget,
  divertEnemyTarget,
  reflectEnemyTarget,
  none
}

public enum PassiveSkillActivationTrigger {
  enemy_appears,
  turn_started,
  before_enemy_attack,
  after_enemy_attack,
  before_action,
  after_action,
  before_death_checks,
  none
}

public enum SkillAffectsCharacter {
  boy,
  girl,
  couple
}

public enum SkillAnimation {
  attack,
  run_over,
  charged_attack,
  active_support,
  active_offensive,
  passive,
  long_charge,
  throw_object,
  multi_throw,
  none
}


[System.Serializable]
public struct SkilltreeEntry {
  public int id;
  public bool isUnlocked;
  public SkillAffectsCharacter affectsPerson;
  //public int prerequisite;
}


[System.Serializable]
public class Skilltree {
  public SkilltreeEntry[] skills;
  public static readonly int[] skillRequisites = { 8, 0, 0, -1, 8, 4, 4, -1, -1, 8, 9, 9, -1 };


  public Skilltree() {
    skills = new SkilltreeEntry[13];
    for(int i = 0; i < skills.Length; i++) {
      skills[i].isUnlocked = false;
    }

    for(int i=0; i<4; i++) {
      skills[i].affectsPerson = SkillAffectsCharacter.boy;
      skills[4 + i].affectsPerson = SkillAffectsCharacter.girl;
      skills[8 + i].affectsPerson = SkillAffectsCharacter.couple;
    }
    skills[12].affectsPerson = SkillAffectsCharacter.girl;

    skills[3].isUnlocked = true;
    skills[7].isUnlocked = true;
    skills[8].isUnlocked = true;
  }

  public void InitializeSkillIds(int[] ids) {
    for(int i=0; i<skills.Length; i++) {
      skills[i].id = ids[i];
    }
  }
}


[System.Serializable]
public class ActionSkin{
  public string name;
  public string id;
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

  public int spCost;

  public SkillSpecialEffect specialEffect;
  public float effectPower;

  public Attributes damageAttribute;
  public int damagePower;

  public string[] healsConditionNames;
  public string[] givesConditionNames;
  public int giveStatusChance;
  public int duration;
  public int healHp;
  public float healHpPercent;
  public int healSp;

  public PassiveSkillActivationTrigger activationTrigger;
  public string[] triggerConditions;
  public float triggerChance;

  public SkillAnimation animation;
  public string skillAnimationArgument;

  public string[] tags;


  public string GetPrintableName() {
    //return Lean.Localization.LeanLocalization.GetTranslationText("skill/name/" + nameKey);
    return SpecialCodes.InterpretStrings(name);
  }

  public string GetPrintableDescription() {
    //return Lean.Localization.LeanLocalization.GetTranslationText("skill/description/" + descriptionKey);
    return SpecialCodes.InterpretStrings(description);
  }

  public bool HasTag(string tagToSearch) {
    return Utils.TagIsInArray(tagToSearch, tags);
  }
}


