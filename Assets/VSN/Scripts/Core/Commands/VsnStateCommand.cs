using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "vsn_state")]
  public class VsnStateCommand : VsnCommand {

    public override void Execute() {
      ExecutionState state = GetExecutionStateByString(args[0].GetStringValue());
      if(state != ExecutionState.NumberOfExecutionStates) {
        VsnController.instance.state = state;
      }      
    }


    public static ExecutionState GetExecutionStateByString(string state) {
      for(int i = 0; i <= (int)ExecutionState.NumberOfExecutionStates; i++) {
        if(((ExecutionState)i).ToString() == state) {
          return (ExecutionState)i;
        }
      }
      return ExecutionState.NumberOfExecutionStates;
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}