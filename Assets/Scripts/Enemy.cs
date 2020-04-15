using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RewardType{
  none,
  item,
  money
}


public enum DateLocation{
  park,
  shopping,
  generic
}

[System.Serializable]
public class ActiveSkillLogic {
  public int skillId;
  public int frequency;
  public string[] conditions;
}

[System.Serializable]
public class CustomEventLogic {
  public string scriptWaypoint;
  public string situationTrigger;
  public string[] conditions;
}


[System.Serializable]
public class Enemy : Battler {

  public string scriptName;
  public float[] attributeEffectivity;
  public int level;
  public int stage;
  public string location;
  public string spriteName;
  public string appearSfxName;

  public int maxHp;
  public int hp;

  public string attackSfxName;

  public ActiveSkillLogic[] activeSkillLogics;
  public CustomEventLogic[] customEvents;
  public int[] passiveSkills;

  public string[] tags;

  public RewardType rewardType;
  public int rewardId;


  public Attributes[] GetWeaknesses() {
    List<Attributes> att = new List<Attributes>();
    for(int i=0; i<3; i++) {
      if(attributeEffectivity[i] > 1f) {
        att.Add((Attributes)i);
      }
    }
    return att.ToArray();
  }

  public Attributes[] GetResistances() {
    List<Attributes> att = new List<Attributes>();
    for(int i = 0; i<3; i++) {
      if(attributeEffectivity[i] < 1f) {
        att.Add((Attributes)i);
      }
    }
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
    return attributeEffectivity[(int)att];
  }


  public override Actor2D GetActor2D() {
    int partyMemberPos = BattleController.instance.GetPartyMemberPosition(this);
    if(partyMemberPos == -1) {
      return null;
    }
    return TheaterController.instance.GetActorByIdInParty(partyMemberPos);
  }
  
  
  public override void HealHP(int value) {
    BattleController.instance.HealEnemyHp(value);
  }

  public override void HealHpPercent(float fraction) {
    BattleController.instance.HealEnemyHp((int)(maxHp*fraction));
  }

  public override void TakeDamage(int value) {
    BattleController.instance.DamageEnemyHp(value);
  }

  public override void HealSp(int value){}

  public override bool IsDefending() {
    return false;
  }

  public override int MaxHP() {
    return maxHp;
  }

  public override int CurrentHP() {
    return hp;
  }

  public override int Level() {
    return level;
  }


  public Skill DecideWhichSkillToUse() {
    List<ActiveSkillLogic> availableSkills = new List<ActiveSkillLogic>();
    int count = 0;

    // decide which skills are available
    for(int i=0; i<activeSkillLogics.Length; i++) {
      if(Utils.AreAllConditionsMet(BattleController.instance.GetSkillById(activeSkillLogics[i].skillId),
         activeSkillLogics[i].conditions, 3)) {
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




  public void UpdateStatusConditions() {
    ClearStatusConditionIcons();

    for(int i = 0; i < statusConditions.Count; i++) {
      GameObject newObj = MonoBehaviour.Instantiate(UIController.instance.statusConditionIconPrefab, UIController.instance.enemyStatusConditionsContent);
      newObj.GetComponent<StatusConditionIcon>().Initialize(statusConditions[i]);
    }
  }

  public void ClearStatusConditionIcons() {
    int childCount = UIController.instance.enemyStatusConditionsContent.transform.childCount;

    for(int i = 0; i < childCount; i++) {
      MonoBehaviour.Destroy(UIController.instance.enemyStatusConditionsContent.transform.GetChild(i).gameObject);
    }
  }

  public bool HasTag(string tagToSearch) {
    return Utils.TagIsInArray(tagToSearch, tags);
  }
}
