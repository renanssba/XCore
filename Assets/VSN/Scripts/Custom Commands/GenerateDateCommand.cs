using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "generate_date")]
  public class GenerateDateCommand : VsnCommand {

    public override void Execute() {
      BattleController.instance.GenerateDateEnemies();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
