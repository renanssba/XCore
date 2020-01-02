using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_heart")]
  public class AddHeartCommand : VsnCommand {

    public override void Execute() {
      int hearts = 1;
      if(args.Length > 1) {
        hearts = (int)args[1].GetNumberValue();
      }
      GlobalData.instance.AddHeartsWithGirlId((int)args[0].GetNumberValue(), hearts);
      VsnController.instance.WaitForCustomInput();
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