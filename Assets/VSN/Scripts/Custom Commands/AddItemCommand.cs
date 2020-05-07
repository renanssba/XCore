using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_item")]
  public class AddItemCommand : VsnCommand {

    public override void Execute() {
      Item itemToAdd = null;

      if(args[0].GetType() == typeof(VsnString)) {
        itemToAdd = ItemDatabase.instance.GetItemByName(args[0].GetStringValue());
      } else {
        itemToAdd = ItemDatabase.instance.GetItemById((int)args[0].GetNumberValue());
      }
      int amount = (int)args[1].GetNumberValue();


      Debug.LogWarning("Adding " + amount + " to item " + itemToAdd.id + "!");

      Inventory inventory = GlobalData.instance.CurrentBoy().inventory;

      if(amount >= 0) {
        inventory.AddItem(itemToAdd.id, amount);
      } else {
        inventory.ConsumeItem(itemToAdd.id, -amount);
      }

      VsnSaveSystem.SetVariable("item_id", itemToAdd.id);
      VsnSaveSystem.SetVariable("item_name", Item.GetPrintableNameById(itemToAdd.id));
      VsnSaveSystem.SetVariable("item_amount", amount);

      if(args.Length < 3 || args[2].GetBooleanValue() == true) {
        VsnArgument[] sayargs = new VsnArgument[2];
        sayargs[0] = new VsnString("char_name/none");
        if(amount == 1) {
          sayargs[1] = new VsnString("shop/say_5");
        } else {
          sayargs[1] = new VsnString("shop/say_7");
        }        
        SayCommand.StaticExecute(sayargs);
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg,
        VsnArgType.booleanArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg,
        VsnArgType.booleanArg
      });
    }
  }
}