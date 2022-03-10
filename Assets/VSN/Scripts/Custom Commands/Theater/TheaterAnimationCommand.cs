using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "theater_animation")]
  public class TheaterAnimationCommand : VsnCommand {

    public override void Execute() {
      switch(args[0].GetStringValue()) {
        case "setup_battle":
          TheaterController.instance.SetupBattle();
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
        case "main_actor_enters":
          TheaterController.instance.MainActorEntersScene();
          break;
        case "support_actor_enters":
          TheaterController.instance.SupportActorEntersScene();
          break;
        case "color_bg_dark":
          TheaterController.instance.bgRenderer.color = new Color(0.8f, 0.8f, 0.8f);
          break;
        case "color_bg_normal":
          TheaterController.instance.bgRenderer.color = Color.white;
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
