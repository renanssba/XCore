using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "for")]
  public class ForCommand : VsnCommand {

    public override void Execute() {
      VsnSaveSystem.SetVariable(args[0].GetReference(), InitialValue());

      if(InitialValue() >= FinalValue()) {
        VsnController.instance.CurrentScriptReader().GotoNextEndfor();
      }
    }

    public void ExecuteStep() {
      int currentValue = VsnSaveSystem.GetIntVariable(args[0].GetReference());
      currentValue = currentValue+1;
      VsnSaveSystem.SetVariable(args[0].GetReference(), currentValue);

      if(currentValue < FinalValue()) {
        Debug.LogWarning("Back to new FOR iteration");
        VsnController.instance.CurrentScriptReader().GotoCommandId(commandIndex+1);
      }
    }

    public int InitialValue() {
      if(args.Length >= 3) {
        return (int)args[1].GetNumberValue();
      }
      return 0;
    }

    public int FinalValue() {
      if(args.Length >= 3) {
        return (int)args[2].GetNumberValue();
      }
      return (int)args[1].GetNumberValue();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[]{
        VsnArgType.referenceArg,
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[]{
        VsnArgType.referenceArg,
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }
  }
}