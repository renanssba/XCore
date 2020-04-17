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
          VsnController.instance.WaitForSeconds(1.5f);
          break;
        case "enemy_enters":
          TheaterController.instance.EnemyEntersScene();
          VsnController.instance.WaitForSeconds(1.2f);
          break;
        case "enemy_leaves":
          TheaterController.instance.EnemyLeavesScene();
          VsnController.instance.WaitForSeconds(0.7f);
          break;
        case "girl_waiting_for_boy":
          TheaterController.instance.SetupGirlWaitingForBoy();
          break;
        case "boy_waiting_for_girl":
          TheaterController.instance.SetupBoyWaitingForGirl();
          break;
        case "main_actor_enters":
          TheaterController.instance.MainActorEntersScene();
          break;
        case "support_actor_enters":
          TheaterController.instance.SupportActorEntersScene();
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
