using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attributes {
  maxHp = 0,
  movementRange = 1,
  attack = 2,
  agility = 3,
  dodgeRate = 4
}

public enum PersonState {
  unrevealed,
  available,
  shipped
}

public enum PersonId {
  main = 0
}


[System.Serializable]
public class Pilot : Battler {

  public bool isMale;

  protected int initialMaxSp;
  public int sp;
  public int skillPoints;

  public Inventory inventory;
  public Inventory giftsReceived;


  public Pilot() {
    inventory = new Inventory();
    giftsReceived = new Inventory();
    inventory.owner = this;
    giftsReceived.owner = this;
    initialMaxSp = 4;
    sp = 4;
    attributes = new int[] { 1, 1, 1, 1 };
  }


  public override string GetName() {
    return Lean.Localization.LeanLocalization.GetTranslationText("char_name/" + nameKey);
  }

  public bool CanExecuteSkill(Skill skillToUse) {
    if(CurrentStatusConditionStacks("spotted") > 0) {
      return false;
    }
    if(sp < skillToUse.spCost) {
      return false;
    }
    return true;
  }

  public override bool IsSpotted() {
    return CurrentStatusConditionStacks("spotted") > 0;
  }

  public override int MaxHP() {
    return BattleController.instance.maxHp;
  }

  public override int CurrentHP() {
    return BattleController.instance.hp;
  }

  public override int MaxSP() {
    return GetMaxSp(GlobalData.instance.GetCurrentRelationship().id);
  }

  public override int CurrentSP() {
    return sp;
  }

  public override int Level() {
    return GlobalData.instance.GetCurrentRelationship().level;
  }


  public int GetMaxSp(int relationshipId) {
    Relationship relationship = GlobalData.instance.relationships[relationshipId];
    int count = initialMaxSp;
    Skill[] skills = relationship.GetPassiveSkillsByCharacter(isMale);

    if(relationship.level >= 4) {
      count += 3;
    }
    if(relationship.level >= 6) {
      count += 5;
    }

    for(int i = 0; i < skills.Length; i++) {
      if(skills[i].specialEffect == SkillSpecialEffect.raiseMaxSp) {
        count += (int)skills[i].effectPower;
      }
    }
    return count;
  }


  public override void HealSp(int value) {
    sp += value;
    sp = Mathf.Min(sp, GetMaxSp(GlobalData.instance.GetCurrentRelationship().id));
    sp = Mathf.Max(sp, 0);
    UIController.instance.UpdateBattleUI();
  }

  public override bool IsDefending() {
    SkillTarget partyMemberPosition = BattleController.instance.GetPartyMemberPosition(this);
    if(partyMemberPosition == SkillTarget.none) {
      return false;
    }
    return BattleController.instance.selectedActionType[(int)partyMemberPosition] == TurnActionType.defend ||
           CurrentStatusConditionStacks("guardian")>0;
  }

  public override int FightingSide() {
    return 1;
  }

  public override void SpendSp(int value) {
    sp -= value;
    //sp = Mathf.Min(sp, maxSp);
    sp = Mathf.Max(sp, 0);
    UIController.instance.UpdateBattleUI();
  }

  public Skill[] GetActiveSkills(int relationshipId) {
    List<Skill> skills = new List<Skill>();
    Relationship relationship = GlobalData.instance.relationships[relationshipId];

    skills.Add(BattleController.instance.GetSkillById(0));
    //skills.Add(BattleController.instance.GetSkillById(1));
    //skills.Add(BattleController.instance.GetSkillById(2));
    skills.AddRange(relationship.GetActiveSkillsByCharacter());

    return skills.ToArray();
  }

  public Skill[] GetAllCharacterSpecificSkills(int relationshipId) {
    Relationship relationship = GlobalData.instance.relationships[relationshipId];

    return relationship.GetAllCharacterSpecificSkills(isMale);
  }


  public override Skill[] GetPassiveSkills() {
    Relationship relationship = GlobalData.instance.relationships[GlobalData.instance.GetCurrentRelationship().id];
    return relationship.GetPassiveSkillsByCharacter(isMale);
  }

  public override float HealingSkillMultiplier() {
    return GlobalData.instance.GetCurrentRelationship().HealingSkillMultiplier();
  }

  public override float GetAttributeEffectivity(Attributes att) {
    return 1f;
  }


  public override Actor2D GetActor2D() {
    return TheaterController.instance.GetActorByBattlingCharacter(this);
  }
  

  public override void HealHP(int value) {
    BattleController.instance.HealPartyHp(value);
  }

  public override void HealHpPercent(float fraction) {
    BattleController.instance.HealPartyHp((int)(BattleController.instance.maxHp * fraction));
  }

  public override void TakeDamage(int value) {
    BattleController.instance.DamagePartyHp(value);
  }

  public Relationship[] GetRelationships() {
    /// TODO: Implement
    List<Relationship> myRelationships = new List<Relationship>();
    foreach(Relationship rel in GlobalData.instance.relationships) {
      if(rel.people[0] == this || rel.people[1] == this) {
        myRelationships.Add(rel);
      }
    }
    return myRelationships.ToArray();
  }
}


[System.Serializable]
public class Relationship {
  public int id;
  public Pilot[] people;
  public int level = 0;
  public int exp = 0;

  public int bondPoints = 0;
  public Skilltree skilltree;

