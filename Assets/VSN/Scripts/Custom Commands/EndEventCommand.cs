using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "end_event")]
  public class EndEventCommand : VsnCommand {

    public override void Execute() {
      Event currentEvent = GameController.instance.GetCurrentEvent();
      int result = VsnSaveSystem.GetIntVariable("resolution");

      GameController.instance.currentDateEvent++;
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
