﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attributes{
  guts = 0,
  intelligence = 1,
  charisma = 2,
  endurance = 3
}

public enum PersonState {
  unrevealed,
  available,
  shipped
}

public enum PersonId {
  main = 0,
  fertiliel = 10
}


[System.Serializable]
public class Person : Battler {

  public bool isMale;
  public bool isHuman = true;

  protected int maxSp;
  public int sp;

  public string favoriteMatter;
  public string mostHatedMatter;

  public Inventory inventory;
  public Inventory giftsReceived;

  public int faceId;


  public Person() {
    inventory = new Inventory();
    giftsReceived = new Inventory();
    inventory.owner = this;
    giftsReceived.owner = this;
    maxSp = 3;
    sp = 3;
  }


  public void Initialize(int personId) {
    List<int> attValues = new List<int>();
    switch (Random.Range(0, 2)) {
      case 0:
        attValues.Add(6);
        attValues.Add(6);
        attValues.Add(4);
        break;
      case 1:
        attValues.Add(8);
        attValues.Add(6);
        attValues.Add(2);
        break;
    }
    attValues = attValues.OrderBy(x => Random.value).ToList();

    favoriteMatter = Lean.Localization.LeanLocalization.GetTranslationText("random_taste/taste_" + Random.Range(0, 20));
    mostHatedMatter = Lean.Localization.LeanLocalization.GetTranslationText("random_taste/taste_" + Random.Range(0, 20));

    id = personId;
    faceId = personId;
  }


  public bool CanExecuteSkill(Skill skillToUse) {
    if(CurrentStatusConditionStacks("spotted") > 0) {
      return false;
    }
    if(sp < skillToUse.spCost) {
      return false;
    }
    if(CurrentStatusConditionStacks("angry") > 0 && (skillToUse.type == SkillType.active || skillToUse.damageAttribute != Attributes.guts)) {
      return false;
    }
    return true;
  }

  public override bool IsSpotted() {
    return CurrentStatusConditionStacks("spotted") > 0;
  }


  public int GetMaxSp(int relationshipId) {
    int count = maxSp;
    Relationship relationship = GlobalData.instance.relationships[relationshipId];
    Skill[] skills = relationship.GetPassiveSkillsByCharacter(isMale);

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
    UIController.instance.UpdateDateUI();
  }

  public override bool IsDefending() {
    int partyMemberPosition = BattleController.instance.GetPartyMemberPosition(this);
    if(partyMemberPosition == -1) {
      return false;
    }
    return BattleController.instance.selectedActionType[partyMemberPosition] == TurnActionType.defend;
  }

  public void SpendSp(int value) {
    sp -= value;
    //sp = Mathf.Min(sp, maxSp);
    sp = Mathf.Max(sp, 0);
    UIController.instance.UpdateDateUI();
  }

  public Skill[] GetActiveSkills(int relationshipId) {
    List<Skill> skills = new List<Skill>();
    Relationship relationship = GlobalData.instance.relationships[relationshipId];

    if(id == (int)PersonId.fertiliel) {
      skills.Add(BattleController.instance.GetSkillById(9));
      return skills.ToArray();
    } else {
      skills.Add(BattleController.instance.GetSkillById(0));
      skills.Add(BattleController.instance.GetSkillById(1));
      skills.Add(BattleController.instance.GetSkillById(2));
    }
    skills.AddRange(relationship.GetActiveSkillsByCharacter(isMale));

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

  public override void TakeDamage(int value) {
    BattleController.instance.DamagePartyHp(value);
  }
}


[System.Serializable]
public class Relationship {
  public int id;
  public Person[] people;
  public int level = 0;
  public int exp = 0;
  public int heartLocksOpened = 0;

  public int bondPoints = 0;
  public Skilltree skilltree;

  public static readonly int[] levelUpCosts = {20, 40, 100, 180, 300, 480, 750, 1150, 1700, 2500};
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

  public bool OpenHeartLock(int lockId) {
    bool raised = false;
    if(lockId > heartLocksOpened) {
      heartLocksOpened = lockId;
      raised = true;
    }
    UIController.instance.UpdateUI();
    return raised;
  }


  public Person GetBoy() {
    return people[0];
  }

  public Person GetGirl() {
    return people[1];
  }

  public int GetMaxHp() {
    int count = level * 8 + 10;

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

  public float FleeChance() {
    float count = 0.6f;

    for(int i = 0; i < skilltree.skills.Length; i++) {
      Skill skill = BattleController.instance.GetSkillById(skilltree.skills[i].id);
      if(skilltree.skills[i].isUnlocked && skill.type == SkillType.passive &&
         skill.specialEffect == SkillSpecialEffect.fleeChanceBonus) {
        count += skill.effectPower;
      }
    }
    Debug.LogWarning("Flee chance: " + (100*count)+"%");
    return count;
  }



  public bool GetExp(int value) {
    bool didLevelUp = false;

    exp += value;
    while(level < 10 && exp - LevelStartingExp(level) >= LevelUpNeededExp(level)) {
      level++;
      bondPoints++;
      didLevelUp = true;
    }
    return didLevelUp;
  }

  public string GetRelationshipLevelDescription() {
    if(GetGirl().name == "Ana" && heartLocksOpened == 0) {
      return "Amigos";
    }
    return Utils.RelationshipNameByHeartLocksOpened(heartLocksOpened);
  }

  public Skill[] GetActiveSkillsByCharacter(bool isBoy) {
    List<Skill> skills = new List<Skill>();

    for(int i = 0; i < skilltree.skills.Length; i++) {
      if((isBoy && skilltree.skills[i].affectsPerson != SkillAffectsCharacter.girl) ||
         (!isBoy && skilltree.skills[i].affectsPerson != SkillAffectsCharacter.boy)) {
        AddActiveSkillToList(skills, i);
      }
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
}
