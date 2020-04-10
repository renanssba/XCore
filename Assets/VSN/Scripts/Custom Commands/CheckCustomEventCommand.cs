using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "check_custom_event")]
  public class CheckCustomEventCommand : VsnCommand {

    public override void Execute() {
      int customEventPos = (int)args[0].GetNumberValue();
      string situation = args[1].GetStringValue();
      Enemy enemy = BattleController.instance.GetCurrentEnemy();

      Debug.LogWarning("CHECKING for custom event activation: " + enemy.customEvents[customEventPos].scriptWaypoint);
      
      // check all trigger conditions
      if(!Utils.AreAllConditionsMet(null, enemy.customEvents[customEventPos].conditions, 3) ||
         enemy.customEvents[customEventPos].situationTrigger != situation) {
        return;
      }

      Debug.LogWarning("Custom event " + enemy.customEvents[customEventPos].scriptWaypoint + "activated!!!");
      // activate custom event
      StartCustomEvent(enemy.customEvents[customEventPos].scriptWaypoint);
    }


    public static void StartCustomEvent(string customEventWaypoint) {
      string scriptToLoadPath = "date enemies/" + BattleController.instance.GetCurrentEnemyName();
      VsnArgument[] newArgs = new VsnArgument[1];
      newArgs[0] = new VsnString(customEventWaypoint);
      GotoScriptCommand.StaticExecute(scriptToLoadPath, newArgs);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
    }
  }
}
