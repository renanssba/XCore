using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_heart")]
  public class AddHeartCommand : VsnCommand {

    public override void Execute() {
      GlobalData.instance.AddHeart((int)args[0].GetNumberValue());
      if(args.Length > 1) {
        GlobalData.instance.AddBondSkill((int)args[0].GetNumberValue(), args[1].GetStringValue());
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
    }
  }
}