using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "action_input")]
  public class ActionInputCommand : VsnCommand {

    public override void Execute() {
      int currentPlayer = (int)args[0].GetNumberValue();
      bool waitForInput = true;

      if(args.Length > 1) {
        waitForInput = args[1].GetBooleanValue();
      }
      WaitForCharacterInput(currentPlayer, waitForInput);
    }

    public static void WaitForCharacterInput(int currentPlayer, bool waitForInput = true) {
      TheaterController.instance.SetCharacterChoosingAction(currentPlayer);
      UIController.instance.SetupCurrentCharacterUi(currentPlayer);
      UIController.instance.actionsPanel.Initialize(currentPlayer);
      if(waitForInput) {
        VsnController.instance.state = ExecutionState.WAITINGCUSTOMINPUT;
      }      
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg,
        VsnArgType.booleanArg
      });
    }
  }
}