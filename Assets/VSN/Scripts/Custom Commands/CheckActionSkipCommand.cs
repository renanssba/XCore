using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "check_action_skip")]
  public class CheckActionSkipCommand : VsnCommand {

    public override void Execute() {
      if(BattleController.instance.CurrentBattler.GetType() == typeof(Enemy)) {
        return;
      }

      // if action is idle
      if(BattleController.instance.selectedActionType[BattleController.instance.CurrentBattlerId] == TurnActionType.idle) {
        ContinueCommand.StaticExecute();
        return;
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}