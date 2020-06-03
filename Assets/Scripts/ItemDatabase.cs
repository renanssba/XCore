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
    itemsForSale = new List<int>();
  }

  void Start(){
    Debug.LogWarning("START ITEMDATABASE");
    LoadAllItems();
  }


  void LoadAllItems(){
    SpreadsheetData data;
    data = SpreadsheetReader.ReadSpreadsheet("Data\\items", 1);

    database = new List<Item>();
    foreach(Dictionary<string, string> entry in data.data){
      //Debug.LogWarning("Importing Item");

      Item newItem = new Item();
      newItem.id = int.Parse(entry["id"]);
      newItem.nameKey = entry["name"];
      newItem.types = GetItemTypes(entry["types"]);
      newItem.range = BattleController.GetActionRangeByString(entry["range"]);
      newItem.duration = int.Parse(entry["duration"]);
      newItem.healsConditionNames = GetStatusConditionNamesByString(entry["heals status conditions"]);
      newItem.givesConditionNames = GetStatusConditionNamesByString(entry["gives status conditions"]);
      newItem.healHp = int.Parse(entry["heal hp"]);
      newItem.healSp = int.Parse(entry["heal sp"]);

      newItem.price = int.Parse(entry["price"]);
      //newItem.sprite = ResourcesManager.instance.itemSprites[int.Parse(entry["sprite_id"])];
      newItem.sprite = Resources.Load<Sprite>("Icons/" + entry["sprite"]);
      newItem.sells_in_store = int.Parse(entry["sells_in_store"]);
      newItem.tags = Utils.SeparateTags(entry["tags"]);
      database.Add(newItem);
    }
  }

  public static string[] GetStatusConditionNamesByString(string effects) {
    if(string.IsNullOrEmpty(effects)) {
      return new string[0];
    }
    string[] parts = effects.Split(',');
    for(int i=0; i<parts.Length; i++) {
      parts[i] = parts[i].Trim();
    }
    return parts;
  }

  public List<ItemType> GetItemTypes(string typesString) {
    List<ItemType> types = new List<ItemType>();
    string[] parts = typesString.Split(',');

    foreach(string part in parts) {
      switch(part.Trim()) {
        case "battle":
          types.Add(ItemType.battle);
          break;
        case "gift":
          types.Add(ItemType.gift);
          break;
        case "key":
          types.Add(ItemType.key);
          break;
        case "night":
          types.Add(ItemType.night);
          break;
      }
    }
    return types;
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

  public void UpdateItemsForSale(int shopLevel) {
    Debug.LogWarning("Updating Items for Sale. Level: "+shopLevel);
    itemsForSale = new List<int>();
    foreach(Item item in database) {
      if(item.sells_in_store > 0 && item.sells_in_store <= shopLevel) {
        itemsForSale.Add(item.id);
      }
    }
  }
}
