using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "activate_object")]
  public class ActivateObjectCommand : VsnCommand {

    public override void Execute() {
      Transform t = VsnController.instance.transform.parent.Find(args[0].GetStringValue());
      t.gameObject.SetActive(args[1].GetBooleanValue());
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.booleanArg
      });
    }
  }
}