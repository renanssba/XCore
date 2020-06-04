using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "cupid_diary")]
  public class CupidDiaryCommand : VsnCommand {

    public override void Execute() {
      bool showValue = args[0].GetBooleanValue();

      if(showValue) {
        CupidDiaryController.instance.panel.ShowPanel();
        VsnController.instance.WaitForCustomInput();
      } else {
        CupidDiaryController.instance.panel.HidePanel();
      }      
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg
      });
    }
  }
}
