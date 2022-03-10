using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsPanel : MonoBehaviour {
  public static ActionsPanel instance;

  public ScreenTransitions baseActionsPanel;
  public ScreenTransitions skillsPanel;

  public GameObject[] baseActionButtons;
  public ActionButton[] skillButtons;
  public int currentPartyMember;

  public GameObject[] baseActionButtonShades;

  public Vector2[] fourButtonPositions;
  public Vector2[] threeButtonPositions;


  public void Awake() {
    instance = this;
  }

  public void Initialize(int partyMemberId) {
    currentPartyMember = partyMemberId;
    SetupBaseActionButtons(partyMemberId);

    PositionPanels(partyMemberId);
  }

  public Pilot CurrentCharacter() {
    return BattleController.instance.partyMembers[currentPartyMember];
  }


  public void SetupBaseActionButtons(int currentPartyMember) {
    List<GameObject> availableButtons = new List<GameObject>();
    Debug.LogWarning("currentPartyMember: "+ currentPartyMember);
    Pilot currentPerson = BattleController.instance.partyMembers[currentPartyMember];

    baseActionButtons[0].GetComponent<ActionButton>().InitializeGeneric(currentPerson);
    baseActionButtons[3].GetComponent<ActionButton>().InitializeGeneric(currentPerson);

    baseActionButtons[0].SetActive(false);
    baseActionButtons[1].SetActive(false);
    baseActionButtons[2].SetActive(true);
    availableButtons.Add(baseActionButtons[2]);
    baseActionButtons[3].SetActive(true);
    availableButtons.Add(baseActionButtons[3]);

    if (currentPartyMember == 0) {
      baseActionButtons[4].SetActive(false);
    } else {
      baseActionButtons[4].SetActive(true);
      availableButtons.Add(baseActionButtons[4]);
    }

    SetupBaseActionButtonsPositions(availableButtons);


    skillsPanel.gameObject.SetActive(false);
    baseActionsPanel.gameObject.SetActive(true);

    Utils.SelectUiElement(availableButtons[0]);

    SetupBaseActionButtonsShades();
  }

  public void SetupBaseActionButtonsShades() {
    Pilot currentPerson = BattleController.instance.partyMembers[currentPartyMember];

    /// itens button
    baseActionButtonShades[1].SetActive(!currentPerson.CanExecuteAction(TurnActionType.useItem) || !ThereAreItemsAvailable());

    /// skills button
    baseActionButtonShades[2].SetActive(!currentPerson.CanExecuteAction(TurnActionType.useSkill));

    /// defend button
    //baseActionButtonShades[3].SetActive(!currentPerson.CanExecuteAction(TurnActionType.defend));
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

  public void SetupCharacterActions(int currentPartyMember) {
    int relationshipId = GlobalData.instance.GetCurrentRelationship().id;

    for(int i = 0; i < skillButtons.Length; i++) {
      if(i < CurrentCharacter().GetActiveSkills(relationshipId).Length) {
        skillButtons[i].InitializeAsSkill(CurrentCharacter(), CurrentCharacter().GetActiveSkills(relationshipId)[i] );
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
        skillButtons[i].InitializeAsItem(CurrentCharacter(), battleItems[i]);
        skillButtons[i].gameObject.SetActive(true);
      } else {
        skillButtons[i].gameObject.SetActive(false);
      }
    }
  }

  public void PositionPanels(int currentPartyMember) {
    transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-320f + 160f * currentPartyMember, 0f, 0f);
  }


  public void ClickSkillsPanel() {
    if(!BattleController.instance.partyMembers[currentPartyMember].CanExecuteAction(TurnActionType.useSkill)) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    baseActionsPanel.gameObject.SetActive(false);
    SetupCharacterActions(currentPartyMember);
    skillsPanel.gameObject.SetActive(true);
    Utils.SelectUiElement(skillButtons[0].gameObject);

    if(VsnSaveSystem.GetBoolVariable("tut_require_click_action_button")) {
      VsnSaveSystem.SetVariable("tut_require_click_action_button", false);
      VsnController.instance.state = ExecutionState.PLAYING;
    }
  }

  public void ClickItemsPanel() {
    if(!ThereAreItemsAvailable() || !BattleController.instance.partyMembers[currentPartyMember].CanExecuteAction(TurnActionType.useItem)) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    baseActionsPanel.gameObject.SetActive(false);
    SetupItemButtons();
    skillsPanel.gameObject.SetActive(true);
    Utils.SelectUiElement(skillButtons[0].gameObject);
  }


  public void ClickBackButton() {
    Debug.LogWarning("clicked BACK button");
    SfxManager.StaticPlayConfirmSfx();

    int currentPlayer = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    currentPlayer -= 1;

    // TODO: consider all cases, considering which previous can input 
    if(BattleController.instance.partyMembers[currentPlayer].TotalStatusEffectPower(StatusConditionEffect.cantAct) >= 1f) {
      currentPlayer -= 1;
    }

    VsnSaveSystem.SetVariable("currentPlayerTurn", currentPlayer);
    Command.ActionInputCommand.WaitForCharacterInput(currentPlayer);
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
    if(BattleController.instance.partyMembers[currentPartyMember].id == 10) {
      Utils.SelectUiElement(baseActionButtons[0]);
    } else {
      Utils.SelectUiElement(baseActionButtons[2]);
    }    
    UIController.instance.selectTargetPanel.SetActive(false);
    UIController.instance.CleanHelpMessagePanel();
  }

  public IEnumerator WaitAndShowActionsPanel(ScreenTransitions panel) {
    yield return new WaitForSeconds(0.3f);
    panel.ShowPanel();
  }

  public void EndActionSelect() {
    baseActionsPanel.gameObject.SetActive(false);
    skillsPanel.gameObject.SetActive(false);
  }
}
