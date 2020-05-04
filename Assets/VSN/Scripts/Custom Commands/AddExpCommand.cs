using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_exp")]
  public class AddExpCommand : VsnCommand {

    public override void Execute() {
      Relationship relation = GlobalData.instance.GetCurrentRelationship();
      bool useMultiplier = args[0].GetBooleanValue();
      int expToGet = (int)args[1].GetNumberValue();

      Debug.LogWarning("Add exp: "+expToGet);

      if(args.Length >= 3) {
        relation = GlobalData.instance.relationships[(int)args[2].GetNumberValue()];
      }
      GlobalData.instance.observedPeople = new Person[2];
      GlobalData.instance.observedPeople[0] = relation.GetBoy();
      GlobalData.instance.observedPeople[1] = relation.GetGirl();

      if(useMultiplier) {
        switch(relation.heartLocksOpened) {
          case 0:
            break;
          case 1:
            expToGet *= 2;
            break;
          case 2:
            expToGet *= 4;
            break;
        }
      }
      GlobalData.instance.AddExpForRelationship(relation, expToGet);
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