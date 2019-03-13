using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "generate_date")]
  public class GenerateDateCommand : VsnCommand {

    public override void Execute() {
      if(args.Length>0){
        GameController.instance.GenerateDate((int)args[0].GetNumberValue());
      } else{
        GameController.instance.GenerateDate(0);
      }
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
