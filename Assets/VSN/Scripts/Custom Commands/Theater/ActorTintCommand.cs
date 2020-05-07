using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Command {

  [CommandAttribute(CommandString = "actor_tint")]
  public class ActorTintCommand : VsnCommand {

    public override void Execute() {
      Actor2D actor = TheaterController.instance.GetActorByString(args[0].GetReference());
      float flashAmount = args[1].GetNumberValue();
      float duration = args[2].GetNumberValue();
      Color color = Utils.GetColorByString(args[3].GetStringValue());

      if(actor == null) {
        return;
      }

      actor.TintActorToColor(flashAmount, duration, color);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.numberArg,
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
    }
  }
}
