using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsPanel : MonoBehaviour {
  public static ActionsPanel instance;

  public ScreenTransitions baseActionsPanel;
  public ScreenTransitions skillsPanel;
  public ScreenTransitions itemsPanel;

  public GameObject[] baseActionButtons;
  public ActionButton[] skillButtons;
  public ActionButton[] itemButtons;

  public GameObject[] baseActionButtonShades;

  public Person currentCharacter;


  public void Initialize(int currentPartyMember) {
    currentCharacter = BattleController.instance.partyMembers[currentPartyMember];
    SetupBaseActionButtons(currentPartyMember);
    SetupCharacterActions(currentPartyMember);
    SetupItemButtons();

    skillsPanel.gameObject.SetActive(false);
    itemsPanel.gameObject.SetActive(false);
    baseActionsPanel.gameObject.SetActive(false);
    baseActionsPanel.ShowPanel();

    PositionPanels(currentPartyMember);
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
      if(i < currentCharacter.skillIds.Length) {
        skillButtons[i].InitializeAsSkill(currentCharacter,
                                             BattleController.instance.GetSkillById(currentCharacter.skillIds[i]));
        skillButtons[i].gameObject.SetActive(true);
      } else {
        skillButtons[i].gameObject.SetActive(false);
      }
    }
  }

  public void SetupItemButtons() {
    List<ItemListing> battleItems = GlobalData.instance.people[0].inventory.GetItemListingsByType(ItemType.battle);
    for(int i = 0; i < itemButtons.Length; i++) {
      if(i < battleItems.Count) {
        itemButtons[i].InitializeAsItem(battleItems[i]);
        itemButtons[i].gameObject.SetActive(true);
      } else {
        itemButtons[i].gameObject.SetActive(false);
      }
    }
  }

  public void PositionPanels(int currentPartyMember) {
    transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f - 320f * currentPartyMember, 0f, 0f);
  }


  public void ClickSkillsPanel() {
    SfxManager.StaticPlayConfirmSfx();
    baseActionsPanel.HidePanel();
    skillsPanel.ShowPanel();
    UIController.instance.ShowHelpMessagePanel("");
  }

  public void ClickItemsPanel() {
    if(!ThereAreItemsAvailable()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    baseActionsPanel.HidePanel();
    itemsPanel.ShowPanel();
    UIController.instance.ShowHelpMessagePanel("");
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
    skillsPanel.HidePanel();
    itemsPanel.HidePanel();
    baseActionsPanel.ShowPanel();
    UIController.instance.selectTargetPanel.SetActive(false);
    UIController.instance.HideHelpMessagePanel();
  }

  public IEnumerator WaitAndShowBaseActionsPanel(ScreenTransitions panel) {
    yield return new WaitForSeconds(0.3f);
    panel.ShowPanel();
  }

  public void EndActionSelect() {
    baseActionsPanel.HidePanel();
    itemsPanel.HidePanel();
    skillsPanel.HidePanel();
  }
}
