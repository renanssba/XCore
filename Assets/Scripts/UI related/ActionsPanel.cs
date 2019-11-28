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
      baseActionButtons[2].SetActive(true);
      baseActionButtons[3].SetActive(false);
    } else {
      baseActionButtons[2].SetActive(false);
      baseActionButtons[3].SetActive(true);
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
    Inventory inventory = GlobalData.instance.people[0].inventory;
    for(int i = 0; i < skillButtons.Length; i++) {
      if(i < inventory.itemListings.Count) {
        itemButtons[i].InitializeAsItem(inventory.itemListings[i]);
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
  }

  public void ClickItemsPanel() {
    if(!ThereAreItemsAvailable()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    baseActionsPanel.HidePanel();
    itemsPanel.ShowPanel();
  }

  public void ClickFleeButton() {
    Debug.LogWarning("clicked FLEE button");
  }

  public void ClickBackButton() {
    Debug.LogWarning("clicked BACK button");
    SfxManager.StaticPlayConfirmSfx();

    int currentPlayer = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    currentPlayer -= 1;
    VsnSaveSystem.SetVariable("currentPlayerTurn", currentPlayer);
    Command.ActionInputCommand.WaitForCharacterInput(currentPlayer);
  }


  public bool ThereAreItemsAvailable() {
    return GlobalData.instance.people[0].inventory.itemListings.Count > 0;
  }

  public void ClickBackToBaseActionsPanel() {
    SfxManager.StaticPlayCancelSfx();
    skillsPanel.HidePanel();
    itemsPanel.HidePanel();
    baseActionsPanel.ShowPanel();
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
