using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "check_action_skip")]
  public class CheckActionSkipCommand : VsnCommand {

    public override void Execute() {
      int partyMemberId = (int)args[0].GetNumberValue();

      // if action is idle
      if(BattleController.instance.selectedActionType[partyMemberId] == TurnActionType.idle &&
         BattleController.instance.partyMembers[partyMemberId].id == (int)PersonId.fertiliel) {
        ContinueCommand.StaticExecute();
        return;
      }

      // if enemy is already dead
      if(BattleController.instance.GetCurrentEnemy().hp <= 0 &&
         BattleController.instance.selectedActionType[partyMemberId] == TurnActionType.useSkill &&
         BattleController.instance.selectedSkills[partyMemberId].type == SkillType.attack) {
        ContinueCommand.StaticExecute();
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}