using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attributes{
  guts = 0,
  intelligence = 1,
  charisma = 2,
  magic = 3
}

public enum PersonState {
  unrevealed,
  available,
  shipped
}


[System.Serializable]
public class Person {

  public string name;
  public bool isMale;
  public bool isHuman = true;

  public PersonState state = PersonState.unrevealed;

  public int[] attributes;

  public int maxSp;
  public int sp;

  public int[] skillIds;

  public string favoriteMatter;
  public string mostHatedMatter;

  public Inventory inventory;

  public List<StatusCondition> statusConditions;

  public int id;
  public int faceId;


  public Person() {
    inventory = new Inventory();
    statusConditions = new List<StatusCondition>();
    inventory.owner = this;
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

    state = PersonState.unrevealed;

    favoriteMatter = Lean.Localization.LeanLocalization.GetTranslationText("random_taste/taste_" + Random.Range(0, 20));
    mostHatedMatter = Lean.Localization.LeanLocalization.GetTranslationText("random_taste/taste_" + Random.Range(0, 20));

    //if (isMale) {
    //  faceId = Random.Range(0, 5);
    //} else {
    //  faceId = 5 + Random.Range(0, 5);
    //}
    id = personId;
    faceId = personId;
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

  public float DamageMultiplier() {
    float modifier = 1f;
    if(statusConditions != null) {
      foreach(StatusCondition sc in statusConditions) {
        modifier *= sc.DamageMultiplier();
      }
    }
    return modifier;
  }


  public void HealSp(int value) {
    sp += value;
    sp = Mathf.Min(sp, maxSp);
    sp = Mathf.Max(sp, 0);
    UIController.instance.UpdateDateUI();
  }

  public void SpendSp(int value) {
    sp -= value;
    sp = Mathf.Min(sp, maxSp);
    sp = Mathf.Max(sp, 0);
    UIController.instance.UpdateDateUI();
  }

  //public void EquipItemInSlot(int slotId, Item item){
  //  VsnAudioManager.instance.PlaySfx("inventory_equip");
  //  equipment = item;
  //  GlobalData.instance.CurrentBoy().inventory.ConsumeItem(equipment.id, 1);
  //  UIController.instance.UpdateUI();
  //}

  //public void UnequipItemInSlot(int slotId) {
  //  if (equipment != null) {
  //    VsnAudioManager.instance.PlaySfx("inventory_equip");
  //    Debug.LogWarning("Unequiping item: " + equipment.id +", " + equipment.nameKey);
  //    GlobalData.instance.CurrentBoy().inventory.AddItem(equipment.id, 1);
  //    equipment = null;
  //    UIController.instance.UpdateUI();
  //  }
  //}

  //public int EquipsCount(){
  //  if (equipment != null) {
  //    return 1;
  //  }
  //  return 0;
  //}

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
      newCondition.duration = usedSkill.duration + 1;
      newCondition.maxDurationShowable = usedSkill.duration;
      newCondition.name = usedSkill.GetPrintableName();
      newCondition.sprite = usedSkill.sprite;
      ReceiveStatusCondition(newCondition);
    }
  }

  public void ReceiveStatusConditionByItem(Item usedItem) {
    StatusCondition newCondition;
    foreach(string condName in usedItem.givesConditionNames) {
      newCondition = BattleController.instance.GetStatusConditionByName(condName);
      newCondition = newCondition.GenerateClone();
      newCondition.duration = usedItem.duration + 1;
      newCondition.maxDurationShowable = usedItem.duration;
      newCondition.name = usedItem.GetPrintableName();
      newCondition.sprite = usedItem.sprite;
      ReceiveStatusCondition(newCondition);
    }
  }

  public bool ReceiveStatusCondition(StatusCondition newCondition) {
    int i = FindStatusCondition(newCondition);
    bool receivedNewStatus = false;
    if(i == -1 || statusConditions[i].stackable) {
      statusConditions.Add(newCondition);
      Actor2D actor = TheaterController.instance.GetActorByPerson(this);
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
    return receivedNewStatus;
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
    UIController.instance.UpdateDateUI();
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
         sc.statusEffect[i] <= StatusConditionEffect.turnDamageMagic) {
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
        BattleController.instance.DamagePartyHp(damage);
        GetActor2D().ShowDamageParticle(((int)sc.statusEffect[i])-4, damage, 1f);
        VsnController.instance.WaitForCustomInput();
        BattleController.instance.ShowActionDescription("receive_damage_" + statusConditions[statusCondPos].name, name);
      }
    }
  }

  public Actor2D GetActor2D() {
    int partyMemberPos = BattleController.instance.GetPartyMemberPosition(this);
    if(partyMemberPos == -1) {
      return null;
    }
    return TheaterController.instance.GetActorByIdInParty(partyMemberPos);
  }
}


[System.Serializable]
public class Relationship {
  public Person[] people;
  public int hearts = 0;
  public List<int> bondSkills;

  public Relationship() {
    hearts = 0;
    bondSkills = new List<int>();
  }


  public Person GetBoy() {
    return people[0];
  }

  public Person GetGirl() {
    return people[1];
  }
}
