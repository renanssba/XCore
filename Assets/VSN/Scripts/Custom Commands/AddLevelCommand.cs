using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_level")]
  public class AddLevelCommand : VsnCommand {

    public override void Execute() {
      int levelToGet = (int)args[0].GetNumberValue();

      if(args.Length > 1) {
        GlobalData.instance.currentRelationshipId = (int)args[1].GetNumberValue();
      }

      GlobalData.instance.GetCurrentRelationship().level += levelToGet;
      UIController.instance.UpdateUI();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}