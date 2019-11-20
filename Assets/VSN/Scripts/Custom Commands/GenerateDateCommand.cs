using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "generate_date")]
  public class GenerateDateCommand : VsnCommand {

    public override void Execute() {
      if(args.Length >= 1) {
        BattleController.instance.GenerateDate((int)args[0].GetNumberValue());
      } else {
        BattleController.instance.GenerateDate();
      }
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
