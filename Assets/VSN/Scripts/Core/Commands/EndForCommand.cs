using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "endfor")]
  public class EndForCommand : VsnCommand {

    public override void Execute() {
      Debug.LogWarning("Called endfor");
      VsnCommand forCommand = VsnController.instance.CurrentScriptReader().ReturnPreviousForCommand();
      if(forCommand == null) {
        Debug.LogError("Invalid for/endfor structure. Please check the commands");
        return;
      }

      ((ForCommand)forCommand).ExecuteStep();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}