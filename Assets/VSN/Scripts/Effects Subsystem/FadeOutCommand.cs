using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "fade_out")]
  public class FadeOutCommand : VsnCommand {

    public override void Execute() {
      VsnEffectManager.instance.SetFadeColor(Color.white);

      if(args.Length == 1) {
        VsnEffectManager.instance.FadeOut(args[0].GetNumberValue());
      } else if(args.Length == 2) {
        VsnEffectManager.instance.SetFadeColor( Utils.GetColorByString(args[1].GetStringValue()) );
        VsnEffectManager.instance.FadeOut(args[0].GetNumberValue());
      }
      DialogStyleCommand.StaticExecute("faded_screen_message");
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.stringArg
      });
    }
  }
}