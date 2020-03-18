using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectButton : MonoBehaviour {

  public int currentTargetId;

  public void ClickedTargetSelectButton() {
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");

    Debug.LogWarning("clicked person: " + BattleController.instance.partyMembers[currentTargetId].name);

    BattleController.instance.selectedTargetPartyId[currentPlayerTurn] = currentTargetId;
    UIController.instance.HideHelpMessagePanel();
    UIController.instance.selectTargetPanel.SetActive(false);
    ActionsPanel.instance.turnIndicator.SetActive(false);
    VsnController.instance.GotCustomInput();
  }
}
