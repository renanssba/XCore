using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "enable_input")]
  public class EnableInputCommand : VsnCommand {

    public override void Execute() {
      VsnController.instance.BlockExternalInput(!args[0].GetBooleanValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg
      });
    }
  }
}