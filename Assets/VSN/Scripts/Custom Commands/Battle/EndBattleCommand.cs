using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "end_battle")]
  public class EndBattleCommand : VsnCommand {

    public override void Execute() {
      BattleController.instance.EndBattle();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
