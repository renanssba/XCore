using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_overlay")]
  public class AddOverlayCommand : VsnCommand {

    public override void Execute() {
      Actor2D targetActor = TheaterController.instance.GetActorByString(args[0].GetReference());

      if(args[2].GetBooleanValue()) {
        //Debug.LogWarning("Add overlay: " + args[1].GetStringValue());
        targetActor.AddOverlay(args[1].GetStringValue());
      } else {
        targetActor.RemoveOveraly(args[1].GetStringValue());
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg,
        VsnArgType.booleanArg
      });
    }
  }
}