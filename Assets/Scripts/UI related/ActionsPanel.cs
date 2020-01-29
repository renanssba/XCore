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


  public void Initialize(int partyMemberId) {
    currentPartyMember = partyMemberId;
    SetupBaseActionButtons(partyMemberId);

    skillsPanel.gameObject.SetActive(false);
    //itemsPanel.gameObject.SetActive(false);
    baseActionsPanel.gameObject.SetActive(true);
    Utils.SelectUiElement(baseActionButtons[0]);

    PositionPanels(partyMemberId);
  }

  public Person CurrentCharacter() {
    return BattleController.instance.partyMembers[currentPartyMember];
  }


  public void SetupBaseActionButtons(int currentPartyMember) {
    if(currentPartyMember == 0) {
      baseActionButtons[3].SetActive(true);
      baseActionButtons[4].SetActive(false);
    } else {
      baseActionButtons[3].SetActive(false);
      baseActionButtons[4].SetActive(true);
    }

    baseActionButtonShades[1].SetActive(!ThereAreItemsAvailable());
  }

  public void SetupCharacterActions(int currentPartyMember) {
    for(int i = 0; i < skillButtons.Length; i++) {
      if(i < CurrentCharacter().skillIds.Length) {
        skillButtons[i].InitializeAsSkill(CurrentCharacter(),
                                          BattleController.instance.GetSkillById(CurrentCharacter().skillIds[i]));
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
