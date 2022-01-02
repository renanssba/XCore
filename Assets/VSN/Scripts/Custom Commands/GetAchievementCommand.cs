using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "get_achievement")]
  public class GetAchievementCommand : VsnCommand {

    public override void Execute() {
      AchievementsController.ReceiveAchievement(args[0].GetStringValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
