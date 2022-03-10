using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "show_date_ui")]
  public class ShowDateUICommand : VsnCommand {

    public override void Execute() {
      //if(args.Length > 0){
      //  UIController.instance.ShowBattleUI(args[0].GetBooleanValue());
      //} else{
      //  UIController.instance.ShowBattleUI(true);
      //}
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg
      });
    }
  }
}
