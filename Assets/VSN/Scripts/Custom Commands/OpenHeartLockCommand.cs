using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "open_heart_lock")]
  public class OpenHeartLockCommand : VsnCommand {

    public override void Execute() {
      Relationship relation = GlobalData.instance.GetCurrentRelationship();
      //if(args.Length > 1) {
      //  relation = GlobalData.instance.relationships[(int)args[1].GetNumberValue()];
      //}
      //GlobalData.instance.AddExpForRelationship(relation, (int)args[0].GetNumberValue());
      relation.OpenHeartLock(BattleController.instance.currentDateId);
      VsnController.instance.WaitForCustomInput();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
      //signatures.Add(new VsnArgType[] {
      //  VsnArgType.numberArg,
      //  VsnArgType.numberArg
      //});
    }
  }
}
