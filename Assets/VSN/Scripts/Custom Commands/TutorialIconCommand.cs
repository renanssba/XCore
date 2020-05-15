using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "tutorial_icon")]
  public class TutorialIconCommand : VsnCommand {

    public override void Execute() {
      if(args[1].GetBooleanValue() == true) {
        Tut_Controller.instance.StartAnimation(args[0].GetStringValue());
        ActivateExtraMaterial(args[0].GetStringValue());
      } else {
        Tut_Controller.instance.StopAnimation(args[0].GetStringValue());
      }
    }

    public void ActivateExtraMaterial(string tutorialIcon) {
      //switch(tutorialIcon) {
      //  case "show_action_buttons":
      //    ActionInputCommand.WaitForCharacterInput(0);
      //    VsnController.instance.state = ExecutionState.PLAYING;
      //    break;
      //}
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.booleanArg
      });
    }
  }
}