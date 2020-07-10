using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command{

  [CommandAttribute(CommandString="skip_button")]
  public class SkipButtonCommand : VsnCommand {

    public override void Execute (){
      //VsnUIManager.instance.ShowSkipButton(args[0].GetBooleanValue());
      //if(args.Length > 1) {
      //  VsnUIManager.instance.SetSkipButtonWaypoint(args[1].GetReference());
      //}
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg,
        VsnArgType.referenceArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg
      });
    }
  }
}