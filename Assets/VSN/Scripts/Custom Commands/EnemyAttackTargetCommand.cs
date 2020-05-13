using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "enemy_attack_target")]
  public class EnemyAttackTargetCommand : VsnCommand {

    public override void Execute() {
      int targetId = Random.Range(0, Mathf.Min(BattleController.instance.partyMembers.Length, 2));
      VsnSaveSystem.SetVariable("enemyAttackTargetId", targetId);

      //VsnArgument[] newArgs = new VsnArgument[1];
      //string scriptToLoadPath = "date enemies/" + BattleController.instance.GetCurrentEnemyName();

      //switch(targetId) {
      //  case 0:
      //    newArgs[0] = new VsnString("attack_boy");
      //    break;
      //  case 1:
      //    newArgs[0] = new VsnString("attack_girl");
      //    break;
      //  case 2:
      //    newArgs[0] = new VsnString("attack_angel");
      //    break;
      //}
      //GotoScriptCommand.StaticExecute(scriptToLoadPath, newArgs);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
