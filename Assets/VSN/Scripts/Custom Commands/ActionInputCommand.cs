using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "action_input")]
  public class ActionInputCommand : VsnCommand {

    public override void Execute() {
      int currentPlayer = (int)args[0].GetNumberValue();
      WaitForCharacterInput(currentPlayer);
    }

    public static void WaitForCharacterInput(int currentPlayer) {
      TheaterController.instance.SetCharacterChoosingAction(currentPlayer);
      UIController.instance.SetupCurrentCharacterUi(currentPlayer);
      UIController.instance.actionsPanel.Initialize(currentPlayer);
      VsnController.instance.state = ExecutionState.WAITINGCUSTOMINPUT;
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}