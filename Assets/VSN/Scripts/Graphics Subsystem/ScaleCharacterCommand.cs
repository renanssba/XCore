using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "char_scale")]
  public class ScaleCharacterCommand : VsnCommand {

    public override void Execute() {
      string characterLabel;
      float characterScale;
      float duration = 0;

      characterLabel = args[0].GetReference();
      characterScale = args[1].GetNumberValue();
      if(args.Length >= 3) {
        duration = args[2].GetNumberValue();
      }

      VsnUIManager.instance.ScaleCharacter(characterLabel, characterScale, duration);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg,
        VsnArgType.numberArg
      });
    }

  }
}