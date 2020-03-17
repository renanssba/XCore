using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{

  [CommandAttribute(CommandString = "setup_date")]
  public class SetupDateCommand : VsnCommand {

    public override void Execute() {
      StaticExecute();
    }

    public static void StaticExecute() {
      BattleController.instance.SetupBattleStart(VsnSaveSystem.GetIntVariable("dateId"));
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
