using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "remove_context")]
  public class RemoveContextCommand : VsnCommand {

    public override void Execute() {
      JoystickController.instance.RemoveContext(args[0].GetStringValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
