using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_item")]
  public class AddItemCommand : VsnCommand {

    public override void Execute() {
      int itemId = (int)args[0].GetNumberValue();
      int amount = (int)args[1].GetNumberValue();

      Debug.LogWarning("Adding " + amount + " to item " + itemId + "!");

      Inventory inventory = GlobalData.instance.inventory;

      if (amount >= 0) {
        inventory.AddItem(itemId, amount);
      } else {
        inventory.ConsumeItem(itemId, -amount);
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}