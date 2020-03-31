using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "check_skill_activation")]
  public class CheckSkillActivationCommand : VsnCommand {

    public override void Execute() {
      int skillId = (int)args[1].GetNumberValue();
      string situation = args[2].GetStringValue();

      switch(args[0].GetStringValue()) {
        case "players":
          CheckPlayersSkills(skillId, situation);
          break;
        case "enemy":
          CheckEnemySkills(skillId, situation);
          break;
      }
    }


    public void CheckEnemySkills(int skillId, string situation) {
      Enemy enemy = BattleController.instance.GetCurrentEnemy();
      int partyMemberId = 3;
      Skill skillChecked = BattleController.instance.GetSkillById(enemy.skills[skillId]);


      Debug.LogWarning("CHECKING for ENEMY skill activation. skill: "+skillChecked.name+", situation: "+situation);
      // return if not checking the correct activation trigger, or the skill is not passive / unlocked
      if(skillChecked.type != SkillType.passive || skillChecked.activationTrigger.ToString() != situation) {
        return;
      }

      // check trigger chance
      if(Random.Range(0, 100) >= skillChecked.triggerChance) {
        return;
      }
      Debug.LogWarning("Check for skill activation. Passed trigger chance.");


      // check all trigger conditions
      if(!AreAllConditionsMet(skillChecked.triggerConditions, partyMemberId)) {
        return;
      }


      Debug.LogWarning("Skill activated.");
      // activate passive skill
      ActivatePassiveSkill(skillChecked, partyMemberId);
    }


    public void CheckPlayersSkills(int skillId, string situation) {
      Relationship relationship = GlobalData.instance.GetCurrentRelationship();
      int partyMemberId = -1;
      switch(relationship.skilltree.skills[skillId].affectsPerson) {
        case SkillAffectsCharacter.boy:
          partyMemberId = 0;
          break;
        case SkillAffectsCharacter.girl:
          partyMemberId = 1;
          break;
      }
      Skill skillChecked = BattleController.instance.GetSkillById(relationship.skilltree.skills[skillId].id);


      Debug.LogWarning("CHECKING for PLAYER skill activation. skill: "+skillChecked.name+", situation: "+situation);
      // return if not checking the correct activation trigger, or the skill is not passive / unlocked
      if(!relationship.skilltree.skills[skillId].isUnlocked || skillChecked.type != SkillType.passive || skillChecked.activationTrigger.ToString() != situation) {
        return;
      }
      Debug.LogWarning("Skill is trigged.");

      // check trigger chance
      if(Random.Range(0, 100) >= skillChecked.triggerChance) {
        return;
      }
      Debug.LogWarning("Check for skill activation. Passed trigger chance.");



      // check all trigger conditions
      if(!AreAllConditionsMet(skillChecked.triggerConditions, partyMemberId)) {
        return;
      }

      
      Debug.LogWarning("Skill activated.");
      // activate passive skill
      ActivatePassiveSkill(skillChecked, partyMemberId);
    }


    public bool AreAllConditionsMet(string[] allConditions, int partyMemberId) {
      string conditionArgument;
      int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");


      foreach(string condition in allConditions) {
        conditionArgument = Utils.GetStringArgument(condition);

        Debug.LogWarning("Check for skill activation. Checking condition: " + condition);

        if(condition.StartsWith("enemy_has_tag")) {
          if(!TagIsInArray(conditionArgument, BattleController.instance.GetCurrentEnemy().tags)) {
            return false;
          }
        }

        if(condition.StartsWith("is_turn")) {
          if(VsnSaveSystem.GetIntVariable("currentBattleTurn") != int.Parse(conditionArgument)) {
            return false;
          }
        }

        if(condition.StartsWith("turn_is_multiple")) {
          if(VsnSaveSystem.GetIntVariable("currentBattleTurn") % int.Parse(conditionArgument) != 0) {
            return false;
          }
        }

        if(condition.StartsWith("received_item_with_tag")) {
          Debug.LogWarning("Checking received_item_with_tag. Action: " + BattleController.instance.selectedActionType[currentPlayerTurn]);
          if(BattleController.instance.selectedActionType[currentPlayerTurn] != TurnActionType.useItem) {
            return false;
          }
          conditionArgument = Utils.GetStringArgument(condition);
          Debug.LogWarning("Condition argument: " + conditionArgument);
          if(!TagIsInArray(conditionArgument, BattleController.instance.selectedItems[currentPlayerTurn].tags) ||
             BattleController.instance.selectedTargetPartyId[currentPlayerTurn] != partyMemberId) {
            return false;
          }
        }


        if(condition == "ally_targeted" && VsnSaveSystem.GetIntVariable("enemyAttackTargetId") == partyMemberId) {
          return false;
        }

        if(condition == "self_targeted" && VsnSaveSystem.GetIntVariable("enemyAttackTargetId") != partyMemberId) {
          return false;
        }

        if(condition == "defending" && BattleController.instance.selectedActionType[partyMemberId] != TurnActionType.defend) {
          return false;
        }
      }
      return true;
    }


    public bool TagIsInArray(string tagToCheck, string[] tags) {
      foreach(string tag in tags) {
        if(tag == tagToCheck) {
          return true;
        }
      }
      return false;
    }

    
    public static void ActivatePassiveSkill(Skill skillToActivate, int partyMemberId) {
      BattleController battle = BattleController.instance;

      Debug.LogWarning("Activating passive skill: " + skillToActivate+", party member id:" + partyMemberId);
      VsnController.instance.state = ExecutionState.WAITING;
      battle.StartCoroutine(battle.ExecuteCharacterSkill(partyMemberId, partyMemberId, skillToActivate));
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
    }
  }
}
