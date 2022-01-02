using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "check_buy_all_achievement")]
  public class CheckBuyAllAchievementCommand : VsnCommand {

    public int[] itemsToBuy = {0,1,4,5,6,7,8,9,10,13};

    public override void Execute() {
      int itemId = VsnSaveSystem.GetIntVariable("item_id");
      List<int> itemsBought = BoughtItems();

      if(!itemsBought.Contains(itemId)) {
        itemsBought.Add(itemId);
      }

      if(HasBoughtEverything(itemsBought)) {
        AchievementsController.ReceiveAchievement("SHOPPING_ALL_ITEMS");
      }

      UpdateBoughtItems(itemsBought);
    }


    public List<int> BoughtItems() {
      string itemsBoughtString = VsnSaveSystem.GetStringVariable("itemsBought", "");
      string[] itemsBoughtStrings = itemsBoughtString.Split(';');
      List<int> itemsBought = new List<int>();

      if(itemsBoughtString == "") {
        return itemsBought;
      }

      foreach(string item in itemsBoughtStrings) {
        itemsBought.Add(int.Parse(item));
      }

      return itemsBought;
    }

    public bool HasBoughtEverything(List<int> itemsBought) {
      for(int i = 0; i < itemsToBuy.Length; i++) {
        if(!itemsBought.Contains(itemsToBuy[i])) {
          return false;
        }
      }
      return true;
    }

    public void UpdateBoughtItems(List<int> itemsBought) {
      string list = "";
      for(int i=0; i<itemsBought.Count; i++) {
        list += itemsBought[i];
        if(i < itemsBought.Count-1) {
          list += ";";
        } 
      }

      Debug.LogWarning("check_buy_all_achievement. LIST: " + list);

      VsnSaveSystem.SetVariable("itemsBought", list);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
