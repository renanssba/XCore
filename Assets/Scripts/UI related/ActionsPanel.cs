using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsPanel : MonoBehaviour {
  public static ActionsPanel instance;

  [Header("- Button Panels -")]
  public Panel baseActionsPanel;
  public Panel skillsPanel;

  [Header("- Base Action Buttons -")]
  public GameObject[] baseActionButtons;
  public ActionButton[] skillButtons;

  public GameObject[] baseActionButtonShades;

  public Vector2[] fourButtonPositions;
  public Vector2[] threeButtonPositions;


  public int CurrentBattlerId {
    get { return BattleController.instance.CurrentBattlerId; }
  }

  public Pilot CurrentPilot {
    get { return (Pilot)BattleController.instance.CurrentBattler; }
  }


  public void Awake() {
    instance = this;
  }

  public void Initialize() {
    SetupBaseActionButtons();
    PositionPanels();
  }


  public void SetupBaseActionButtons() {
    List<GameObject> availableButtons = new List<GameObject>();

    baseActionButtons[0].GetComponent<ActionButton>().InitializeGeneric(CurrentPilot);
    baseActionButtons[3].GetComponent<ActionButton>().InitializeGeneric(CurrentPilot);

    baseActionButtons[0].SetActive(false);
    baseActionButtons[1].SetActive(false);
    baseActionButtons[2].SetActive(true);
    availableButtons.Add(baseActionButtons[2]);
    baseActionButtons[3].SetActive(true);
    availableButtons.Add(baseActionButtons[3]);
    SetupBaseActionButtonsPositions(availableButtons);

    skillsPanel.gameObject.SetActive(false);
    baseActionsPanel.gameObject.SetActive(true);

    Utils.SelectUiElement(availableButtons[0]);

    SetupBaseActionButtonsShades();
  }

  public void SetupBaseActionButtonsShades() {
    Battler currentPilot = CurrentPilot;

    /// itens button
    baseActionButtonShades[1].SetActive(!currentPilot.CanExecuteAction(TurnActionType.useItem) || !ThereAreItemsAvailable());

    /// skills button
    baseActionButtonShades[2].SetActive(!currentPilot.CanExecuteAction(TurnActionType.useSkill));
  }


  public void SetupBaseActionButtonsPositions(List<GameObject> buttons){
    if(buttons.Count == 4) {
      for(int i=0; i<buttons.Count; i++) {
        buttons[i].GetComponent<RectTransform>().anchoredPosition = fourButtonPositions[i];
      }
    } else if(buttons.Count == 3) {
      for(int i = 0; i < buttons.Count; i++) {
        buttons[i].GetComponent<RectTransform>().anchoredPosition = threeButtonPositions[i];
      }
    }
  }

  public void SetupCharacterActions() {
    int relationshipId = GlobalData.instance.GetCurrentRelationship().id;

    for(int i = 0; i < skillButtons.Length; i++) {
      if(i < CurrentPilot.GetActiveSkills(relationshipId).Length) {
        skillButtons[i].InitializeAsSkill(CurrentPilot, CurrentPilot.GetActiveSkills(relationshipId)[i] );
        skillButtons[i].gameObject.SetActive(true);
      } else {
        skillButtons[i].gameObject.SetActive(false);
      }
    }
    skillButtons[1].gameObject.SetActive(false);
    skillButtons[2].gameObject.SetActive(false);
  }

  public void SetupItemButtons() {
    List<ItemListing> battleItems = GlobalData.instance.pilots[0].inventory.GetItemListingsByType(ItemType.battle);
    for(int i = 0; i < skillButtons.Length; i++) {
      if(i < battleItems.Count) {
        skillButtons[i].InitializeAsItem(CurrentPilot, battleItems[i]);
        skillButtons[i].gameObject.SetActive(true);
      } else {
        skillButtons[i].gameObject.SetActive(false);
      }
    }
  }

  public void PositionPanels() {
    SkillTarget currentPosInParty = BattleController.instance.CurrentPartyPos;
    transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-320f + 160f * (int)currentPosInParty, 0f, 0f);
  }


  public void ClickSkillsPanel() {
    if(!CurrentPilot.CanExecuteAction(TurnActionType.useSkill)) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    baseActionsPanel.gameObject.SetActive(false);
    SetupCharacterActions();
    skillsPanel.gameObject.SetActive(true);
    Utils.SelectUiElement(skillButtons[0].gameObject);

    if(VsnSaveSystem.GetBoolVariable("tut_require_click_action_button")) {
      VsnSaveSystem.SetVariable("tut_require_click_action_button", false);
      VsnController.instance.state = ExecutionState.PLAYING;
    }
  }

  public void ClickItemsPanel() {
    if(!ThereAreItemsAvailable() || !CurrentPilot.CanExecuteAction(TurnActionType.useItem)) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    baseActionsPanel.gameObject.SetActive(false);
    SetupItemButtons();
    skillsPanel.gameObject.SetActive(true);
    Utils.SelectUiElement(skillButtons[0].gameObject);
  }


  public bool ThereAreItemsAvailable() {
    List<ItemListing> battleItems = GlobalData.instance.pilots[0].inventory.GetItemListingsByType(ItemType.battle);
    return battleItems.Count > 0;
  }

  public void ClickBackToBaseActionsPanel() {
    if(VsnSaveSystem.GetBoolVariable("tut_require_click_guts")) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    if(VsnSaveSystem.GetBoolVariable("tut_cant_cancel_target") == true) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayCancelSfx();
    skillsPanel.gameObject.SetActive(false);
    baseActionsPanel.gameObject.SetActive(true);
    Utils.SelectUiElement(baseActionButtons[0]);
    UIController.instance.selectTargetPanel.SetActive(false);
  }

  public IEnumerator WaitAndShowActionsPanel(Panel panel) {
    yield return new WaitForSeconds(0.3f);
    panel.ShowPanel();
  }

  public void EndActionSelect() {
    baseActionsPanel.gameObject.SetActive(false);
    skillsPanel.gameObject.SetActive(false);
  }
}
