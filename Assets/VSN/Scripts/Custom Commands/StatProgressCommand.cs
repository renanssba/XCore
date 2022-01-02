using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "stat_progress")]
  public class StatProgressCommand : VsnCommand {

    public override void Execute() {
      AchievementsController.StatProgress(args[0].GetStringValue(), (int)args[1].GetNumberValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}
