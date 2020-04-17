using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "use_skill")]
  public class UseSkillCommand : VsnCommand {

    public override void Execute() {
      Battler user = BattleController.instance.GetBattlerByString(args[0].GetStringValue());
      Battler target = BattleController.instance.GetBattlerByString(args[1].GetStringValue());
      int userPos = BattleController.instance.GetPartyMemberPosition(user);
      int targetPos = BattleController.instance.GetPartyMemberPosition(target);
      Skill usedSkill = BattleController.instance.GetSkillById((int)args[2].GetNumberValue());

      Debug.LogWarning("USE SKILL: " + usedSkill + ", user: " + user.GetName() + ", target: " + target.GetName());

      BattleController battle = BattleController.instance;
      VsnController.instance.state = ExecutionState.WAITING;
      battle.StartCoroutine(battle.ExecuteBattlerSkill(userPos, targetPos, usedSkill));
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}
