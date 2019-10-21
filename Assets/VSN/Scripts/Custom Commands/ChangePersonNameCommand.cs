using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "change_person_name")]
  public class ChangePersonNameCommand : VsnCommand {

    public override void Execute() {
      GlobalData.instance.people[(int)args[0].GetNumberValue()].name = args[1].GetStringValue();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
    }
  }
}