using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "save")]
  public class SaveCommand : VsnCommand {

    public override void Execute() {
      int saveSlot = 0;

      if(args.Length > 0){
        saveSlot = (int)args[0].GetNumberValue();
      }
      //Debug.LogError("SAVING!!!!!!!!!");

      GlobalData.instance.SavePersistantGlobalData();
      VsnSaveSystem.Save(saveSlot);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);

      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}