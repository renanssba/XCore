using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType{
  mundane,
  celestial
}

[System.Serializable]
public class Item {

  public int id;
  public string name;
  public string description;

  public ItemType type;
  public int price;
  public Sprite sprite;

  public static string GetName(int id){
    if(id == -1){
      return "";
    }
    return GetItem(id).name;
  }

  public static string GetDescription(int id){
    if(id == -1){
      return "";
    }
    return GetItem(id).description;
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
  public List<ItemListing> items;

  public Inventory(){
    items = new List<ItemListing>();
  }

  public Inventory(List<int> ids){
    items = new List<ItemListing>();
    foreach(int id in ids){
      AddItem(id, 1);
    }
  }

  public Inventory(List<Item> ids){
    items = new List<ItemListing>();
    foreach(Item item in ids){
      AddItem(item.id, 1);
    }
  }

  public void AddItem(int id, int amount){
    for(int i=0; i<items.Count; i++){
      if(items[i].id == id){
        items[i].amount += amount;
        return;
      }
    }

    for(int i=0; i<items.Count; i++){
      if(items[i].id > id){
        items.Insert(i, new ItemListing(id, amount));
        return;
      }
    }
    items.Add(new ItemListing(id, amount));
  }

  public void ConsumeItem(int id, int amount){
    if(Item.GetType(id) == ItemType.celestial){
      return;
    }

    for(int i=0; i<items.Count; i++){
      if(items[i].id == id){
        items[i].amount -= amount;
        if(items[i].amount <= 0){
          items.RemoveAt(i);
        }
        return;
      }
    }
  }

  public bool HasItem(int id){
    foreach(ItemListing item in items){
      if(item.id == id){
        return true;
      }
    }
    return false;
  }

  public bool IsEmpty(){
    if(items.Count <= 0){
      return true;
    }
    return false;
  }
}

[System.Serializable]
public class ItemListing{
  public int id;
  public int amount;

  public ItemListing(int arg1, int arg2){
    id = arg1;
    amount = arg2;
  }
}