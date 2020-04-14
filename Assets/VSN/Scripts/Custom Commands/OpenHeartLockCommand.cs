using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "open_heart_lock")]
  public class OpenHeartLockCommand : VsnCommand {

    public override void Execute() {
      Relationship relation = GlobalData.instance.GetCurrentRelationship();
      int levelToRaise = (int)args[0].GetNumberValue();

      if(relation.OpenHeartLock(levelToRaise) ) {
        SfxManager.StaticPlayBigConfirmSfx();
        VsnArgument[] sayArgs = new VsnArgument[2];
        sayArgs[0] = new VsnString("char_name/none");

        if(levelToRaise == 1) {
          sayArgs[1] = new VsnString("improve_relationship/say_2");
        } else if(levelToRaise == 2) {
          sayArgs[1] = new VsnString("improve_relationship/say_3");
        } else if(levelToRaise == 2) {
          return;
        }
        Command.SayCommand.StaticExecute(sayArgs);
      }
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
