using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{

  [CommandAttribute(CommandString = "setup_date")]
  public class SetupDateCommand : VsnCommand {

    public override void Execute() {
      BattleController.instance.SetupBattleStart((int)args[0].GetNumberValue());
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
