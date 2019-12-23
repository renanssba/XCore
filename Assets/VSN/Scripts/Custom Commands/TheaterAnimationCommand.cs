using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "theater_animation")]
  public class TheaterAnimationCommand : VsnCommand {

    public override void Execute() {
      switch(args[0].GetStringValue()) {
        case "setup_date":
          TheaterController.instance.SetupDate();
          break;
        case "party_enters":
          TheaterController.instance.PartyEntersScene();
          break;
        case "enemy_enters":
          TheaterController.instance.EnemyEntersScene();
          break;
        case "enemy_leaves":
          TheaterController.instance.EnemyLeavesScene();
          break;
      }      
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
