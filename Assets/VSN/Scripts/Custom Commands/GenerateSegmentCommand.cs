using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "generate_segment")]
  public class GenerateSegmentCommand : VsnCommand {

    public override void Execute() {
      if(args[0].GetStringValue() == "date") {
        if (args.Length >= 2) {
          GameController.instance.GenerateDate((int)args[1].GetNumberValue());
        } else {
          GameController.instance.GenerateDate(0);
        }
      } else{
        Debug.LogWarning("GENERATING OBSERVATION SEGMENT");
        GameController.instance.GenerateObservation();
      }
      
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] { 
        VsnArgType.stringArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}
