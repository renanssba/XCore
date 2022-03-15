using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "character_action")]
  public class CharacterActionCommand : VsnCommand {

    public override void Execute() {
      if(BattleController.instance.CurrentBattler.GetType() != typeof(Enemy)) {
        BattleController.instance.CharacterTurn();
      } else {
        Debug.LogWarning("character_action for enemy!");
        BattleController.instance.SelectEnemyTarget();
        BattleController.instance.EnemyAttack();
      }
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}