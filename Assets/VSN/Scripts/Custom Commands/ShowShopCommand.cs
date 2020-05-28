using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "show_shop")]
  public class ShowShopCommand : VsnCommand {

    public override void Execute() {
      ItemSelectorScreen itemScreen = GameController.instance.itemSelectorScreen;

      if(args[0].GetBooleanValue()) {
        switch (args[1].GetStringValue()) {
          case "buy":
            itemScreen.OpenBuyStore();// (ItemInteractionType.store_buy, new Inventory(ItemDatabase.instance.itemsForSale));
            break;
          case "sell":
            itemScreen.OpenSellStore();// Initialize(ItemInteractionType.store_sell, GlobalData.instance.inventory);
            break;
        }
        VsnController.instance.WaitForCustomInput();
      } else {
        itemScreen.screenTransition.CloseMenuScreen();
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg,
        VsnArgType.stringArg
      });
    }
  }
}