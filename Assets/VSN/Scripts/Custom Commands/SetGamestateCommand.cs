using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_gamestate")]
  public class SetGamestateCommand : VsnCommand {

    public override void Execute() {
      GameState state = (GameState)System.Enum.Parse(typeof(GameState), args[0].GetStringValue());
      GameController.instance.SetGameState(state);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
