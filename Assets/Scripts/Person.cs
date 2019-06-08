﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Personality{
  heroico,
  racional,
  emotivo
}

public enum Trait{
  mirror,
  adaptative,
  simpleton,
  unpredictable
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
  public Personality personality;
  public List<Trait> traits;
  public Item[] equips;

  public RandomTastes favoriteMatter;
  public RandomTastes mostHatedMatter;

  public int id;
  public int faceId;


  public void Initialize(int personId) {
    List<int> attValues = new List<int>();
    switch (Random.Range(0, 2)) {
      case 0:
        attValues.Add(3);
        attValues.Add(3);
        attValues.Add(2);
        break;
      case 1:
        attValues.Add(4);
        attValues.Add(3);
        attValues.Add(1);
        break;
    }
    attValues = attValues.OrderBy(x => Random.value).ToList();

    attributes = new int[3];
    for(int i=0; i<3; i++){
      attributes[i] = attValues[i];
    }

    state = PersonState.unrevealed;

    favoriteMatter = (RandomTastes)Random.Range(0, (int)RandomTastes.count - 1);
    mostHatedMatter = (RandomTastes)Random.Range(0, (int)RandomTastes.count - 1);

    //if (isMale) {
    //  faceId = Random.Range(0, 5);
    //} else {
    //  faceId = 5 + Random.Range(0, 5);
    //}
    id = personId;
    faceId = personId;
    personality = (Personality)Random.Range(0, 3);
    equips = new Item[3];
    for(int i=0; i<3; i++){
      equips[i] = null;
    }
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
    int sum = attributes[att];
    for(int i=0; i<equips.Length; i++){
      if(equips[i] != null){
        sum += equips[i].attribute_bonus[att];
      }
    }
    return Mathf.Max(sum, 0);
  }
  
  public void EquipItemInSlot(int slotId, Item item){
    VsnAudioManager.instance.PlaySfx("inventory_equip");
    equips[slotId] = item;
    GlobalData.instance.inventory.ConsumeItem(equips[slotId].id, 1);
    GameController.instance.UpdateUI();
  }

  public void UnequipItemInSlot(int slotId) {
    if (equips[slotId] != null) {
      VsnAudioManager.instance.PlaySfx("inventory_equip");
      Debug.LogWarning("Unequiping item: " + equips[slotId].id +", " + equips[slotId].name);
      GlobalData.instance.inventory.AddItem(equips[slotId].id, 1);
      equips[slotId] = null;
      GameController.instance.UpdateUI();
    }
  }

  public int EquipsCount(){
    int count = 0;
    for (int i = 0; i < equips.Length; i++) {
      if (equips[i] != null) {
        count++;
      }
    }
    return count;
  }

  public char GenderedVowel(){
    if(isMale){
      return 'o';
    }else{
      return 'a';
    }
  }

  public string PersonalityString(){
    if (VsnSaveSystem.GetStringVariable("language") == "pt_br") {
      return PersonalityStringPtBr();
    } else {
      return PersonalityStringEng();
    }
  }

  public string PersonalityStringPtBr() {
    switch (personality) {
      case Personality.heroico:
        return "<color=#B27535><sprite=\"Attributes\" index=0 tint>Heróic" + GenderedVowel() + "</color>";
      case Personality.racional:
        return "<color=#248BCF><sprite=\"Attributes\" index=1 tint>Racional</color>";
      case Personality.emotivo:
        return "<color=#A80218><sprite=\"Attributes\" index=2 tint>Emotiv" + GenderedVowel() + "</color>";
    }
    return "";
  }

  public string PersonalityStringEng(){
    switch (personality) {
      case Personality.heroico:
        return "<color=#B27535><sprite=\"Attributes\" index=0 tint>Heroic</color>";
      case Personality.racional:
        return "<color=#248BCF><sprite=\"Attributes\" index=1 tint>Rational</color>";
      case Personality.emotivo:
        return "<color=#A80218><sprite=\"Attributes\" index=2 tint>Emotional</color>";
    }
    return "";
  }
}
