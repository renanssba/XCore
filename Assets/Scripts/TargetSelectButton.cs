using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectButton : MonoBehaviour {

  public int currentTargetId;

  public void ClickedTargetSelectButton() {
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");

    Debug.LogWarning("clicked person: " + BattleController.instance.GetBattlerByTargetId(currentTargetId).name);

    SfxManager.StaticPlayConfirmSfx();

    BattleController.instance.selectedTargetPartyId[currentPlayerTurn] = currentTargetId;
    UIController.instance.HideHelpMessagePanel();
    UIController.instance.selectTargetPanel.SetActive(false);
    ActionsPanel.instance.turnIndicator.SetActive(false);
    TheaterController.instance.SetCharacterChoosingAction(-1);
    VsnController.instance.GotCustomInput();
  }
}
