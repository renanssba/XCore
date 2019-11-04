using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Personality{
  heroico,
  racional,
  emotivo
}

public enum SkillType {
  character,
  couple
}

public enum Skill{
  sensor,
  raiseAttribute,
  flee,
  gluttony,
  bondSkill,
  none
}

public enum Attributes{
  guts = 0,
  intelligence = 1,
  charisma = 2
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

  public PersonState state = PersonState.unrevealed;

  public int[] attributes;
  public int[] attributeBonuses;

  public Personality personality;
  public int skillId = -1;
  public Item equipment = null;

  public string favoriteMatter;
  public string mostHatedMatter;

  public int id;
  public int faceId;


  public Person() {
    attributeBonuses = new int[] { 0, 0, 0 };
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
    attributeBonuses = new int[] {0, 0, 0};

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
    personality = (Personality)Random.Range(0, 3);
  }

  public Attributes AttributetoUse(){
    switch(personality){
      case Personality.heroico:
        return Attributes.guts;
      case Personality.racional:
        return Attributes.intelligence;
      case Personality.emotivo:
        return Attributes.charisma;
    }
    return Attributes.guts;
  }

  public int AttributeValue(int att){
    if(attributes == null) {
      return 0;
    }
    int sum = attributes[att];
    if(equipment != null){
      sum += equipment.attribute_bonus[att];
    }
    if(attributeBonuses != null) {
      sum += attributeBonuses[att];
    }
    return Mathf.Max(sum, 0);
  }
  
  public void EquipItemInSlot(int slotId, Item item){
    VsnAudioManager.instance.PlaySfx("inventory_equip");
    equipment = item;
    GlobalData.instance.inventory.ConsumeItem(equipment.id, 1);
    GameController.instance.UpdateUI();
  }

  public void UnequipItemInSlot(int slotId) {
    if (equipment != null) {
      VsnAudioManager.instance.PlaySfx("inventory_equip");
      Debug.LogWarning("Unequiping item: " + equipment.id +", " + equipment.name);
      GlobalData.instance.inventory.AddItem(equipment.id, 1);
      equipment = null;
      GameController.instance.UpdateUI();
    }
  }

  public int EquipsCount(){
    if (equipment != null) {
      return 1;
    }
    return 0;
  }

  public char GenderedVowel(){
    if(isMale){
      return 'o';
    }else{
      return 'a';
    }
  }

  public void GetAttributeBonus(Attributes attr, int bonus) {
    attributeBonuses[(int)attr] += bonus;
  }

  public void EndTurn() {
    /// remove attributes bonus
    for(int i=0; i<3; i++) {
      attributeBonuses[i] = 0;
    }
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
