using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "spend_ap")]
  public class SpendApCommand : VsnCommand {

    public override void Execute() {
      GameController.instance.SpendAP((int)args[0].GetNumberValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
