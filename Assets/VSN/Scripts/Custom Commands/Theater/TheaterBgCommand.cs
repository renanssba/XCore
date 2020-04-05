using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "theater_bg")]
  public class TheaterBgCommand : VsnCommand {

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