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

  public Attributes attackAttribute;
  public int attackPower;
  public string attackSfxName;
  public string[] givesConditionNames;
  public int giveStatusConditionChance;

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

  public override void TakeDamage(int value) {
    BattleController.instance.DamageEnemyHp(value);
  }

  public override void HealSp(int value){}

  public override bool IsDefending() {
    return false;
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
}
