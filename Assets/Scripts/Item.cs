using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType{
  mundane,
  celestial
}

public enum ItemCategory{
  clothing,
  accessory,
  food
}

[System.Serializable]
public class Item {

  public int id;
  public string nameKey;
  public string descriptionKey;

  public int[] attribute_bonus;

  public ItemType type;
  public int price;
  public Sprite sprite;

  public Item(){
    attribute_bonus = new int[3];
  }

  public static string GetKeyById(int id){
    if(id == -1){
      return "";
    }
    return GetItem(id).nameKey;
  }

  public string GetPrintableName() {
    return Lean.Localization.LeanLocalization.GetTranslationText("item/name/" + GetItem(id).nameKey);
  }

  public string GetPrintableDescription(){
    return Lean.Localization.LeanLocalization.GetTranslationText("item/description/" + descriptionKey);
  }

  public static string GetPrintableNameById(int id) {
    if(id == -1) {
      return "";
    }
    return Lean.Localization.LeanLocalization.GetTranslationText("item/name/" + GetItem(id).nameKey);
  }

  public static ItemType GetType(int id){
    if(id == -1){
      return ItemType.mundane;
    }
    return GetItem(id).type;
  }

  public static Item GetItem(int id){
    foreach(Item it in ItemDatabase.instance.database){
      if(it.id == id){
        return it;
      }
    }
    return null;
  }


  public static string UseItemEffect(int itemId, Person target){
    string itemUseMsg = "*Nada aconteceu.*";

    return itemUseMsg;
  }
}

[System.Serializable]
public class Inventory{
  public Person owner;
  public List<ItemListing> itemListings;

  public Inventory(){
    itemListings = new List<ItemListing>();
    itemListings.Clear();
  }

  public Inventory(List<int> ids){
    itemListings = new List<ItemListing>();
    foreach(int id in ids){
      AddItem(id, 1);
    }
  }

  public Inventory(List<Item> ids){
    itemListings = new List<ItemListing>();
    foreach(Item item in ids){
      AddItem(item.id, 1);
    }
  }


  void EffectiveAddItem(int id, int amount, int ownerId) {
    if(id == -1) {
      Debug.LogError("No item to add");
      return;
    }

    for(int i = 0; i < itemListings.Count; i++) {
      if(itemListings[i].id == id && itemListings[i].ownerId == ownerId) {
        itemListings[i].amount += amount;
        SortItems();
        return;
      }
    }

    itemListings.Add(new ItemListing() {
      id = id,
      amount = amount,
      ownerId = ownerId
    });
    SortItems();
  }

  public void AddItem(int id, int amount){
    EffectiveAddItem(id, amount, -1);
  }

  public void AddItem(string itemName, int amount) {
    AddItem(ItemDatabase.instance.GetItemByName(itemName).id, amount);
  }

  public void AddItemWithOwnership(string itemName, int amount, int ownerId) {
    EffectiveAddItem(ItemDatabase.instance.GetItemByName(itemName).id, amount, ownerId);
  }

  public void AddItemWithOwnership(int id, int amount, int ownerId) {
    EffectiveAddItem(id, amount, ownerId);
  }


  public void ConsumeItem(int id, int amount){
    if(Item.GetType(id) == ItemType.celestial){
      return;
    }

    for(int i=0; i<itemListings.Count; i++){
      if(itemListings[i].id == id){
        itemListings[i].amount -= amount;
        if(itemListings[i].amount <= 0){
          itemListings.RemoveAt(i);
        }
        SortItems();
        return;
      }
    }
  }

  public ItemListing GetItemListingById(int id) {
    for(int i = 0; i < itemListings.Count; i++) {
      if(itemListings[i].id == id) {
        return itemListings[i];
      }
    }
    return null;
  }

  public void SortItems(){
    itemListings = itemListings.OrderBy(o => o.id).ToList();
  }

  public bool HasItem(int id){
    foreach(ItemListing item in itemListings){
      if(item.id == id){
        return true;
      }
    }
    return false;
  }

  public int ItemCount(int id) {
    foreach(ItemListing item in itemListings) {
      if(item.id == id) {
        return item.amount;
      }
    }
    return 0;
  }

  public int ItemCountFromOwner(int id, int ownerId) {
    foreach(ItemListing item in itemListings) {
      if(item.id == id && item.ownerId == ownerId) {
        return item.amount;
      }
    }
    return 0;
  }

  public int ItemCount(string name) {
    Item tocheck = ItemDatabase.instance.GetItemByName(name);
    if(tocheck == null) {
      return 0;
    }
    return ItemCount(tocheck.id);
  }

  public bool IsEmpty(){
    if(itemListings.Count <= 0){
      return true;
    }
    return false;
  }
}


[System.Serializable]
public class ItemListing{
  public int id;
  public int amount;
  public int ownerId;

  public ItemListing(){
    ownerId = -1;
  }
}