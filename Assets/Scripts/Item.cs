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
  public string name;
  public string description;

  public int[] attribute_bonus;

  public ItemType type;
  public int price;
  public Sprite sprite;

  public Item(){
    attribute_bonus = new int[3];
  }

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
    if(id == -1){
      Debug.LogError("No item to add");
      return;
    }

    for(int i=0; i<items.Count; i++){
      if(items[i].id == id){
        items[i].amount += amount;
        SortItems();
        return;
      }
    }

    for(int i=0; i<items.Count; i++){
      if(items[i].id > id){
        items.Insert(i, new ItemListing(id, amount));
        SortItems();
        return;
      }
    }
    items.Add(new ItemListing(id, amount));
    SortItems();
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
        SortItems();
        return;
      }
    }
  }

  public void SortItems(){
  items = items.OrderBy(o => o.id).ToList();
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