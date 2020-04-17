using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "actor_animator_parameter")]
  public class ActorAnimatorParameterCommand : VsnCommand {

    public override void Execute() {
      TheaterController.instance.SetActorAnimatorParameter(args[0].GetReference(),
        args[1].GetStringValue(),
        args[2].GetBooleanValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg,
        VsnArgType.booleanArg
      });
    }
  }
}
