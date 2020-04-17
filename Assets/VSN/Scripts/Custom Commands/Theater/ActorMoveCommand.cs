using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Command {

  [CommandAttribute(CommandString = "actor_move")]
  public class ActorMoveCommand : VsnCommand {

    public override void Execute() {
      Actor2D actor = TheaterController.instance.GetActorByString(args[0].GetReference());
      Vector3 position = Vector3.zero;
      float duration;

      if(actor == null) {
        return;
      }

      if(args.Length == 3) {
        duration = args[2].GetNumberValue();
        position = TheaterController.instance.GetPositionByString(args[1].GetStringValue());
        actor.MoveToPosition(position, duration);
        return;
      }

      position = new Vector3(args[1].GetNumberValue(), args[2].GetNumberValue(), args[3].GetNumberValue());
      duration = args[4].GetNumberValue();
      actor.MoveToPosition(position, duration);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.numberArg,
        VsnArgType.numberArg,
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}
