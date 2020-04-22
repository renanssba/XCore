using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "unlock_skill")]
  public class UnlockSkillCommand : VsnCommand {

    public override void Execute() {
      Relationship relation = GlobalData.instance.GetCurrentRelationship();
      int skillToUnlock = (int)args[0].GetNumberValue();

      if(relation.skilltree.skills[skillToUnlock].isUnlocked == false) {
        relation.skilltree.skills[skillToUnlock].isUnlocked = true;
        SfxManager.StaticPlayBigConfirmSfx();

        VsnSaveSystem.SetVariable("skill_got", BattleController.instance.GetSkillById(relation.skilltree.skills[skillToUnlock].id).GetPrintableName() );

        VsnArgument[] sayArgs = new VsnArgument[2];
        sayArgs[0] = new VsnString("unlocked_skill");
        sayArgs[1] = new VsnNumber((int)relation.skilltree.skills[skillToUnlock].affectsPerson);
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
