using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "actor_animator_parameter")]
  public class ActorAnimatorParameterCommand : VsnCommand {

    public override void Execute() {
      Actor2D actor = TheaterController.instance.GetActorByString(args[0].GetReference());

      if(args.Length >= 3) {
        Debug.LogWarning("arg0: "+ args[0].GetReference()+", arg1: "+ args[1].GetStringValue()+", arg2: "+ args[2].GetBooleanValue());
        actor.SetAnimationParameter(args[1].GetStringValue(), args[2].GetBooleanValue());
      } else if(args.Length == 2) {
        actor.SetAnimationTrigger(args[1].GetStringValue());
      }
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg,
        VsnArgType.booleanArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg
      });
    }
  }
}
