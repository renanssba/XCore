using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class Battler {
  [Header("- Identity -")]
  public string nameKey;
  public int id;

  [Header("- Attributes -")]
  public int hp;
  public int[] attributes;


  [Header("- Status Conditions -")]
  public List<StatusCondition> statusConditions;
  
  public List<int> usedSkillsInBattle;


  public Battler() {
    attributes = new int[5];
    statusConditions = new List<StatusCondition>();
  }



  public abstract string GetName();

  public int GetAttributeValue(Attributes att) {
    return AttributeValue((int)att);
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

  public bool IsPreferredTarget() {
    foreach(StatusCondition sc in statusConditions) {
      if(sc.ContainsStatusEffect(StatusConditionEffect.becomeTarget)) {
        return true;
      }
    }
    return false;
  }

  public abstract Skill[] GetPassiveSkills();

  public abstract float HealingSkillMultiplier();

  public abstract float GetAttributeEffectivity(Attributes att);

  public virtual void HealHP(int value) {
    //Debug.Log("Healed "+nameKey+" for "+value+" hp!");
    hp += value;
    hp = Mathf.Min(hp, GetAttributeValue(Attributes.maxHp));
    hp = Mathf.Max(hp, 0);
    UIController.instance.UpdateBattleUI();
  }

  public virtual void HealHpPercent(float fraction) {
    HealHP((int)(GetAttributeValue(Attributes.maxHp) * fraction));
  }

  public virtual void TakeDamage(int value) {
    HealHP(-value);
  }

  public abstract void HealSp(int value);

  public virtual bool IsDefending() {
    return false;
  }

  public abstract int FightingSide();

  public virtual void SpendSp(int value) {}

  public virtual int StatusResistance(string statusName) {
    return 0;
  }

  public virtual int MaxHP() {
    return GetAttributeValue(Attributes.maxHp);
  }

  public virtual int CurrentHP() {
    return hp;
  }

  public virtual int MaxSP() { return 0; }

  public virtual int CurrentSP() { return 0; }

  public void ClearAllSkillsUsage() {
    ClearSkillUsesInBattle();
  }

  public void RegisterUsedSkill(int skillId) {
    usedSkillsInBattle.Add(skillId);
  }

  public int CheckSkillUsesInBattle(int skillId) {
    int count = 0;
    foreach(int used in usedSkillsInBattle) {
      if(skillId == used) {
        count++;
      }
    }
    return count;
  }

  public int CheckTimeSinceLastUsedSkill(int skillId) {
    for(int i=0; i<usedSkillsInBattle.Count; i++) {
      if(usedSkillsInBattle[usedSkillsInBattle.Count-1-i] == skillId) {
        return i;
      }
    }
    return 1000;
  }

  public void ClearSkillUsesInBattle() {
    usedSkillsInBattle = new List<int>();
  }


  /// ///
  /// STATUS CONDITIONS SEGMENT
  /// ///

  public int FindStatusCondition(string conditionName) {
    for(int i=0; i< statusConditions.Count; i++) {
      if(statusConditions[i].name == conditionName) {
        return i;
      }
    }
    return -1;
  }


  public void ReceiveStatusConditionBySkill(Skill usedSkill, string currentEffectStatusCondition) {
    StatusCondition newCondition;

    //Debug.LogWarning("Received skill: " + usedSkill.name);
    newCondition = BattleController.instance.GetStatusConditionByName(currentEffectStatusCondition);
    newCondition = newCondition.GenerateClone();
    newCondition.duration = usedSkill.duration;
    if(newCondition.duration > 0) {
      newCondition.duration++;
    }
    newCondition.maxDurationShowable = usedSkill.duration;
    ReceiveStatusCondition(newCondition);
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
      ReceiveStatusCondition(newCondition);
    }
  }

  public bool ReceiveStatusCondition(StatusCondition newCondition) {
    int i = FindStatusCondition(newCondition.name);
    bool receivedNewStatus = false;
    Actor2D actor = TheaterController.instance.GetActorByBattler(this);

    Debug.LogWarning("receiving new condition: " + newCondition.name+", turns: "+newCondition.duration);

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
    UIController.instance.UpdateBattleUI();
    UpdateActorGraphics();
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

  public float TotalStatusEffectPower(StatusConditionEffect effect) {
    float count = 0;

    foreach(StatusCondition sc in statusConditions) {
      for(int i = 0; i < sc.statusEffect.Length; i++) {
        if(sc.statusEffect[i] == effect) {
          count += sc.statusEffectPower[i];
        }
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
    for(int i = statusConditions.Count-1; i >= 0; i--) {
      if(statusConditions[i].name == name) {
        statusConditions.RemoveAt(i);
      }
    }
    UIController.instance.UpdateBattleUI();
    UpdateActorGraphics();
  }

  public void RemoveAllStatusConditions() {
    statusConditions.Clear();
    UIController.instance.UpdateBattleUI();
    UpdateActorGraphics();
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
    UIController.instance.UpdateBattleUI();
    UpdateActorGraphics();
  }
  


  public void ActivateStatusConditionEffect(int statusCondPos) {
    StatusCondition sc = statusConditions[statusCondPos];
    int statusEffectStacks = 0;
    bool damageDealtBefore;

    if(sc.duration > sc.maxDurationShowable) {
      return;
    }

    for(int i=0; i<sc.statusEffect.Length; i++) {
      if(sc.statusEffect[i] == StatusConditionEffect.damageEveryTurn) {
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

        float damagePercentage = 0.025f * damage;
        int totaldamage = Mathf.RoundToInt(damagePercentage * MaxHP());
        //Debug.LogWarning("STATUS DAMAGE - Damage count: "+damage);
        //Debug.LogWarning("STATUS DAMAGE - Percentage: " + damagePercentage);
        //Debug.LogWarning("STATUS DAMAGE - Total damage: " + totaldamage);

        TakeDamage(totaldamage);
        GetActor2D().ShowDamageParticle(totaldamage, 1f);
        VsnController.instance.WaitForCustomInput();

        VsnSaveSystem.SetVariable("target_name", GetName());
        BattleController.instance.ShowTakeStatusConditionDescription(GetName(), sc.statusEffect[i]);
      }
    }
  }
  
  public abstract Actor2D GetActor2D();

  public void UpdateActorGraphics() {
    Actor2D actor = GetActor2D();
    if(actor != null) {
      actor.UpdateGraphics();
    }
  }

}
