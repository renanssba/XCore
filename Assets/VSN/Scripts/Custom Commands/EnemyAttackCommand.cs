using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "enemy_attack")]
  public class EnemyAttackCommand : VsnCommand {

    public override void Execute() {
      BattleController.instance.EnemyAttack();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
