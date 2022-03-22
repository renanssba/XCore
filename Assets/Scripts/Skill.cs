using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType {
  attack,
  active,
  passive
}

public enum SkillRange {
  self,
  one_ally,
  other_ally,
  one_enemy,
  anyone,
  all_allies,
  all_enemies,
  random_enemy,
  all_but_caster,
  all_characters,
  none
}

public enum SkillTarget {
  partyMember1 = 0,
  partyMember2 = 1,
  partyMember3 = 2,
  enemy1 = 3,
  enemy2 = 4,
  enemy3 = 5,

  allEnemies = 6,
  allHeroes = 7,
  allCharacters = 8,

  none = 9,

  allButPartyMember1 = 10,
  allButPartyMember2 = 11,
  allButPartyMember3 = 12,
  allButEnemy1 = 13,
  allButEnemy2 = 14,
  allButEnemy3 = 15,
}

public enum SkillSpecialEffect {
  sensor,
  buffDebuff,
  healingItemBonus,
  healingSkillBonus,
  damageTakenBonus,
  damageGivenBonus,
  raiseMaxHp,
  raiseMaxSp,
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
  charged_attack,
  active_support,
  active_offensive,
  passive,
  long_charge,
  throw_object,
  multi_throw,
  projectile,
  shout,
  interact,
  none
}


[System.Serializable]
public struct SkilltreeEntry {
  public int id;
  public bool isUnlocked;
}


[System.Serializable]
public class Skilltree {
  public SkilltreeEntry[] skills;
  public static readonly int[] skillRequisites = { 8, 0, 0, -1, 8, 4, 4, -1, -1, 8, 9, 9, -1 };


  public Skilltree() {
    skills = new SkilltreeEntry[1];

    skills[0].isUnlocked = true;
  }

  public void Skilltree_old() {
    skills = new SkilltreeEntry[13];
    for(int i = 0; i < skills.Length; i++) {
      skills[i].isUnlocked = false;
    }

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
public class ActionSkin {
  public string name;
  public string sfxName;
  public SkillAnimation animation;
  public string animationArgument;

  public GameObject projectilePrefab;
}



[System.Serializable]
public class Skill {
  public string name;
  public int id;

  public Sprite sprite;
  public SkillType type;
  public SkillRange range;

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

  public ActionSkin animationSkin;
  public string[] tags;


  public string GetPrintableName() {
    return Lean.Localization.LeanLocalization.GetTranslationText("skill/name/" + name);
    //return SpecialCodes.InterpretStrings(name);
  }

  public string GetPrintableDescription() {
    return Lean.Localization.LeanLocalization.GetTranslationText("skill/description/" + name);
    //return SpecialCodes.InterpretStrings(description);
  }

  public bool HasTag(string tagToSearch) {
    return Utils.TagIsInArray(tagToSearch, tags);
  }
}


