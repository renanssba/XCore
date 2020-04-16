using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "end_date")]
  public class EndDateCommand : VsnCommand {

    public override void Execute() {
      BattleController.instance.EndDate();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
