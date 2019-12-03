using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "end_turn")]
  public class EndTurnCommand : VsnCommand {

    public override void Execute() {
      Debug.LogWarning("END TURN CALLED");
      BattleController.instance.EndTurn();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}