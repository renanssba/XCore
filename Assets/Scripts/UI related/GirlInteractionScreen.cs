using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GirlInteractionScreen : MonoBehaviour {


  public ScreenTransitions girlInteractionPanel;
  public ScreenTransitions girlInteractionButtonsPanel;

  public CoupleEntry coupleEntry;

  public ScreenTransitions dateSelectPanel;
  public GameObject[] girlInteractionButtons;
  public GameObject[] dateSelectButtons;

  public Image[] girlInteractionImage;
  public Image[] boyInteractionImage;



  public void ShowGirlInteractionScreen() {
    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();
    if(currentRelationship == null) {
      Debug.LogError("Error getting current relationship");
    }

    UIController.instance.SetScreenLayout("girl_interaction_screen");

    boyInteractionImage[0].sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[0].id, "base");
    girlInteractionImage[0].sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[1].id, "base");
    if(VsnSaveSystem.GetIntVariable("daytime") == 0) {
      boyInteractionImage[1].sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[0].id, "school");
      girlInteractionImage[1].sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[1].id, "school");
    } else {
      boyInteractionImage[1].sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[0].id, "casual");
      girlInteractionImage[1].sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[1].id, "casual");
    }
    UIController.instance.relationshipUpAnimationCard.Initialize(currentRelationship);

    coupleEntry.Initialize(currentRelationship);
    girlInteractionPanel.ShowPanel();
    girlInteractionButtonsPanel.canvasGroup.alpha = 0f;
    girlInteractionButtonsPanel.ShowPanel();
    Utils.SelectUiElement(girlInteractionButtons[0]);
    dateSelectPanel.gameObject.SetActive(false);
  }

  public void HideGirlInteractionScreen() {
    girlInteractionPanel.HidePanel();
  }



  public void ShowDateSelectionPanel() {
    girlInteractionButtonsPanel.HidePanel();
    dateSelectPanel.ShowPanel();
    Utils.SelectUiElement(dateSelectButtons[0]);
  }

  public void ExitDateSelectionPanel() {
    girlInteractionButtonsPanel.ShowPanel();
    Utils.SelectUiElement(girlInteractionButtons[0]);
    dateSelectPanel.HidePanel();
  }

  public void ClickConversationButton() {
    SfxManager.StaticPlayConfirmSfx();
    HideGirlInteractionScreen();
    Command.GotoCommand.StaticExecute("conversation");
    VsnController.instance.GotCustomInput();
  }

  public void ClickGiveGiftButton() {
    SfxManager.StaticPlayConfirmSfx();
    HideGirlInteractionScreen();
    Command.GotoCommand.StaticExecute("give_gift");
    VsnController.instance.GotCustomInput();
  }


  public void ClickedDateButton(int dateId) {

    /// IF CANNOT GO TO DATE RIGHT NOW
    //if(VsnSaveSystem.GetIntVariable("daytime") == 0) {
    //  SfxManager.StaticPlayForbbidenSfx();
    //  return;
    //}

    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();

    Debug.LogWarning("Clicked date button to " + currentRelationship.GetBoy().name + " and " + currentRelationship.GetGirl().name);

    BattleController.instance.StartBattle(currentRelationship.GetBoy(), currentRelationship.GetGirl(), dateId);

    SfxManager.StaticPlayBigConfirmSfx();
    HideGirlInteractionScreen();
    Command.EndScriptCommand.StaticExecute(new VsnArgument[0]);
    VsnController.instance.GotCustomInput();
    //VsnController.instance.StartVSN("date");
    VsnArgument[] args = new VsnArgument[1];
    args[0] = new VsnString("date_intro_" + dateId);
    switch(currentRelationship.GetGirl().id) {
      case 1:
        VsnController.instance.StartVSN("cap1_conversa_ana", args);
        break;
      case 2:
        VsnController.instance.StartVSN("cap1_conversa_beatrice", args);
        break;
      case 3:
        VsnController.instance.StartVSN("cap1_conversa_clara", args);
        break;
    }
  }
}
