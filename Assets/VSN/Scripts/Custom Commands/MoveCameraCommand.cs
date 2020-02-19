using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "move_camera")]
  public class MoveCameraCommand : VsnCommand {

    public override void Execute() {
      Vector3 cameraPos;
      float duration = args[1].GetNumberValue();

      if(args.Length >= 4) {
        cameraPos = new Vector3(args[0].GetNumberValue(),
          args[1].GetNumberValue(),
          args[2].GetNumberValue());
        duration = args[3].GetNumberValue();
      }


      switch(args[0].GetStringValue()) {
        case "close_shot":
          cameraPos = new Vector3(-0.91f, 1.52f, -2.8f);
          break;
        case "girl_interaction":
          cameraPos = new Vector3(-0.85f, 1.57f, -2.92f);
          break;
        case "battle_view":
        default:
          cameraPos = new Vector3(0f, 1.2f, -6.4f);
          break;
      }

      TheaterController.instance.MoveCamera(cameraPos, duration);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg,
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}
