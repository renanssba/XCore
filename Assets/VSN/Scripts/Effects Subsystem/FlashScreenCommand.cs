using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command{

	[CommandAttribute(CommandString="flash")]
	public class FlashScreenCommand : VsnCommand {

		public override void Execute (){
      VsnEffectManager.instance.FlashScreen(args[0].GetStringValue(), args[1].GetNumberValue());
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
	}
}