using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "spend_time")]
  public class SpendTimeCommand : VsnCommand {

    public override void Execute() {
      GlobalData.instance.PassDay();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
