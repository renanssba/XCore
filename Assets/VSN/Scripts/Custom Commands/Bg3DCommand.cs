using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "bg3d")]
  public class Bg3DCommand : VsnCommand {

    public override void Execute() {
      TheaterController.instance.SetLocation(args[0].GetStringValue());
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}