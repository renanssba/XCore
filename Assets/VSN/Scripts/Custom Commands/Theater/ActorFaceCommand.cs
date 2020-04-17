using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "actor_face")]
  public class ActorFaceCommand : VsnCommand {

    public override void Execute() {
      Actor2D actor = TheaterController.instance.GetActorByString(args[0].GetReference());

      switch(args[1].GetStringValue()) {
        case "right":
          actor.FaceRight();
          break;
        case "left":
          actor.FaceLeft();
          break;
      }
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.stringArg
      });
    }
  }
}
