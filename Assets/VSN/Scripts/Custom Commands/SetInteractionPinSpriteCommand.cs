using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_interaction_pin_sprite")]
  public class SetInteractionPinSpriteCommand : VsnCommand {

    public override void Execute() {
      GameController.instance.SetInteractionPinSprite((int)args[0].GetNumberValue(), args[1].GetStringValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
    }
  }
}