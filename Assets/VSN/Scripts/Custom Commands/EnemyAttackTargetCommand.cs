using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "enemy_attack_target")]
  public class EnemyAttackTargetCommand : VsnCommand {

    public override void Execute() {
      int currentBattler = VsnSaveSystem.GetIntVariable("currentBattlerTurn");
      int targetId = Random.Range(0, BattleController.instance.partyMembers.Length);
      BattleController.instance.selectedTargetPartyId[currentBattler] = (SkillTarget)targetId;

      // old implementation
      //int targetId = Random.Range(0, Mathf.Min(BattleController.instance.partyMembers.Length, 2));
      //VsnSaveSystem.SetVariable("enemyAttackTargetId", targetId);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
