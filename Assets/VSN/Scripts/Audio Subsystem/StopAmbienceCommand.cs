using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{

  [CommandAttribute(CommandString = "stop_ambience")]
  public class StopAmbienceCommand : VsnCommand
  {

    public override void Execute() {
        VsnAudioManager.instance.StopAmbience(args[0].GetStringValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
