using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_current_couple")]
  public class SetCurrentCoupleCommand : VsnCommand {

    public override void Execute() {
      int coupleId = (int)args[0].GetNumberValue();
      GlobalData.instance.currentRelationshipId = coupleId;
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}