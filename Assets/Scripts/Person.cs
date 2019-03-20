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


[System.Serializable]
public class Person {

  public string name;
  public bool isMale;

  public int[] attributes;
  public Personality personality;
  public List<Trait> traits;
  public Item[] equips;

  public int faceId;


  public void Initialize() {
    List<int> attValues = new List<int>();
    switch (Random.Range(0, 3)) {
      case 0:
        attValues.Add(3);
        attValues.Add(2);
        attValues.Add(1);
        break;
      case 1:
        attValues.Add(2);
        attValues.Add(2);
        attValues.Add(2);
        break;
      case 2:
        attValues.Add(4);
        attValues.Add(1);
        attValues.Add(1);
        break;
    }
    attValues = attValues.OrderBy(x => Random.value).ToList();

    attributes = new int[3];
    for(int i=0; i<3; i++){
      attributes[i] = attValues[i];
    }
    
    if (isMale) {
      faceId = Random.Range(0, 5);
    } else {
      faceId = 5 + Random.Range(0, 5);
    }
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
    return sum;
  }
  
  public void EquipItemInSlot(int slotId, Item item){
    equips[slotId] = item;
    GlobalData.instance.inventory.ConsumeItem(equips[slotId].id, 1);
    GameController.instance.UpdateUI();
  }

  public void UnequipItemInSlot(int slotId) {
    if (equips[slotId] != null) {
      Debug.LogWarning("Unequiping item: " + equips[slotId].id +", " + equips[slotId].name);
      GlobalData.instance.inventory.AddItem(equips[slotId].id, 1);
      equips[slotId] = null;
      GameController.instance.UpdateUI();
    }
  }
}
