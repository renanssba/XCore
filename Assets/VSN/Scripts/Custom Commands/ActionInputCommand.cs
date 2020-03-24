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

      if(currentPlayer == 2 && BattleController.instance.currentStealth <= 0f) {
        SfxManager.StaticPlayCancelSfx();
        BattleController.instance.selectedActionType[2] = TurnActionType.idle;
        BattleController.instance.FinishSelectingCharacterAction();
        return;
      }

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