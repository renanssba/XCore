using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_actor_animator_parameter")]
  public class SetActorAnimatorCommand : VsnCommand {

    public override void Execute() {
      TheaterController.instance.SetActorAnimatorParameter(args[0].GetStringValue(),
        args[1].GetStringValue(),
        args[2].GetBooleanValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.stringArg,
        VsnArgType.booleanArg
      });
    }
  }
}
