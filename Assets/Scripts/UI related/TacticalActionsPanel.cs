using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticalActionsPanel : Panel {

  [Header("- Action Buttons -")]
  public TacticalActionButton[] actionButtons;


  public override void PreShowPanel() {
    TacticalActionButton firstAvailable = null;
    int availableButtons = 0;

    foreach(TacticalActionButton skillButton in actionButtons) {
      if(TacticalActionButton.ActionCanBeUsed(skillButton.action)) {
        skillButton.UpdateUI();
        skillButton.gameObject.SetActive(true);
        if(firstAvailable == null) {
          firstAvailable = skillButton;
        }
        availableButtons++;
      } else {
        skillButton.gameObject.SetActive(false);
      }
    }

    if(availableButtons <= 1) {
      GameController.instance.EndTurn();
    } else {
      gameObject.SetActive(true);
      Utils.SelectUiElement(firstAvailable.gameObject);
    }    
  }

}
