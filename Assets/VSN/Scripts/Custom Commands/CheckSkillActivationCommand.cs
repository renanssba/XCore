using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "check_skill_activation")]
  public class CheckSkillActivationCommand : VsnCommand {

    public override void Execute() {
      int skillPos = (int)args[1].GetNumberValue();
      string situation = args[2].GetStringValue();

      switch(args[0].GetStringValue()) {
        case "players":
          CheckPlayersSkills(skillPos, situation);
          break;
        case "enemy":
          CheckEnemySkills(skillPos, situation);
          break;
      }
    }


    public void CheckEnemySkills(int skillPos, string situation) {
      Enemy enemy = BattleController.instance.GetCurrentEnemy();
      Skill skillChecked = BattleController.instance.GetSkillById(enemy.passiveSkills[skillPos]);


      //Debug.LogWarning("CHECKING for ENEMY skill activation. skill: "+skillChecked.name+", situation: "+situation);
      // return if not checking the correct activation trigger, or the skill is not passive / unlocked
      if(skillChecked.type != SkillType.passive || skillChecked.activationTrigger.ToString() != situation) {
        return;
      }

      // check trigger chance
      if(Random.Range(0, 100) >= skillChecked.triggerChance) {
        return;
      }
      //Debug.LogWarning("Check for skill activation. Passed trigger chance.");


      // check all trigger conditions
      if(!Utils.AreAllConditionsMet(skillChecked, skillChecked.triggerConditions, SkillTarget.enemy1)) {
        return;
      }


      // activate passive skill
      ActivatePassiveSkill(skillChecked, SkillTarget.enemy1);
    }


    public void CheckPlayersSkills(int skillPos, string situation) {
      Relationship relationship = GlobalData.instance.GetCurrentRelationship();
      SkillTarget partyMemberId = SkillTarget.allHeroes;
      Skill skillChecked = BattleController.instance.GetSkillById(relationship.skilltree.skills[skillPos].id);


      //Debug.LogWarning("CHECKING for PLAYER skill activation. skill: "+skillChecked.name+", situation: "+situation);

      // return if there are no applicable heroes in party
      if(partyMemberId == SkillTarget.none) {
        return;
      }
      
      // return if not checking the correct activation trigger, or the skill is not passive / unlocked
      if(!relationship.skilltree.skills[skillPos].isUnlocked || skillChecked.type != SkillType.passive || skillChecked.activationTrigger.ToString() != situation) {
        return;
      }
      //Debug.LogWarning("Skill " + skillChecked.name + " is trigged.");

      // check trigger chance
      if(Random.Range(0, 100) >= skillChecked.triggerChance) {
        return;
      }
      //Debug.LogWarning("Check for skill activation. Passed trigger chance.");



      // check all trigger conditions
      if(!Utils.AreAllConditionsMet(skillChecked, skillChecked.triggerConditions, partyMemberId)) {
        return;
      }


      // activate passive skill
      ActivatePassiveSkill(skillChecked, partyMemberId);
    }

    
    public static void ActivatePassiveSkill(Skill skillToActivate, SkillTarget partyMemberId) {
      BattleController battle = BattleController.instance;
      SkillTarget target = GetTargetFromSkill(skillToActivate, partyMemberId);

      Debug.LogWarning("Activating passive skill: " + skillToActivate+", party member id:" + partyMemberId);

      VsnController.instance.state = ExecutionState.WAITING;
      battle.StartCoroutine(battle.ExecuteBattlerSkill(partyMemberId, target, skillToActivate));
    }


    public static SkillTarget GetTargetFromSkill(Skill skillToActivate, SkillTarget partyMemberId) {
      if(skillToActivate.range == ActionRange.self) {
        return partyMemberId;
      }
      
      switch(partyMemberId) {
        case SkillTarget.enemy1:
        case SkillTarget.enemy2:
        case SkillTarget.enemy3:
          if(skillToActivate.range == ActionRange.all_allies) {
            return SkillTarget.allEnemies;
          }else if(skillToActivate.range == ActionRange.all_enemies) {
            return SkillTarget.allHeroes;
          }
          break;
        case SkillTarget.partyMember1:
        case SkillTarget.partyMember2:
        case SkillTarget.partyMember3:
          if(skillToActivate.range == ActionRange.all_allies) {
            return SkillTarget.allHeroes;
          } else if(skillToActivate.range == ActionRange.all_enemies) {
            return SkillTarget.allEnemies;
          }
          break;
      }
      return SkillTarget.none;
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
