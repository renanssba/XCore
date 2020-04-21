using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_level")]
  public class AddLevelCommand : VsnCommand {

    public override void Execute() {
      Relationship relation = GlobalData.instance.GetCurrentRelationship();
      int levelToGet = (int)args[0].GetNumberValue();

      if(args.Length > 1) {
        relation = GlobalData.instance.relationships[(int)args[1].GetNumberValue()];
      }
      GlobalData.instance.observedPeople = new Person[2];
      GlobalData.instance.observedPeople[0] = relation.GetBoy();
      GlobalData.instance.observedPeople[1] = relation.GetGirl();

      relation.level += levelToGet;
      UIController.instance.UpdateUI();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}