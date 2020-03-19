using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_bond_points")]
  public class AddBondPointsCommand : VsnCommand {

    public override void Execute() {
      StaticExecute(args);
    }

    public static void StaticExecute(VsnArgument[] args) {
      GlobalData.instance.GetCurrentRelationship().bondPoints += (int)args[0].GetNumberValue();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}