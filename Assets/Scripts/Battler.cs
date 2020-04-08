using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Battler {
  public string name;
  public int id;
  public int[] attributes;
  public List<StatusCondition> statusConditions;



  public Battler() {
    attributes = new int[4];
    statusConditions = new List<StatusCondition>();
  }



  public int AttributeValue(int att){
    if(attributes == null) {
      return 0;
    }
    int sum = attributes[att];
    if(statusConditions != null) {
      foreach(StatusCondition sc in statusConditions) {
        sum += sc.AttributeBonus(att);
      }
    }
    return Mathf.Max(sum, 0);
  }

  public bool CanExecuteAction(TurnActionType action) {
    if(IsSpotted() && action != TurnActionType.idle) {
      return false;
    }

    switch(action) {
      case TurnActionType.defend:
        if(CurrentStatusConditionStacks("angry") > 0) {
          return false;
        }
        break;
      case TurnActionType.useSkill:
        if(CurrentStatusConditionStacks("fear") > 0) {
          return false;
        }
        break;
    }
    return true;
  }

  
  public float DamageMultiplier() {
    float modifier = 1f;
    if(statusConditions != null) {
      foreach(StatusCondition sc in statusConditions) {
        modifier *= sc.DamageMultiplier();
      }
    }
    return modifier;
  }


  public virtual float DamageTakenMultiplier(Attributes attribute) {
    float modifier = 1f;
    Skill[] skills = GetPassiveSkills();

   for(int i=0; i<skills.Length; i++) {
      if(skills[i].specialEffect == SkillSpecialEffect.damageTakenBonus &&
         skills[i].damageAttribute == attribute) {
        modifier += skills[i].effectPower;
      }
    }
    return modifier;
  }

  public abstract Skill[] GetPassiveSkills();

  public abstract float HealingSkillMultiplier();

  public abstract float GetAttributeEffectivity(Attributes att);

  public abstract void HealHP(int value);
  public abstract void TakeDamage(int value);
  
  public abstract void HealSp(int value);

  public abstract bool IsDefending();

  public virtual bool IsSpotted() { return false; }


  /// ///
  /// STATUS CONDITIONS SEGMENT
  /// ///
  
  public int FindStatusCondition(StatusCondition cond) {
    for(int i=0; i< statusConditions.Count; i++) {
      if(statusConditions[i].name == cond.name) {
        return i;
      }
    }
    return -1;
  }


  public void ReceiveStatusConditionBySkill(Skill usedSkill) {
    StatusCondition newCondition;
    Debug.LogWarning("Used skill: " + usedSkill.name);
    for(int i=0; i<usedSkill.givesConditionNames.Length; i++) {
      newCondition = BattleController.instance.GetStatusConditionByName(usedSkill.givesConditionNames[i]);
      newCondition = newCondition.GenerateClone();
      newCondition.duration = usedSkill.duration;
      if(newCondition.duration > 0) {
        newCondition.duration++;
      }
      newCondition.maxDurationShowable = usedSkill.duration;
      //newCondition.name = usedSkill.GetPrintableName();
      //newCondition.sprite = usedSkill.sprite;
      ReceiveStatusCondition(newCondition);
    }
  }

  public void ReceiveStatusConditionByItem(Item usedItem) {
    StatusCondition newCondition;
    foreach(string condName in usedItem.givesConditionNames) {
      newCondition = BattleController.instance.GetStatusConditionByName(condName);
      newCondition = newCondition.GenerateClone();
      newCondition.duration = usedItem.duration;
      if(newCondition.duration > 0) {
        newCondition.duration++;
      }
      newCondition.maxDurationShowable = usedItem.duration;
      //newCondition.name = usedItem.GetPrintableName();
      //newCondition.sprite = usedItem.sprite;
      ReceiveStatusCondition(newCondition);
    }
  }

  public bool ReceiveStatusCondition(StatusCondition newCondition) {
    int i = FindStatusCondition(newCondition);
    bool receivedNewStatus = false;
    Actor2D actor = TheaterController.instance.GetActorByBattlingCharacter(this);

    if(CurrentStatusConditionStacks(newCondition.name) < newCondition.stackable) {
      statusConditions.Add(newCondition);
      actor.ShowStatusConditionParticle(newCondition);
      receivedNewStatus = true;
    } else if(statusConditions[i].duration < newCondition.duration) {
      statusConditions[i].duration = Mathf.Max(statusConditions[i].duration,
                                               newCondition.duration);
      statusConditions[i].maxDurationShowable = Mathf.Max(statusConditions[i].maxDurationShowable,
                                                          newCondition.maxDurationShowable);
      receivedNewStatus = true;
    }
    UIController.instance.UpdateDateUI();
    actor.UpdateGraphics();
    return receivedNewStatus;
  }

  public int CurrentStatusConditionStacks(string name) {
    int count = 0;
    foreach(StatusCondition sc in statusConditions) {
      if(sc.name == name) {
        count++;
      }
    }
    return count;
  }


  public void RemoveStatusConditionBySkill(Skill usedSkill) {
    foreach(string condName in usedSkill.healsConditionNames) {
      RemoveStatusCondition(condName);
    }
  }

  public void RemoveStatusCondition(string name) {
    Actor2D actor = TheaterController.instance.GetActorByBattlingCharacter(this);

    for(int i = statusConditions.Count-1; i >= 0; i--) {
      if(statusConditions[i].name == name) {
        statusConditions.RemoveAt(i);
      }
    }
    UIController.instance.UpdateDateUI();
    actor.UpdateCharacterGraphics();
  }

  public void RemoveAllStatusConditions() {
    statusConditions.Clear();
    UIController.instance.UpdateDateUI();
  }

  public void EndTurn() {
    for(int i = statusConditions.Count-1; i>=0; i--) {
      /// pass status conditions turn
      if(statusConditions[i].duration > 0) {
        statusConditions[i].duration--;
      }      
      if(statusConditions[i].duration == 0) {
        statusConditions.RemoveAt(i);
      }
    }
    UIController.instance.UpdateDateUI();
  }
  


  public void ActivateStatusConditionEffect(int statusCondPos) {
    StatusCondition sc = statusConditions[statusCondPos];
    int statusEffectStacks = 0;
    bool damageDealtBefore;

    for(int i=0; i<sc.statusEffect.Length; i++) {
      if(sc.statusEffect[i] >= StatusConditionEffect.turnDamageGuts &&
         sc.statusEffect[i] <= StatusConditionEffect.turnDamageCharisma) {
        damageDealtBefore = false;
        for(int j=0; j < statusCondPos; j++) {
          if(statusConditions[j].ContainsStatusEffect(sc.statusEffect[i])) {
            damageDealtBefore = true;
          }
        }
        if(damageDealtBefore) {
          continue;
        }

        statusEffectStacks = 0;
        for(int j = 0; j < statusConditions.Count; j++) {
          if(statusConditions[j].ContainsStatusEffect(sc.statusEffect[i])) {
            statusEffectStacks++;
          }
        }

        //Debug.LogWarning("Activating "+ sc.statusEffect[i]);
        int damage = (int)sc.statusEffectPower[i] - AttributeValue(((int)sc.statusEffect[i])-4)/3;
        damage = Mathf.Max(1, damage);
        damage *= statusEffectStacks;
        TakeDamage(damage);
        GetActor2D().ShowDamageParticle(damage, 1f);
        VsnController.instance.WaitForCustomInput();

        VsnSaveSystem.SetVariable("target_name", name);
        BattleController.instance.ShowTakeStatusConditionDescription(name, sc.statusEffect[i]);
      }
    }
  }
  
  public abstract Actor2D GetActor2D();

}
