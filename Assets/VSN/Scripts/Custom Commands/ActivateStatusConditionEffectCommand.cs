using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "activate_status_condition_effect")]
  public class ActivateStatusConditionEffectCommand : VsnCommand {

    public override void Execute() {
      Battler battler = BattleController.instance.GetBattlerByTargetId((int)args[0].GetNumberValue());
      if(battler == null) {
        Debug.LogWarning("No party member found in that position.");
        return;
      }
      battler.ActivateStatusConditionEffect((int)args[1].GetNumberValue());
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}