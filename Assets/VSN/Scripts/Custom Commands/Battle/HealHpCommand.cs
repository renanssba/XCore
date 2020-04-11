using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "heal_hp")]
  public class HealHpCommand : VsnCommand {

    public override void Execute() {
      Battler target = BattleController.instance.GetBattlerByString(args[0].GetStringValue());
      target.HealHP((int)args[1].GetNumberValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}
