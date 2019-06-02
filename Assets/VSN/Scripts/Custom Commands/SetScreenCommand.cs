using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_screen")]
  public class SetScreenCommand : VsnCommand {

    public override void Execute() {

      VsnSaveSystem.SetVariable("people_ui_state", args[0].GetStringValue());
      GameController.instance.SetScreenLayout(args[0].GetStringValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
