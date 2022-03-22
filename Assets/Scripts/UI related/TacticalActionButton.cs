using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum TacticalAction {
  repair,
  engage,

  longShot,
  useItem,
  tradeItems,

  wait,
}


public class TacticalActionButton : MonoBehaviour {

  [Header("- Data -")]
  public TacticalAction action;

  [Header("- Visuals -")]
  public TextMeshProUGUI actionNameText;
  public Sprite[] actionSprites;
  public GameObject unusableShade;


  public void UpdateUI() {
    unusableShade.SetActive(!ActionCanBeUsed(action));

    //switch(action) {
    //  case Action.wait:
    //    actionNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("tactical/end");
    //    break;
    //  case Action.useItem:
    //    actionNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("tactical/use_item");
    //    break;
    //  case Action.maneuvers:
    //    actionNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("tactical/maneuver");
    //    break;
    //}
  }


  public static bool ActionCanBeUsed(TacticalAction action) {
    List<Vector2Int> posList;
    switch(action) {
      case TacticalAction.engage:
        posList = BoardController.instance.CalculateEngagementTargets(GameController.instance.CurrentCharacter);
        return posList.Count > 0;
      case TacticalAction.tradeItems:
        return false;
        posList = BoardController.instance.CalculateTradeableTiles(GameController.instance.CurrentCharacter);
        return posList.Count > 0;
      case TacticalAction.repair:
        posList = BoardController.instance.CalculateRepairTiles(GameController.instance.CurrentCharacter);
        return posList.Count > 0;
      case TacticalAction.wait:
        return true;
      default:
        return false;
    }
    return true;
  }



  public void SelectByMouse() {
    if(EventSystem.current.currentSelectedGameObject != gameObject) {
      Utils.SelectUiElement(gameObject);
      SfxManager.StaticPlaySelectSfx();
    }
  }


  public void Clicked() {
    if(GameController.instance.InputOrigin == InputOrigin.IA) {
      return;
    }
    if(!ActionCanBeUsed(action)) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    SfxManager.StaticPlayConfirmSfx();
    ExecuteAction(action);
  }

  public static void ExecuteAction(TacticalAction action) {
    switch(action) {
      case TacticalAction.engage:
        TacticalUIController.instance.ClickedEngageButton();
        break;
      case TacticalAction.wait:
      default:
        GameController.instance.EndTurn();
        break;
    }
  }

}
