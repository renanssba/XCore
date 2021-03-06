using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_interaction_pin")]
  public class SetInteractionPinCommand : VsnCommand {

    public override void Execute() {
      if(args.Length >= 3) {
        UIController.instance.SetInteractionPin((int)args[0].GetNumberValue(), args[1].GetBooleanValue(), args[2].GetStringValue(), args[3].GetStringValue());
      } else {
        UIController.instance.SetInteractionPin((int)args[0].GetNumberValue(), args[1].GetBooleanValue());
      }
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.booleanArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.booleanArg,
        VsnArgType.stringArg,
        VsnArgType.stringArg
      });
    }
  }
}