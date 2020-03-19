using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "check_skill_activation")]
  public class CheckSkillActivationCommand : VsnCommand {

    public override void Execute() {
      string situation = args[1].GetStringValue();
      int skillId = (int)args[0].GetNumberValue();
      Relationship relationship = GlobalData.instance.GetCurrentRelationship();
      Skill skillChecked = BattleController.instance.GetSkillById(relationship.skilltree.skills[skillId].id);

      // return if not checking the correct activation trigger, or the skill is not passive / unlocked
      if(!relationship.skilltree.skills[skillId].isUnlocked || skillChecked.type != SkillType.passive  || skillChecked.activationTrigger.ToString() != situation) {
        return;
      }

      // check trigger chance
      if(Random.Range(0, 100) >= skillChecked.triggerChance) {
        return;
      }

      switch(situation) {
        case "enemy_appears":
          foreach(string tag in BattleController.instance.GetCurrentDateEvent().tags) {
            if(tag == skillChecked.triggerCondition) {
              Debug.LogWarning("Activating skill via enemy appear with tag: " + tag);
              ActivatePassiveSkill(skillChecked, skillId);
              return;
            }
          }
          break;
        case "turn_started":
          int currentTurn = VsnSaveSystem.GetIntVariable("currentBattleTurn");
          if(currentTurn == int.Parse(skillChecked.triggerCondition)) {
            Debug.LogWarning("Activating skill via turn: " + currentTurn);
            ActivatePassiveSkill(skillChecked, skillId);
            return;
          }
          break;
        case "battle_ended":
        case "enemy_defeated":
          ActivatePassiveSkill(skillChecked, skillId);
          break;
      }
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

      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
    }
  }
}
