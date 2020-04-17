using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "actor_become")]
  public class ActorBecomeCommand : VsnCommand {

    public override void Execute() {
      TheaterController.instance.ChangeActor(args[0].GetReference(), args[1].GetStringValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg
      });
    }
  }
}
