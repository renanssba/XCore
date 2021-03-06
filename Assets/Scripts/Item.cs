using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType{
  gift,
  battle,
  night,
  celestial,
  key
}

public enum ItemCategory{
  clothing,
  accessory,
  food
}

[System.Serializable]
public class Item {
  
  public string nameKey;
  public int id;

  public List<ItemType> types;
  public SkillRange range;

  public string[] healsConditionNames;
  public string[] givesConditionNames;
  public int duration;
  public int healSp;
  public int healHp;

  public int sells_in_store;

  public int price;
  public string spriteName;

  public string[] tags;


  public Item(){
    types = new List<ItemType>();
  }

  public static string GetKeyById(int id){
    if(id == -1){
      return "";
    }
    return GetItemById(id).nameKey;
  }

  public string GetPrintableName() {
    return Lean.Localization.LeanLocalization.GetTranslationText("item/name/" + nameKey);
  }

  public Sprite GetSprite() {
    return Resources.Load<Sprite>("Icons/" + spriteName);
  }

  public string GetPrintableDescription(){
    return Lean.Localization.LeanLocalization.GetTranslationText("item/description/" + nameKey);
  }

  public string GetBattleDescription(bool useBattleMarker = false) {
    string healDesc = "";
    string healSpDesc = "";
    string removeStatusDesc = "";
    string givesStatusDesc = "";
    string highlightMarkerStart = "";
    string highlightMarkerEnd = "";

    if(useBattleMarker) {
      highlightMarkerStart = "<color=yellow>";
      highlightMarkerEnd = "</color>";
    }

    if(healHp > 0) {
      VsnSaveSystem.SetVariable("heal_hp_value", healHp);
      healDesc += SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("item/description/heal_hp")) + " ";
    }

    if(healSp > 0) {
      VsnSaveSystem.SetVariable("heal_sp_value", healSp);
      healSpDesc += SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("item/description/heal_sp")) + " ";
    }

    if(healsConditionNames.Length > 0) {
      if(removeStatusDesc != "") {
        removeStatusDesc += ", ";
      }
      for(int i = 0; i < healsConditionNames.Length; i++) {
        removeStatusDesc += highlightMarkerStart + Lean.Localization.LeanLocalization.GetTranslationText("status_condition/name/"+healsConditionNames[i])+ highlightMarkerEnd;
        if(i == healsConditionNames.Length - 1) {
          continue;
        }
        //else if(i == healsConditionNames.Length-2) {
        //  removeStatusDesc += " e ";
        //}
        else {
          removeStatusDesc += ", ";
        }
      }
    }

    if(givesConditionNames.Length > 0) {
      for(int i = 0; i < givesConditionNames.Length; i++) {
        givesStatusDesc += highlightMarkerStart + Lean.Localization.LeanLocalization.GetTranslationText("status_condition/name/" + givesConditionNames[i]) + highlightMarkerEnd;
        if(i == givesConditionNames.Length - 1) {
          continue;
        }
        //else if(i == givesConditionNames.Length - 2) {
        //  givesStatusDesc += " e ";
        //}
        else {
          givesStatusDesc += ", ";
        }
      }
    }

    if(removeStatusDesc != "") {
      removeStatusDesc = Lean.Localization.LeanLocalization.GetTranslationText("item/description/heals_status") + " " + removeStatusDesc + ". ";
    }
    if(givesStatusDesc != "") {
      givesStatusDesc = Lean.Localization.LeanLocalization.GetTranslationText("item/description/gives_status") + " " + givesStatusDesc + ". ";
    }

    return (healDesc + healSpDesc + removeStatusDesc + givesStatusDesc).TrimEnd();
  }

  public static string GetPrintableNameById(int id) {
    if(id == -1) {
      return "";
    }
    return Lean.Localization.LeanLocalization.GetTranslationText("item/name/" + GetItemById(id).nameKey);
  }

  public bool HasType(ItemType type) {
    return types.Contains(type);
  }

  public bool HealsStatusCondition() {
    return healsConditionNames.Length > 0;
  }

  public bool GivesStatusCondition() {
    return givesConditionNames.Length > 0;
  }



  public static bool HasType(int id, ItemType type) {
    if(id == -1){
      return false;
    }
    return GetItemById(id).types.Contains(type);
  }

  public static Item GetItemById(int id){
    foreach(Item it in ItemDatabase.instance.database){
      if(it.id == id){
        return it;
      }
    }
    return null;
  }


  public static string UseItemEffect(int itemId, Pilot target){
    string itemUseMsg = "*Nada aconteceu.*";

    return itemUseMsg;
  }


  public bool HasTag(string tagToSearch) {
    return Utils.TagIsInArray(tagToSearch, tags);
  }
}

[System.Serializable]
public class Inventory{
  public Pilot owner;
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


  void EffectiveAddItem(int id, int amount) {
    if(id == -1) {
      Debug.LogError("No item to add");
      return;
    }

    for(int i = 0; i < itemListings.Count; i++) {
      if(itemListings[i].id == id) {
        itemListings[i].amount += amount;
        SortItems();
        return;
      }
    }

    itemListings.Add(new ItemListing() {
      id = id,
      amount = amount,
    });
    SortItems();
  }

  public void AddItem(int id, int amount){
    EffectiveAddItem(id, amount);
  }

  public void AddItem(string itemName, int amount) {
    AddItem(ItemDatabase.instance.GetItemByName(itemName).id, amount);
  }


  public void ConsumeItem(int id, int amount){
    //if(Item.GetType(id) == ItemType.battle){
    //  return;
    //}

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

  public int ItemCount(string name) {
    Item tocheck = ItemDatabase.instance.GetItemByName(name);
    if(tocheck == null) {
      return 0;
    }
    return ItemCount(tocheck.id);
  }

  public int CountItemListingsByType(ItemType type) {
    int count = 0;
    foreach(ItemListing il in itemListings) {
      if(Item.GetItemById(il.id).HasType(type)) {
        count++;
      }
    }
    return count;
  }

  public List<ItemListing> GetItemListingsByType(ItemType type) {
    List<ItemListing> items = new List<ItemListing>();

    foreach(ItemListing il in itemListings) {
      if(Item.GetItemById(il.id).HasType(type)) {
        items.Add(il);
      }
    }
    return items;
  }

  public bool IsEmpty(){
    if(itemListings.Count <= 0){
      return true;
    }
    return false;
  }
}


[System.Serializable]
public class InventorySaveStruct {
  public List<ItemListing> itemListings;
}


[System.Serializable]
public class ItemListing{
  public int id;
  public int amount;
}