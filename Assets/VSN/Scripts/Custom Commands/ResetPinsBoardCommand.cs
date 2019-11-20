using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "reset_pins_board")]
  public class ResetPinsBoardCommand : VsnCommand {

    public override void Execute() {
      UIController.instance.ResetPinsBoard(args[0].GetStringValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}