using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "heal_hp_percent")]
  public class HealHpPercentCommand : VsnCommand {

    public override void Execute() {
      Battler target = BattleController.instance.GetBattlerByString(args[0].GetStringValue());
      int valueToHeal = (int)(args[1].GetNumberValue() * target.MaxHP());
      target.HealHP(valueToHeal);

      VsnSaveSystem.SetVariable("recovered_hp", valueToHeal);

      VsnArgument[] sayargs = new VsnArgument[2];
      sayargs[0] = new VsnString("char_name/none");
      sayargs[1] = new VsnString("date/heal_hp");
      SayCommand.StaticExecute(sayargs);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}
