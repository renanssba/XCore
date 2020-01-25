using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "continue")]
  public class ContinueCommand : VsnCommand {

    public override void Execute() {
      StaticExecute();
    }

    public static void StaticExecute() {
      VsnCommand loopEnd = VsnController.instance.CurrentScriptReader().FindNextEndwhileOrEndforCommand();
      VsnController.instance.CurrentScriptReader().GotoCommandId(loopEnd.commandIndex);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}