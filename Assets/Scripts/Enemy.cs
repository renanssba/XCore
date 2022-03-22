using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RewardType {
  none,
  item,
  money
}


[System.Serializable]
public class ActiveSkillLogic {
  public int skillId;
  public int frequency;
  public string[] conditions;
}


[System.Serializable]
public class Enemy : Battler {

  public string spriteName;
  public string appearSfxName;

  public ActionSkin baseAttackSkin;

  public ActiveSkillLogic[] activeSkillLogics;
  public int[] passiveSkills;

  public int expReward;
  public int moneyReward;

  public string[] statusImmunities;
  public string[] tags;



  public Enemy(Enemy copyFrom) {
    nameKey = copyFrom.nameKey;
    id = copyFrom.id;
    attributes = copyFrom.attributes;

    /// HP starts full
    hp = attributes[(int)Attributes.maxHp];

    spriteName = copyFrom.spriteName;
    appearSfxName = copyFrom.appearSfxName;
    baseAttackSkin = copyFrom.baseAttackSkin;

    activeSkillLogics = copyFrom.activeSkillLogics;
    passiveSkills = copyFrom.passiveSkills;
    expReward = copyFrom.hp;
    moneyReward = copyFrom.hp;
    statusImmunities = copyFrom.statusImmunities;
    tags = copyFrom.tags;

    statusConditions = new List<StatusCondition>();
    usedSkillsInBattle = new List<int>();
  }

  public override string GetName() {
    return Lean.Localization.LeanLocalization.GetTranslationText("enemy/name/" + nameKey);
  }

  public Attributes[] GetWeaknesses() {
    List<Attributes> att = new List<Attributes>();
    //for(int i=0; i<3; i++) {
    //  if(attributeEffectivity[i] > 1f) {
    //    att.Add((Attributes)i);
    //  }
    //}
    return att.ToArray();
  }

  public Attributes[] GetResistances() {
    List<Attributes> att = new List<Attributes>();
    //for(int i = 0; i<3; i++) {
    //  if(attributeEffectivity[i] < 1f) {
    //    att.Add((Attributes)i);
    //  }
    //}
    return att.ToArray();
  }


  public override float DamageTakenMultiplier(Attributes attribute) {
    return base.DamageTakenMultiplier(attribute) * GetAttributeEffectivity(attribute);
  }

  public override Skill[] GetPassiveSkills() {
    return new Skill[0];
  }

  public override float HealingSkillMultiplier() {
    return 1f;
  }

  public override float GetAttributeEffectivity(Attributes att) {
    //return attributeEffectivity[(int)att];
    return 1f;
  }


  public override Actor2D GetActor2D() {
    return TheaterController.instance.GetActorByBattler(this);
  }
  
  
  public override void HealSp(int value){}

  public override int FightingSide() {
    return 2;
  }

  public override int StatusResistance(string statusName) {
    if(Utils.TagIsInArray(statusName, statusImmunities)) {
      //Debug.LogWarning("Resitance: 100%");
      return 100;
    }
    return 0;
  }


  public Skill DecideWhichSkillToUse() {
    List<ActiveSkillLogic> availableSkills = new List<ActiveSkillLogic>();
    int count = 0;

    // decide which skills are available
    for(int i=0; i<activeSkillLogics.Length; i++) {
      if(Utils.AreAllConditionsMet(BattleController.instance.GetSkillById(activeSkillLogics[i].skillId),
         activeSkillLogics[i].conditions, SkillTarget.enemy1)) {
        availableSkills.Add(activeSkillLogics[i]);
        count += activeSkillLogics[i].frequency;
      }
    }
    
    // if only one skill available, use it
    if(availableSkills.Count == 1) {
      return BattleController.instance.GetSkillById(availableSkills[0].skillId);
    }

    // decide which available skill to use
    int selection = Random.Range(0, count);
    for(int i = 0; i < availableSkills.Count; i++) {
      if(selection < availableSkills[i].frequency) {
        return BattleController.instance.GetSkillById(availableSkills[i].skillId);
      }
      selection -= availableSkills[i].frequency;
    }
    return null;
  }


  public bool HasTag(string tagToSearch) {
    return Utils.TagIsInArray(tagToSearch, tags);
  }
}
