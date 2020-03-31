using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_custom_battle")]
  public class SetCustomBattleCommand : VsnCommand {

    public override void Execute() {
      BattleController.instance.SetCustomBattle((int)args[0].GetNumberValue());
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
