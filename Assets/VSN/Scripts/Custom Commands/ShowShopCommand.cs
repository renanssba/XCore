using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "show_shop")]
  public class ShowShopCommand : VsnCommand {

    public override void Execute() {
      ItemSelectorScreen itemScreen = GameController.instance.itemSelectorScreen;

      if(args[0].GetBooleanValue()) {
        itemScreen.Initialize(ItemInteractionType.store, new Inventory(ItemDatabase.instance.itemsForSale));
        itemScreen.screenTransition.OpenMenuScreen();
        VsnController.instance.WaitForCustomInput();
      } else {
        itemScreen.gameObject.SetActive(false);
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg
      });
    }
  }
}