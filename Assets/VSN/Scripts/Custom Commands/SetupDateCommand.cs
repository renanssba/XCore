using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{

  [CommandAttribute(CommandString = "setup_date")]
  public class SetupDateCommand : VsnCommand {

    public override void Execute() {
      BattleController.instance.SetupBattleStart();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
