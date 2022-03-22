using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TacticalUIController : MonoBehaviour {
  public static TacticalUIController instance;


  [Header("- Battler Info Panels -")]
  public BattlerInfoPanel currentBattlerInfoPanel;
  public BattlerInfoPanel selectedBattlerInfoPanel;

  [Header("- Tactical Actions Panel -")]
  public TacticalActionsPanel tacticalActionsPanel;

  //[Header("- Setup Phase Panel -")]
  //public Panel setupPhasePanel;
  //public Image heroToPositionImage;
  //public TextMeshProUGUI setupPhaseTitleText;

  [Header("- Engagement Confirm Panel -")]
  public Panel engagementConfirmPanel;
  public TextMeshProUGUI engagementConfirmText;


  [Header("- Cancel Button -")]
  public GameObject cancelButton;

  [Header("- Map Input Button -")]
  public GameObject clickMapButton;


  void Awake() {
    instance = this;
  }

  public void EnterBattlePhase() {
    SelectCharacterByCursor(null);
    clickMapButton.SetActive(false);
  }

  public void EndBattlePhase() {
    clickMapButton.SetActive(true);
  }

  public void ShowCurrentCharacterInfo(Battler character) {
    currentBattlerInfoPanel.SetSelectedUnit(character);
  }

  public void SelectCharacterByCursor(Battler selectedUnit) {
    selectedBattlerInfoPanel.SetSelectedUnit(selectedUnit);
  }


  public void ShowActionsMenu() {
    tacticalActionsPanel.ShowPanel();
  }

  public void HideActionsMenu() {
    tacticalActionsPanel.gameObject.SetActive(false);
  }


  public void ShowEngagementConfirmPanel() {
    //skillConfirmText.text = Lean.Localization.LeanLocalization.GetTranslationText("actions/confirm_use").
    //  Replace("XXXXX", GameController.instance.CurrentSkill.PrintableName());
    engagementConfirmPanel.ShowPanel();
  }

  public void HideSkillConfirmPanel() {
    engagementConfirmPanel.gameObject.SetActive(false);
  }


  //public void ShowSetupPanel() {
  //  setupPhasePanel.ShowPanel();
  //}

  //public void HideSetupPanel() {
  //  setupPhasePanel.gameObject.SetActive(false);
  //}

  //public void UpdateSetupPhase() {
  //  heroToPositionImage.sprite = ResourcesManager.instance.characterPortraits[(int)GameController.instance.characterIdToPosition];
  //  setupPhaseTitleText.text = Lean.Localization.LeanLocalization.GetTranslationText("setup_phase/title") + " (" +
  //    GameController.instance.allCharacters.CountByTeam(CombatTeam.player) + "/" + GameController.instance.heroesAllowed + ")";
  //}


  public void ClickedUndoMovementButton() {
    GameController.instance.RevertMovement();
  }

  public void ClickedEngageButton() {
    GameController.instance.SetGameState(GameState.chooseEngagement);
  }

  public void ClickedManeuverButton() {
    //GameController.instance.SetGameState(GameState.chooseMovement);
  }

  public void ClickedEndTurnButton() {
    GameController.instance.EndTurn();
  }
}
