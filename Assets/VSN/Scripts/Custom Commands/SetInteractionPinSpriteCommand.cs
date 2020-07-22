using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_interaction_pin_sprite")]
  public class SetInteractionPinSpriteCommand : VsnCommand {

    public override void Execute() {

      if(args.Length == 2) {
        UIController.instance.SetInteractionPinSprite((int)args[0].GetNumberValue(), args[1].GetStringValue());
      } else {
        UIController.instance.SetInteractionPinSprite((int)args[0].GetNumberValue(), args[1].GetStringValue());
        UIController.instance.SetInteractionPinLocationName((int)args[0].GetNumberValue(), args[2].GetStringValue());
      }      
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg,
        VsnArgType.stringArg
      });
    }
  }
}