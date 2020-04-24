using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "end_turn")]
  public class EndTurnCommand : VsnCommand {

    public override void Execute() {
      Debug.LogWarning("END TURN CALLED");
      if(args.Length == 0) {
        BattleController.instance.EndTurn();
      } else {
        Battler target = BattleController.instance.GetBattlerByTargetId((SkillTarget)(int)args[0].GetNumberValue());
        target.EndTurn();
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