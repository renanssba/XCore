using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectButton : MonoBehaviour {

  public SkillTarget currentTargetId;

  public void ClickedTargetSelectButton() {
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");

    Debug.LogWarning("clicked person: " + BattleController.instance.GetBattlerByTargetId(currentTargetId).GetName());

    if(VsnSaveSystem.GetBoolVariable("tut_cant_cancel_target")) {
      VsnSaveSystem.SetVariable("tut_cant_cancel_target", false);
    }    

    SfxManager.StaticPlayConfirmSfx();

    BattleController.instance.selectedTargetPartyId[currentPlayerTurn] = currentTargetId;
    UIController.instance.CleanHelpMessagePanel();
    UIController.instance.selectTargetPanel.SetActive(false);
    TheaterController.instance.SetCharacterChoosingAction(-1);
    VsnController.instance.GotCustomInput();
  }
}
