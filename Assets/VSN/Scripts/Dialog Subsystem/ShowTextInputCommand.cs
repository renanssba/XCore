using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "show_text_input")]
  public class ShowTextInputCommand : VsnCommand {

    public override void Execute() {
      VsnController.instance.state = ExecutionState.WAITINGTEXTINPUT;

      if(args[0].GetBooleanValue() == true) {
        if(args.Length > 1) {
          VsnUIManager.instance.SetTextInputDescription(args[1].GetStringValue());
        }
        if(args.Length > 2) {
          VsnUIManager.instance.SetTextInputCharacterLimit((int)args[2].GetNumberValue());
        } else {
          VsnUIManager.instance.SetTextInputCharacterLimit(0);
        }
        VsnUIManager.instance.ShowTextInput(true);
      } else if(args[0].GetBooleanValue() == false) {
        VsnUIManager.instance.ShowTextInput(false);
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg,
        VsnArgType.stringArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg,
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}