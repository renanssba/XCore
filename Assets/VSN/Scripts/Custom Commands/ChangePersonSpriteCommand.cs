using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "change_person_sprite")]
  public class ChangePersonSpriteCommand : VsnCommand {

    public override void Execute() {
      GlobalData.instance.people[(int)args[0].GetNumberValue()].faceId = (int)args[1].GetNumberValue();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}