using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "challenge_result")]
  public class ChallengeResultCommand : VsnCommand {

    public override void Execute() {
      int value = VsnSaveSystem.GetIntVariable("currentDateEvent");
      if(args[0].GetBooleanValue()) {
        VsnSaveSystem.SetVariable("date_event_result_" + (value - 1), 1);
      } else {
        VsnSaveSystem.SetVariable("date_event_result_" + (value - 1), 2);
      }
      BattleController.instance.ShowChallengeResult(args[0].GetBooleanValue());
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg
      });
    }
  }
}