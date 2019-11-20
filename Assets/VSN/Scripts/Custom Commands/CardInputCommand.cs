using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "action_input")]
  public class ActionInputCommand : VsnCommand {

    public override void Execute() {
      // instantiate available actions
      UIController.instance.actionsPanel.ShowPanel();
      VsnController.instance.state = ExecutionState.WAITINGCUSTOMINPUT;
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}