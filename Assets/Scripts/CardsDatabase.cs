﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DateCardType {
  actionCard,
  itemCard,
  characterSkillCard,
  bondSkillCard
}

[System.Serializable]
public class DateCardContent {
  public int id;

  public string name;
  public string description;
  public Sprite sprite;
  public DateCardType type;

  public Attributes attribute;
  public float multiplier;

  public Skill skill;
  public int duration;
  public int cost;
}


public class CardsDatabase : MonoBehaviour {

  public static CardsDatabase instance;

  public List<DateCardContent> database;


  void Awake() {
    if(instance == null) {
      instance = this;
    }
  }

  void Start() {
    Debug.LogWarning("START ITEMDATABASE");
    InitializeCardsDatabase();
  }

  void InitializeCardsDatabase() {
    SpreadsheetData data;
    data = SpreadsheetReader.ReadSpreadsheet("Data\\cards", 1);

    database = new List<DateCardContent>();
    foreach(Dictionary<string, string> entry in data.data) {
      //Debug.LogWarning("Importing Card: ");

      DateCardContent newCard = new DateCardContent();

      newCard.id = int.Parse(entry["id"]);

      newCard.name = entry["name"];
      newCard.description = entry["description"];
      newCard.type = GetDateCardTypeByString(entry["type"]);

      newCard.attribute = Utils.GetAttributeByString(entry["attribute"]);
      if(!string.IsNullOrEmpty(entry["multiplier"]) ) {
        newCard.multiplier = float.Parse(entry["multiplier"]);
      }
      if(!string.IsNullOrEmpty(entry["duration"])) {
        newCard.duration = int.Parse(entry["duration"]);
      }
      if(!string.IsNullOrEmpty(entry["cost"])) {
        newCard.cost = int.Parse(entry["cost"]);
      }

      if(newCard.type == DateCardType.actionCard) {
        newCard.sprite = ResourcesManager.instance.attributeSprites[(int)newCard.attribute];
      } else {
        newCard.sprite = Resources.Load<Sprite>("Cards/"+entry["sprite"]);
      }
      newCard.skill = GetSkillByString(entry["skill"]);

      database.Add(newCard);
    }
  }

  public DateCardType GetDateCardTypeByString(string name) {
    switch(name) {
      case "action":
        return DateCardType.actionCard;
      case "character_skill":
        return DateCardType.characterSkillCard;
      case "bond_skill":
        return DateCardType.bondSkillCard;
      case "item":
        return DateCardType.itemCard;
    }
    return DateCardType.actionCard;
  }

  public Skill GetSkillByString(string name) {
    for(int i=0; i<=(int)Skill.none; i++) {
      if( ((Skill)i).ToString() == name ) {
        return (Skill)i;
      }
    }
    return Skill.none;
  }
  
  public DateCardContent GetCardById(int id) {
    foreach(DateCardContent card in database) {
      if(card.id == id) {
        return card;
      }
    }
    return null;
  }

  public DateCardContent GetCardByName(string name) {
    foreach(DateCardContent card in database) {
      if(card.name == name) {
        return card;
      }
    }
    return null;
  }
}
