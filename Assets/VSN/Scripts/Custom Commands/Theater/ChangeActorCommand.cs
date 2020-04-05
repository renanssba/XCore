using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "change_actor")]
  public class ChangeActorCommand : VsnCommand {

    public override void Execute() {
      TheaterController.instance.ChangeActor(args[0].GetStringValue(), args[1].GetStringValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.stringArg
      });
    }
  }
}
