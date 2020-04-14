using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command{

	[CommandAttribute(CommandString="screenshake")]
	public class ScreenShakeCommand : VsnCommand {

		public override void Execute (){
      switch(args.Length){
        case 0:
          TheaterController.instance.Screenshake(1f);
          break;
        case 1:
          TheaterController.instance.Screenshake(args[0].GetNumberValue());
          break;
        case 2:
          TheaterController.instance.Screenshake(args[0].GetNumberValue(), args[1].GetNumberValue());
          break;
      }
		}


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);

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