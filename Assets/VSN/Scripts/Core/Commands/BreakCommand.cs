using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "break")]
  public class BreakCommand : VsnCommand {

    public override void Execute() {
      VsnCommand loopEnd = VsnController.instance.CurrentScriptReader().FindNextEndwhileOrEndforCommand();
      VsnController.instance.CurrentScriptReader().GotoCommandId(loopEnd.commandIndex+1);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}