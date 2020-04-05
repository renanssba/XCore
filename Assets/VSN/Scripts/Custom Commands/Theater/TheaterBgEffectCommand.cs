using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "theater_bg_effect")]
  public class TheaterBgEffectCommand : VsnCommand {

    public override void Execute() {
      BgEffect bgEffect = GetBgEffectByString(args[0].GetStringValue());

      TheaterController.instance.ApplyBgEffect(bgEffect, (int)args[1].GetNumberValue());
    }

    public BgEffect GetBgEffectByString(string effect) {
      for(int i = 0; i <= (int)BgEffect.count; i++) {
        if(((BgEffect)i).ToString() == effect) {
          return (BgEffect)i;
        }
      }
      return BgEffect.pulsingEffect;
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}
