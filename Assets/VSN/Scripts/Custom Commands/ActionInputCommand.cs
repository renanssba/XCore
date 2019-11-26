using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "action_input")]
  public class ActionInputCommand : VsnCommand {

    public override void Execute() {
      SetupCurrentCharacter();

      UIController.instance.actionsPanel.ShowPanel();
      VsnController.instance.state = ExecutionState.WAITINGCUSTOMINPUT;
    }

    public void SetupCurrentCharacter() {
      int currentCharacter = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
      int dateLength = BattleController.instance.partyMembers.Length;
      UIController.instance.SetupCurrentCharacterUi(currentCharacter);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}