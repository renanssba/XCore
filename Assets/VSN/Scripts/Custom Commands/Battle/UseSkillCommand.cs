using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "use_skill")]
  public class UseSkillCommand : VsnCommand {

    public override void Execute() {
      Battler user = BattleController.instance.GetBattlerByString(args[0].GetReference());
      SkillTarget userPos = BattleController.instance.GetPartyMemberPosition(user);
      SkillTarget targetPos = BattleController.instance.GetSkillTargetByString(args[1].GetStringValue());
      Skill usedSkill = BattleController.instance.GetSkillById((int)args[2].GetNumberValue());

      Debug.LogWarning("USE SKILL: " + usedSkill.name + ", user: " + user.GetName() + ", target: " + targetPos);

      BattleController battle = BattleController.instance;
      VsnController.instance.state = ExecutionState.WAITING;
      battle.StartCoroutine(battle.ExecuteBattlerSkill(userPos, targetPos, usedSkill));
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}
