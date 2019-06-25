using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "fade_music")]
  public class FadeMusicCommand : VsnCommand {

    public override void Execute() {
      VsnAudioManager.instance.FadeMusic(args[0].GetNumberValue(), args[1].GetNumberValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}