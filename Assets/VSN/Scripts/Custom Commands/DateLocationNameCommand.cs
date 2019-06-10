using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "date_location_name")]
  public class DateLocationNameCommand : VsnCommand {

    public override void Execute() {
      GameController.instance.dateTitleText.text = args[0].GetStringValue();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}