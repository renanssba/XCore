using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_exp")]
  public class AddExpCommand : VsnCommand {

    public override void Execute() {
      bool useMultiplier = args[0].GetBooleanValue();
      int expToGet = (int)args[1].GetNumberValue();

      Debug.LogWarning("Add exp: "+expToGet);

      if(args.Length >= 3) {
        GlobalData.instance.currentRelationshipId = (int)args[2].GetNumberValue();
      }

      //GlobalData.instance.AddExpForRelationship(GlobalData.instance.GetCurrentRelationship(), expToGet*10);
      VsnController.instance.WaitForCustomInput();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg,
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg,
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}