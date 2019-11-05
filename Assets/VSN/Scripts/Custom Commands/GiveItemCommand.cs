using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "give_item")]
  public class GiveItemCommand : VsnCommand {

    public override void Execute() {
      Person personWhoReceives = GlobalData.instance.people[(int)args[0].GetNumberValue()];
      Item itemToLose = null;
      if(args[1].GetType() == typeof(VsnString)) {
        itemToLose = ItemDatabase.instance.GetItemByName(args[1].GetStringValue());
      } else {
        itemToLose = ItemDatabase.instance.GetItemById((int)args[1].GetNumberValue());
      }
      int amount = (int)args[2].GetNumberValue();

      ItemListing il = GlobalData.instance.CurrentBoy().inventory.GetItemListingById(itemToLose.id);

      Debug.Log("Item selected to give: " + itemToLose.nameKey);
      Debug.LogWarning("Removing " + amount + " of item " + itemToLose.id + "!");

      Inventory inventory = GlobalData.instance.CurrentBoy().inventory;

      personWhoReceives.inventory.AddItemWithOwnership(il.id, amount, il.ownerId);

      inventory.ConsumeItem(itemToLose.id, amount);

      VsnSaveSystem.SetVariable("item_id", itemToLose.id);
      VsnSaveSystem.SetVariable("item_name", Item.GetPrintableNameById(itemToLose.id));

      if(args.Length < 4 || args[3].GetBooleanValue() == true) {
        VsnArgument[] sayargs = new VsnArgument[2];
        sayargs[0] = new VsnString("char_name/none");
        sayargs[1] = new VsnString("item/give/message");
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