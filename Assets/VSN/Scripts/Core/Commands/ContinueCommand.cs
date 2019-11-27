using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "continue")]
  public class ContinueCommand : VsnCommand {

    public override void Execute() {
      VsnCommand loopEnd = VsnController.instance.CurrentScriptReader().FindNextEndwhileOrEndforCommand();
      VsnController.instance.CurrentScriptReader().GotoCommandId(loopEnd.commandIndex);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}