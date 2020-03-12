using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "open_heart_lock")]
  public class OpenHeartLockCommand : VsnCommand {

    public override void Execute() {
      Relationship relation = GlobalData.instance.GetCurrentRelationship();

      relation.OpenHeartLock((int)args[0].GetNumberValue());

      SfxManager.StaticPlayBigConfirmSfx();
      VsnArgument[] sayArgs = new VsnArgument[2];
      sayArgs[0] = new VsnString("char_name/none");
      sayArgs[1] = new VsnString("add_heart/say_1");
      Command.SayCommand.StaticExecute(sayArgs);
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
