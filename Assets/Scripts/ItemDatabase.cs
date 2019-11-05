using System.Collections;
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
    Debug.LogWarning("START ITEMDATABASE");
    InitializeItemDatabase();
  }


  void InitializeItemDatabase(){
    SpreadsheetData data;
    data = SpreadsheetReader.ReadSpreadsheet("Data\\items database", 1);

    database = new List<Item>();
    itemsForSale = new List<int>();
    foreach(Dictionary<string, string> entry in data.data){
      //Debug.LogWarning("Importing Item");

      Item newItem = new Item();
      newItem.id = int.Parse(entry["id"]);
      newItem.nameKey = entry["name"];
      newItem.descriptionKey = entry["description"];
      newItem.type = (entry["type"] == "c") ? ItemType.celestial : ItemType.mundane;
      newItem.attribute_bonus[0] = int.Parse(entry["guts_bonus"]);
      newItem.attribute_bonus[1] = int.Parse(entry["intelligence_bonus"]);
      newItem.attribute_bonus[2] = int.Parse(entry["charisma_bonus"]);

      newItem.price = int.Parse(entry["price"]);
      //newItem.sprite = ResourcesManager.instance.itemSprites[int.Parse(entry["sprite_id"])];
      newItem.sprite = Resources.Load<Sprite>("Cards/" + entry["sprite"]);
      if(entry["sells_in_store"] == "yes"){
        itemsForSale.Add(newItem.id);
      }
      database.Add(newItem);
    }
  }


  public Item GetItemById(int id) {
    foreach(Item it in database) {
      if(it.id == id) {
        return it;
      }
    }
    return null;
  }

  public Item GetItemByName(string name) {
    foreach(Item it in database) {
      if(it.nameKey == name) {
        return it;
      }
    }
    return null;
  }
}
