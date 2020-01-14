using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "activate_status_condition_effect")]
  public class ActivateStatusConditionEffectCommand : VsnCommand {

    public override void Execute() {
      Person p = BattleController.instance.partyMembers[(int)args[0].GetNumberValue()];
      if(p == null) {
        Debug.LogWarning("No party member found in that position.");
        return;
      }
      p.ActivateStatusConditionEffect((int)args[1].GetNumberValue());
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}