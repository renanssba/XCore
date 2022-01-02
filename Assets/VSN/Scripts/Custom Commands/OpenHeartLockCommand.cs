using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "open_heart_lock")]
  public class OpenHeartLockCommand : VsnCommand {

    public override void Execute() {
      Relationship relation = GlobalData.instance.GetCurrentRelationship();
      int levelToRaise = (int)args[0].GetNumberValue();

      VsnSaveSystem.SetVariable("previousOpenedHeartlocks", relation.heartLocksOpened);

      if(relation.heartLocksOpened < levelToRaise) {
        VsnController.instance.state = ExecutionState.WAITING;
        TheaterController.instance.StartCoroutine(TheaterController.instance.OpenHeartlockAnimation(levelToRaise));
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