  public static readonly int[] levelUpCosts = {20, 40, 60, 100, 180, 240, 340, 500, 600, 800};
  public static readonly int[] childrenNumber = {5, 4, 6};
  public const int skillTreeSize = 13;

  public List<string> talkedDialogs;


  public Relationship() {
    level = 0;
    talkedDialogs = new List<string>();
    skilltree = new Skilltree();
  }

  public static int LevelStartingExp(int level) {
    int count = 0;

    for(int i=0; i<level;  i++) {
      count += levelUpCosts[i];
    }
    return count;
  }

  public static int LevelUpNeededExp(int level) {
    if(level >= 10) {
      return -1;
    }
    return levelUpCosts[level];
  }


  public int GetMaxHp() {
    int count = level * 8 + 30;

    for(int i=0; i<skilltree.skills.Length; i++){
      if(skilltree.skills[i].isUnlocked) {
        Skill skill = BattleController.instance.GetSkillById(skilltree.skills[i].id);
        if(skill.type == SkillType.passive && skill.specialEffect == SkillSpecialEffect.raiseMaxHp) {
          count += (int)skill.effectPower;
        }
      }
    }
    return count;
  }

  public float HealingSkillMultiplier() {
    float count = 1;

    for(int i=0; i<skilltree.skills.Length; i++) {
      Skill skill = BattleController.instance.GetSkillById(skilltree.skills[i].id);
      if(skilltree.skills[i].isUnlocked && skill.type == SkillType.passive &&
         skill.specialEffect == SkillSpecialEffect.healingSkillBonus) {
        count += skill.effectPower;
      }      
    }
    return count;
  }

  public float HealingItemMultiplier() {
    float count = 1;

    for(int i = 0; i < skilltree.skills.Length; i++) {
      Skill skill = BattleController.instance.GetSkillById(skilltree.skills[i].id);
      if(skilltree.skills[i].isUnlocked && skill.type == SkillType.passive &&
         skill.specialEffect == SkillSpecialEffect.healingItemBonus) {
        count += skill.effectPower;
      }
    }
    return count;
  }



  public bool GetExp(int value) {
    bool didLevelUp = false;

    exp += value;
    while(level < 10 && exp - LevelStartingExp(level) >= LevelUpNeededExp(level)) {
      level++;
      if(level > 2) {
        bondPoints++;
      }
      didLevelUp = true;
    }
    return didLevelUp;
  }

  public string GetRelationshipLevelDescription() {
    return Lean.Localization.LeanLocalization.GetTranslationText("relationship_level/colleagues");
  }

  public Skill[] GetActiveSkillsByCharacter() {
    List<Skill> skills = new List<Skill>();

    for(int i = 0; i < skilltree.skills.Length; i++) {
      //if((isBoy && skilltree.skills[i].affectsPerson != SkillAffectsCharacter.girl) ||
      //   (!isBoy && skilltree.skills[i].affectsPerson != SkillAffectsCharacter.boy)) {
      AddActiveSkillToList(skills, i);
      //}
    }
    return skills.ToArray();
  }

  public Skill[] GetAllCharacterSpecificSkills(bool isBoy) {
    List<Skill> skills = new List<Skill>();

    for(int i=0; i<skilltree.skills.Length; i++) {
      if((isBoy && skilltree.skills[i].affectsPerson == SkillAffectsCharacter.boy) ||
         (!isBoy && skilltree.skills[i].affectsPerson == SkillAffectsCharacter.girl) ) {
        AddActiveSkillToList(skills, i);
        AddPassiveSkillToList(skills, i);
      }
    }
    return skills.ToArray();
  }

  public Skill[] GetPassiveSkillsByCharacter(bool isBoy) {
    List<Skill> skills = new List<Skill>();

    for(int i = 0; i < skilltree.skills.Length; i++) {
      if((isBoy && skilltree.skills[i].affectsPerson != SkillAffectsCharacter.girl) ||
         (!isBoy && skilltree.skills[i].affectsPerson != SkillAffectsCharacter.boy)) {
        AddPassiveSkillToList(skills, i);
      }
    }
    return skills.ToArray();
  }


  public void AddActiveSkillToList(List<Skill> list, int id) {
    Skill sk = BattleController.instance.GetSkillById(skilltree.skills[id].id);

    if(skilltree.skills[id].isUnlocked && (sk.type == SkillType.active || sk.type == SkillType.attack)) {
      list.Add(sk);
    }
  }

  public void AddPassiveSkillToList(List<Skill> list, int id) {
    Skill sk = BattleController.instance.GetSkillById(skilltree.skills[id].id);

    if(skilltree.skills[id].isUnlocked && sk.type == SkillType.passive) {
      list.Add(sk);
    }
  }

  public void LoadFromStruct(RelationshipSaveStruct origin) {
    level = origin.level;
    exp = origin.exp;
    bondPoints = origin.bondPoints;

    skilltree = origin.skilltree;
    talkedDialogs = origin.talkedDialogs;
  }
}


[System.Serializable]
public class RelationshipSaveStruct {
  public int id;
  public int level;
  public int exp;

  public int bondPoints = 0;
  public Skilltree skilltree;

  public List<string> talkedDialogs;


  public RelationshipSaveStruct(Relationship origin) {
    id = origin.id;
    level = origin.level;
    exp = origin.exp;
    bondPoints = origin.bondPoints;

    skilltree = origin.skilltree;
    talkedDialogs = origin.talkedDialogs;
  }
}
