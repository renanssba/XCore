using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "take_item")]
  public class TakeItemCommand : VsnCommand {

    public override void Execute() {
      Person personWhoLoses = GlobalData.instance.people[(int)args[0].GetNumberValue()];
      Item itemToAdd = null;
      if(args[1].GetType() == typeof(VsnString)) {
        itemToAdd = ItemDatabase.instance.GetItemByName(args[1].GetStringValue());
      } else {
        itemToAdd = ItemDatabase.instance.GetItemById((int)args[1].GetNumberValue());
      }
      int amount = (int)args[2].GetNumberValue();

      ItemListing il = personWhoLoses.inventory.GetItemListingById(itemToAdd.id);

      Debug.Log("Item selected to add: " + itemToAdd.nameKey);
      Debug.LogWarning("Adding " + amount + " to item " + itemToAdd.id + "!");

      Inventory inventory = GlobalData.instance.CurrentBoy().inventory;

      if(amount >= 0) {
        inventory.AddItemWithOwnership(il.id, amount, il.ownerId);
      } else {
        inventory.ConsumeItem(itemToAdd.id, -amount);
      }

      personWhoLoses.inventory.ConsumeItem(itemToAdd.id, amount);

      VsnSaveSystem.SetVariable("item_id", itemToAdd.id);
      VsnSaveSystem.SetVariable("item_name", Item.GetPrintableNameById(itemToAdd.id));
      VsnSaveSystem.SetVariable("item_amount", amount);

      if(args.Length < 4 || args[3].GetBooleanValue() == true) {
        VsnArgument[] sayargs = new VsnArgument[2];
        sayargs[0] = new VsnString("char_name/none");
        if(amount == 1) {
          sayargs[1] = new VsnString("inventory/acquire_single_item");
        } else {
          sayargs[1] = new VsnString("inventory/acquire_multiple_items");
        }
        SayCommand.StaticExecute(sayargs);
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg,
        VsnArgType.numberArg,
        VsnArgType.booleanArg
      });
    }
  }
}