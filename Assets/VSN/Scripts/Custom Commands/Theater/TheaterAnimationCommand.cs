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
