using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "endwhile")]
  public class EndWhileCommand : VsnCommand {

    public override void Execute() {
      VsnController.instance.CurrentScriptReader().GotoPreviousWhile();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}