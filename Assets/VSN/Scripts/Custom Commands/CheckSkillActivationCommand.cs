using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "check_skill_activation")]
  public class CheckSkillActivationCommand : VsnCommand {

    public override void Execute() {
      int skillId = (int)args[0].GetNumberValue();
      string situation = args[1].GetStringValue();
      Relationship relationship = GlobalData.instance.GetCurrentRelationship();
      int partyMemberId = -1;
      int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
      string conditionArgument;
      switch(relationship.skilltree.skills[skillId].affectsPerson) {
        case SkillAffectsCharacter.boy:
          partyMemberId = 0;
          break;
        case SkillAffectsCharacter.girl:
          partyMemberId = 1;
          break;
      }
      Skill skillChecked = BattleController.instance.GetSkillById(relationship.skilltree.skills[skillId].id);

      Debug.LogWarning("CHECKING for skill activation. skill: "+skillChecked.name+", situation: "+situation);

      // return if not checking the correct activation trigger, or the skill is not passive / unlocked
      if(!relationship.skilltree.skills[skillId].isUnlocked || skillChecked.type != SkillType.passive  || skillChecked.activationTrigger.ToString() != situation) {
        return;
      }

      Debug.LogWarning("Skill is viable.");

      // check trigger chance
      if(Random.Range(0, 100) >= skillChecked.triggerChance) {
        return;
      }

      Debug.LogWarning("Check for skill activation. Passed trigger chance.");

      // check trigger conditions
      foreach(string rawCondition in skillChecked.triggerConditions) {
        string condition = rawCondition.Trim();

        Debug.LogWarning("Check for skill activation. Checking condition: " + condition);

        if(condition.StartsWith("enemy_has_tag")) {
          conditionArgument = GetArgument(condition);
          if(!TagIsInArray(conditionArgument, BattleController.instance.GetCurrentEnemy().tags)) {
            return;
          }
        }

        if(condition.StartsWith("turn")) {
          conditionArgument = GetArgument(condition);
          if(VsnSaveSystem.GetIntVariable("currentBattleTurn") != int.Parse(conditionArgument)) {
            return;
          }
        }

        if(condition.StartsWith("received_item_with_tag")) {
          Debug.LogWarning("Checking received_item_with_tag. Action: " + BattleController.instance.selectedActionType[currentPlayerTurn]);
          if(BattleController.instance.selectedActionType[currentPlayerTurn] != TurnActionType.useItem) {
            return;
          }
          conditionArgument = GetArgument(condition);
          Debug.LogWarning("Condition argument: " + conditionArgument);
          if(!TagIsInArray(conditionArgument, BattleController.instance.selectedItems[currentPlayerTurn].tags) ||
             BattleController.instance.selectedTargetPartyId[currentPlayerTurn] != partyMemberId) {
            return;
          }
        }


        if(condition == "ally_targeted" && VsnSaveSystem.GetIntVariable("enemyAttackTargetId") == partyMemberId) {
          return;
        }

        if(condition == "self_targeted" && VsnSaveSystem.GetIntVariable("enemyAttackTargetId") != partyMemberId) {
          return;
        }

        if(condition == "defending" && BattleController.instance.selectedActionType[partyMemberId] != TurnActionType.defend) {
          return;
        }
      }

      // activate passive skill
      ActivatePassiveSkill(skillChecked, skillId);
    }

    public bool TagIsInArray(string tagToCheck, string[] tags) {
      foreach(string tag in tags) {
        if(tag == tagToCheck) {
          return true;
        }
      }
      return false;
    }


    public string GetArgument(string condition) {
      int start = condition.IndexOf("(");
      int end = condition.IndexOf(")");

      if(start == -1 || end == -1){
        return null;
      }

      string argumentName = condition.Substring(start+1, (end-start-1));
      Debug.LogWarning("GET ARGUMENT: '"+argumentName+"'  ");

      return argumentName;
    }


    public static void ActivatePassiveSkill(Skill skillToActivate, int skillPosition) {
      Debug.LogWarning("Activating passive skill: " + skillToActivate);
      BattleController battle = BattleController.instance;
      Relationship relationship = GlobalData.instance.GetCurrentRelationship();
      int partyMemberId = 0;

      switch(relationship.skilltree.skills[skillPosition].affectsPerson) {
        case SkillAffectsCharacter.boy:
          partyMemberId = 0;
          break;
        case SkillAffectsCharacter.girl:
          partyMemberId = 1;
          break;
        case SkillAffectsCharacter.couple:
          return;
      }

      VsnController.instance.state = ExecutionState.WAITING;
      battle.StartCoroutine(battle.ExecuteCharacterSkill(partyMemberId, partyMemberId, skillToActivate));
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
    }
  }
}
