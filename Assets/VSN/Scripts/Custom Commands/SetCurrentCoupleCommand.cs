using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_current_couple")]
  public class SetCurrentCoupleCommand : VsnCommand {

    public override void Execute() {
      int coupleId = (int)args[0].GetNumberValue();
      if(coupleId == 0){
        GlobalData.instance.observedPeople = new Person[] { GlobalData.instance.people[0] };
        return;
      } else if(coupleId < 0) {
        GlobalData.instance.observedPeople = new Person[] { GlobalData.instance.people[-coupleId] };
        return;
      } else {
        GlobalData.instance.observedPeople = new Person[] {GlobalData.instance.people[0],
                                                         GlobalData.instance.people[coupleId]};
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}