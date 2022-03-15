using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "action_input")]
  public class ActionInputCommand : VsnCommand {

    public override void Execute() {
      if(BattleController.instance.CurrentBattler.GetType() == typeof(Enemy)) {
        BattleController.instance.selectedActionType[BattleController.instance.CurrentBattlerId] = TurnActionType.useSkill;
        return;
      }

      int currentBattler = BattleController.instance.CurrentBattlerId;
      bool waitForInput = true;

      if(args.Length > 0) {
        waitForInput = args[0].GetBooleanValue();
      }

      /// skip input if character cant act
      if(BattleController.instance.CurrentBattler.TotalStatusEffectPower(StatusConditionEffect.cantAct) > 0f) {
        BattleController.instance.selectedActionType[BattleController.instance.CurrentBattlerId] = TurnActionType.idle;
        return;
      }

      WaitForCharacterInput(currentBattler, waitForInput);
    }

    public static void WaitForCharacterInput(int currentPlayer, bool waitForInput = true) {
      Debug.LogWarning("action_input calling for character input");

      TheaterController.instance.SetCharacterChoosingAction(currentPlayer);
      UIController.instance.SetupCurrentCharacterUi(currentPlayer);
      UIController.instance.actionsPanel.Initialize();
      if(waitForInput) {
        VsnController.instance.state = ExecutionState.WAITINGCUSTOMINPUT;
      }      
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);

      signatures.Add(new VsnArgType[] {
        VsnArgType.booleanArg
      });
    }
  }
}