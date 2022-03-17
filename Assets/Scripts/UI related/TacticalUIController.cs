using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TacticalUIController : MonoBehaviour {
  public static TacticalUIController instance;


  [Header("- Tactical Battler INFO Panel -")]
  public BattlerInfoPanel battlerInfoPanel;

  [Header("- Tactical Actions Panel -")]
  public Panel tacticalActionsPanel;

  //[Header("- Setup Phase Panel -")]
  //public Panel setupPhasePanel;
  //public Image heroToPositionImage;
  //public TextMeshProUGUI setupPhaseTitleText;

  [Header("- Confirm Skill Use Panel -")]
  public Panel skillConfirmPanel;
  public TextMeshProUGUI skillConfirmText;


  [Header("- Cancel Button -")]
  public GameObject cancelButton;

  [Header("- Map Input Button -")]
  public GameObject clickMapButton;


  void Awake() {
    instance = this;
  }

  public void EnterBattlePhase() {
    Select(null);
    clickMapButton.SetActive(false);
  }

  public void EndBattlePhase() {
    clickMapButton.SetActive(true);
  }

  public void Select(CharacterToken character) {
    if(character == null) {
      battlerInfoPanel.canvasGroup.alpha = 0f;
      return;
    }

    battlerInfoPanel.canvasGroup.alpha = 1f;
    battlerInfoPanel.Initialize(character.battler);
    battlerInfoPanel.SkipHpBarAnimation();
  }


  public void ShowActionsMenu() {
    tacticalActionsPanel.ShowPanel();
  }

  public void HideActionsMenu() {
    tacticalActionsPanel.gameObject.SetActive(false);
  }


  public void ShowSkillConfirmPanel() {
    //skillConfirmText.text = Lean.Localization.LeanLocalization.GetTranslationText("actions/confirm_use").
    //  Replace("XXXXX", GameController.instance.CurrentSkill.PrintableName());
    skillConfirmPanel.ShowPanel();
  }

  public void HideSkillConfirmPanel() {
    skillConfirmPanel.gameObject.SetActive(false);
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
}
