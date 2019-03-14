﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {

  public static ItemDatabase instance;

  public List<Item> database;
  public List<int> itemsForSale;


  void Awake(){
    if(instance == null){
      instance = this;
    }
  }

  void Start(){
    InitializeItemDatabase();
  }

  void InitializeItemDatabase(){
    SpreadsheetData data;
    data = SpreadsheetReader.ReadSpreadsheet("Data\\items", 1);

    database = new List<Item>();
    itemsForSale = new List<int>();
    foreach(Dictionary<string, string> entry in data.data){
      Item newItem = new Item();
      newItem.id = int.Parse(entry["id"]);
      newItem.name = entry["name"];
      newItem.description = entry["description"];
      newItem.type = (entry["type"] == "c") ? ItemType.celestial : ItemType.mundane;
      newItem.price = int.Parse(entry["price"]);
      newItem.sprite = ResourcesManager.instance.itemSprites[int.Parse(entry["sprite_id"])];
      if(entry["sellable"] == "yes"){
        itemsForSale.Add(newItem.id);
      }
      database.Add(newItem);
    }
  }
}
