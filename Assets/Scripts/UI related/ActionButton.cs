using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour {

  public TextMeshProUGUI nameText;
  public TextMeshProUGUI mpCostText;
  public Image iconImage;
  public Image improvementIconImage;


  public void Initialize() {

  }


  public void ClickedActionButton(int actionClickedPosition) {
    //switch(actionClickedPosition) {
    //  case 1:
    //    break;
    //  case 2:
    //    break;
    //  case 3:
    //    break;
    //}
    VsnSaveSystem.SetVariable("selected_attribute", actionClickedPosition);
    VsnSaveSystem.SetVariable("attribute_effective_level", GlobalData.instance.CurrentCharacterAttribute(actionClickedPosition));

    UIController.instance.actionsPanel.HidePanel();

    VsnController.instance.GotCustomInput();
  }
}
