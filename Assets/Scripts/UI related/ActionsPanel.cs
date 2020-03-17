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
  public GameObject turnIndicator;

  public Vector2[] fourButtonPositions;
  public Vector2[] threeButtonPositions;


  public void Awake() {
    instance = this;
  }

  public void Initialize(int partyMemberId) {
    currentPartyMember = partyMemberId;
    SetupBaseActionButtons(partyMemberId);

    skillsPanel.gameObject.SetActive(false);
    //itemsPanel.gameObject.SetActive(false);
    baseActionsPanel.gameObject.SetActive(true);
    turnIndicator.SetActive(true);
    Utils.SelectUiElement(baseActionButtons[0]);

    PositionPanels(partyMemberId);
  }

  public Person CurrentCharacter() {
    return BattleController.instance.partyMembers[currentPartyMember];
  }


  public void SetupBaseActionButtons(int currentPartyMember) {
    List<GameObject> availableButtons = new List<GameObject>();

    if (BattleController.instance.partyMembers[currentPartyMember].id == 10) {
      baseActionButtons[0].SetActive(true);
      availableButtons.Add(baseActionButtons[0]);
      baseActionButtons[1].SetActive(true);
      availableButtons.Add(baseActionButtons[1]);
      baseActionButtons[2].SetActive(true);
      availableButtons.Add(baseActionButtons[2]);
      baseActionButtons[3].SetActive(false);
    } else {
      baseActionButtons[0].SetActive(false);
      baseActionButtons[1].SetActive(false);
      baseActionButtons[2].SetActive(true);
      availableButtons.Add(baseActionButtons[2]);
      baseActionButtons[3].SetActive(true);
      availableButtons.Add(baseActionButtons[3]);
    }

    if (currentPartyMember == 0) {
      baseActionButtons[4].SetActive(true);
      availableButtons.Add(baseActionButtons[4]);
      baseActionButtons[5].SetActive(false);
    } else {
      baseActionButtons[4].SetActive(false);
      baseActionButtons[5].SetActive(true);
      availableButtons.Add(baseActionButtons[5]);
    }

    SetupBaseActionButtonsPositions(availableButtons);

    baseActionButtonShades[2].SetActive(!ThereAreItemsAvailable());
  }

  public void SetupBaseActionButtonsPositions(List<GameObject> buttons){
    if (buttons.Count == 4)
    {
      for (int i=0; i<buttons.Count; i++)
      {
        buttons[i].GetComponent<RectTransform>().anchoredPosition = fourButtonPositions[i];
      }
    }
    else if(buttons.Count == 3)
    {
      for (int i = 0; i < buttons.Count; i++)
      {
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
  }

  public void SetupItemButtons() {
    List<ItemListing> battleItems = GlobalData.instance.people[0].inventory.GetItemListingsByType(ItemType.battle);
    for(int i = 0; i < skillButtons.Length; i++) {
      if(i < battleItems.Count) {
        skillButtons[i].InitializeAsItem(battleItems[i]);
        skillButtons[i].gameObject.SetActive(true);
      } else {
        skillButtons[i].gameObject.SetActive(false);
      }
    }
  }

  public void PositionPanels(int currentPartyMember) {
    transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f - 320f * currentPartyMember, 0f, 0f);
  }


  public void ClickSkillsPanel() {
    SfxManager.StaticPlayConfirmSfx();
    baseActionsPanel.gameObject.SetActive(false);
    SetupCharacterActions(currentPartyMember);
    skillsPanel.gameObject.SetActive(true);
    Utils.SelectUiElement(skillButtons[0].gameObject);
    UIController.instance.ShowHelpMessagePanel();
  }

  public void ClickItemsPanel() {
    if(!ThereAreItemsAvailable()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    baseActionsPanel.gameObject.SetActive(false);
    SetupItemButtons();
    skillsPanel.gameObject.SetActive(true);
    Utils.SelectUiElement(skillButtons[0].gameObject);
    UIController.instance.ShowHelpMessagePanel();
  }

  //public void ClickDefendButton() {
  //  Debug.LogWarning("clicked DEFEND button");
  //}

  //public void ClickFleeButton() {
  //  Debug.LogWarning("clicked FLEE button");
  //}

  public void ClickBackButton() {
    Debug.LogWarning("clicked BACK button");
    SfxManager.StaticPlayConfirmSfx();

    int currentPlayer = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    currentPlayer -= 1;
    VsnSaveSystem.SetVariable("currentPlayerTurn", currentPlayer);
    Command.ActionInputCommand.WaitForCharacterInput(currentPlayer);
  }


  public bool ThereAreItemsAvailable() {
    List<ItemListing> battleItems = GlobalData.instance.people[0].inventory.GetItemListingsByType(ItemType.battle);
    return battleItems.Count > 0;
  }

  public void ClickBackToBaseActionsPanel() {
    SfxManager.StaticPlayCancelSfx();
    skillsPanel.gameObject.SetActive(false);
    baseActionsPanel.gameObject.SetActive(true);
    Utils.SelectUiElement(baseActionButtons[0]);
    UIController.instance.selectTargetPanel.SetActive(false);
    UIController.instance.HideHelpMessagePanel();
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
