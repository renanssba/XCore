using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{

  [CommandAttribute(CommandString = "play_ambience")]
  public class PlayAmbienceCommand : VsnCommand
  {

    public override void Execute() {
        VsnAudioManager.instance.PlayAmbience(args[0].GetStringValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
