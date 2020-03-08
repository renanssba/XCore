using System.Linq;
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


[System.Serializable]
public class Person {

  public string name;
  public bool isMale;
  public bool isHuman = true;

  public PersonState state = PersonState.unrevealed;

  public int[] attributes;

  public int maxSp;
  public int sp;

  public string favoriteMatter;
  public string mostHatedMatter;

  public Inventory inventory;
  public Inventory giftsReceived;

  public List<StatusCondition> statusConditions;

  public int id;
  public int faceId;


  public Person() {
    inventory = new Inventory();
    giftsReceived = new Inventory();
    statusConditions = new List<StatusCondition>();
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

  public Skill[] GetActiveSkills(int relationshipId) {
    List<Skill> skills = new List<Skill>();
    Relationship relationship = GlobalData.instance.relationships[relationshipId];

    if(id == 10) {
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

  public Skill[] GetAllSkills(int relationshipId) {
    Relationship relationship = GlobalData.instance.relationships[relationshipId];

    return relationship.GetAllSkillsByCharacter(isMale);
  }


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
      newCondition.duration = usedItem.duration + 1;
      newCondition.maxDurationShowable = usedItem.duration;
      //newCondition.name = usedItem.GetPrintableName();
      //newCondition.sprite = usedItem.sprite;
      ReceiveStatusCondition(newCondition);
    }
  }

  public bool ReceiveStatusCondition(StatusCondition newCondition) {
    int i = FindStatusCondition(newCondition);
    bool receivedNewStatus = false;
    Actor2D actor = TheaterController.instance.GetActorByPerson(this);

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
    actor.UpdateCharacterGraphics();
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
    Actor2D actor = TheaterController.instance.GetActorByPerson(this);

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
  public int id;
  public Person[] people;
  public int level = 0;
  public int exp = 0;
  public int heartLocksOpened = 0;

  public int bondPoints = 0;
  public int[] skillIds;
  public bool[] unlockedSkill;

  public static readonly int[] levelUpCosts = {20, 40, 100, 180, 300, 480, 750, 1150, 1700, 2500};
  public static readonly int[] skillRequisites = {-1, 0, 0, 0, 1, 1, 2, 2, 3, 3, -1, -1, -1};
  public const int skillTreeSize = 13;


  public Relationship() {
    level = 0;
    skillIds = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
    unlockedSkill = new bool[skillTreeSize];
    for(int i=0; i< unlockedSkill.Length; i++) {
      unlockedSkill[i] = false;
    }
    unlockedSkill[10] = true;
    unlockedSkill[12] = true;
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

  public void OpenHeartLock(int lockId) {
    heartLocksOpened = Mathf.Max(heartLocksOpened, lockId);
  }


  public Person GetBoy() {
    return people[0];
  }

  public Person GetGirl() {
    return people[1];
  }

  public int GetMaxHp() {
    return level * 8 + 26;
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
    if(isBoy) {
      AddSkillToList(skills, 1, true);
      AddSkillToList(skills, 4, true);
      AddSkillToList(skills, 5, true);
    } else {
      AddSkillToList(skills, 3, true);
      AddSkillToList(skills, 8, true);
      AddSkillToList(skills, 9, true);
    }
    return skills.ToArray();
  }

  public void AddSkillToList(List<Skill> list, int id, bool shouldBeActive) {
    Skill sk = BattleController.instance.GetSkillById(skillIds[id]);

    if(unlockedSkill[id]) {
      if(!shouldBeActive || sk.type == SkillType.active) {
        list.Add(sk);
      }
    }
  }

  public Skill[] GetAllSkillsByCharacter(bool isBoy) {
    List<Skill> skills = new List<Skill>();
    if(isBoy) {
      AddSkillToList(skills, 10, false);
      AddSkillToList(skills, 1, false);
      AddSkillToList(skills, 4, false);
      AddSkillToList(skills, 5, false);
    } else {
      AddSkillToList(skills, 12, false);
      AddSkillToList(skills, 3, false);
      AddSkillToList(skills, 8, false);
      AddSkillToList(skills, 9, false);
    }
    return skills.ToArray();
  }
}
