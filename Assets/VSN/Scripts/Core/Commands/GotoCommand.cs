using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "goto")]
  public class GotoCommand : VsnCommand {

    public override void Execute() {
      VsnController.instance.CurrentScriptReader().GotoWaypoint(args[0].GetReference());
    }


    public override void AddSupportedSignatures(){
      signatures.Add( new VsnArgType[]{
        VsnArgType.referenceArg
      } );
    }
  }
}