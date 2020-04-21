using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "character")]
  public class CharacterCommand : VsnCommand {

    public override void Execute() {
      string characterLabel = args[0].GetReference();
      string characterFilename = args[1].GetStringValue();

      VsnUIManager.instance.SetCharacterSprite(characterFilename, characterLabel);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg
      });
    }

  }
}