using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "character_action")]
  public class CharacterActionCommand : VsnCommand {

    public override void Execute() {
      BattleController.instance.CharacterTurn((SkillTarget)(int)args[0].GetNumberValue());
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}