using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "girl_interaction")]
  public class GirlInteractionCommand : VsnCommand {

    public override void Execute() {
      GlobalData.instance.observedPeople = new Person[] {GlobalData.instance.people[0],
                                                         GlobalData.instance.people[(int)args[0].GetNumberValue()]};

      GameController.instance.ShowGirlInteractionScreen();
      VsnController.instance.state = ExecutionState.WAITINGCUSTOMINPUT;
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}