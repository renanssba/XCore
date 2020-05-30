using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "girl_interaction")]
  public class GirlInteractionCommand : VsnCommand {

    public override void Execute() {
      if(args.Length >= 1) {
        GlobalData.instance.currentRelationshipId = (int)args[0].GetNumberValue();
      }
      UIController.instance.girlInteractionScreen.ShowGirlInteractionScreen();
      VsnController.instance.state = ExecutionState.WAITINGCUSTOMINPUT;
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
