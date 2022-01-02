using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "check_sell_wrong_item")]
  public class CheckSellWrongItemAchievementCommand : VsnCommand {

    public override void Execute() {
      int soldItemId = VsnSaveSystem.GetIntVariable("item_id", -1);
      Inventory inventory = GlobalData.instance.people[0].inventory;

      foreach(ItemListing itemListings in inventory.itemListings) {
        if(itemListings.id == soldItemId) {
          if(itemListings.ownerId != -1) {
            AchievementsController.ReceiveAchievement("SELLING_USED_ITEM");
          }
          return;
        }
      }
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
