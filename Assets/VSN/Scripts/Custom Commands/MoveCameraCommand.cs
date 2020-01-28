using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "move_camera")]
  public class MoveCameraCommand : VsnCommand {

    public override void Execute() {
      Vector3 vec = new Vector3(args[0].GetNumberValue(),
        args[1].GetNumberValue(),
        args[2].GetNumberValue());
      float duration = args[3].GetNumberValue();

      TheaterController.instance.MoveCamera(vec, duration);
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
