using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "goto")]
  public class GotoCommand : VsnCommand {

    public override void Execute() {
      Debug.Log("arg0: "+args[0].GetReference());
      StaticExecute(args[0].GetReference());
    }

    public static void StaticExecute(string waypointName) {
      VsnController.instance.CurrentScriptReader().GotoWaypoint(waypointName);
    }


    public override void AddSupportedSignatures(){
      signatures.Add( new VsnArgType[]{
        VsnArgType.referenceArg
      } );
    }
  }
}