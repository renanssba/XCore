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
      Skill skillChecked = BattleController.instance.GetSkillById(enemy.passiveSkills[skillId]);


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
      if(!Utils.AreAllConditionsMet(skillChecked.triggerConditions, partyMemberId)) {
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
      Debug.LogWarning("Skill " + skillChecked.name + " is trigged.");

      // check trigger chance
      if(Random.Range(0, 100) >= skillChecked.triggerChance) {
        return;
      }
      Debug.LogWarning("Check for skill activation. Passed trigger chance.");



      // check all trigger conditions
      if(!Utils.AreAllConditionsMet(skillChecked.triggerConditions, partyMemberId)) {
        return;
      }


      Debug.LogWarning("Skill " + skillChecked.name + " activated.");
      // activate passive skill
      ActivatePassiveSkill(skillChecked, partyMemberId);
    }

    
    public static void ActivatePassiveSkill(Skill skillToActivate, int partyMemberId) {
      BattleController battle = BattleController.instance;

      Debug.LogWarning("Activating passive skill: " + skillToActivate+", party member id:" + partyMemberId);
      VsnController.instance.state = ExecutionState.WAITING;
      battle.StartCoroutine(battle.ExecuteBattlerSkill(partyMemberId, partyMemberId, skillToActivate));
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
