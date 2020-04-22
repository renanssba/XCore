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

      if(relation.OpenHeartLock(levelToRaise) ) {
        SfxManager.StaticPlayBigConfirmSfx();

        VsnArgument[] sayArgs = new VsnArgument[1];
        sayArgs[0] = new VsnString("open_heartlock");
        Command.GotoScriptCommand.StaticExecute("after_get_exp", sayArgs);
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
