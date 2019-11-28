using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_current_character_turn")]
  public class SetCurrentCharacterTurnCommand : VsnCommand {

    public override void Execute() {
      int currentCharacter = (int)args[0].GetNumberValue();
      UIController.instance.SetupCurrentCharacterUi(currentCharacter);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}