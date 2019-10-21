using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "pass_time")]
  public class PassTimeCommand : VsnCommand {

    public override void Execute() {
      GlobalData.instance.PassTime();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
