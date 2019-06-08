using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "pass_day")]
  public class PassDayCommand : VsnCommand {

    public override void Execute() {
      GlobalData.instance.PassDay();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
