using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_current_couple")]
  public class SetCurrentCoupleCommand : VsnCommand {

    public override void Execute() {
      if(args[0].GetNumberValue() == -1){
        GlobalData.instance.observedPeople = new Person[] { GlobalData.instance.people[0] };
        return;
      }

      GlobalData.instance.observedPeople = new Person[] {GlobalData.instance.people[0],
                                                         GlobalData.instance.people[(int)args[0].GetNumberValue()]};
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}