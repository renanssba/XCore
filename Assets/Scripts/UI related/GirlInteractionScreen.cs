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
    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();
    if(currentRelationship.heartLocksOpened < 1) {
      ShowForbiddenMessage("give_gift");
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    HideGirlInteractionScreen();
    Command.GotoCommand.StaticExecute("give_gift");
    VsnController.instance.GotCustomInput();
  }


  public void ClickedDateButton(int dateId) {
    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();
    if(currentRelationship.level < 2 && dateId == 1) {
      ShowForbiddenMessage("date_1");
      return;
    }
    else if(currentRelationship.level < 4 && dateId == 2) {
      ShowForbiddenMessage("date_2");
      return;
    }
    else if(currentRelationship.level < 7 && dateId == 3) {
      ShowForbiddenMessage("date_3");
      return;
    }


    Debug.LogWarning("Clicked date button "+dateId+" to " + currentRelationship.GetBoy().name + " and " + currentRelationship.GetGirl().name);

    VsnSaveSystem.SetVariable("dateId", dateId);
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

  public void ShowForbiddenMessage(string waypointToLoad) {
    HideGirlInteractionScreen();
    Command.GotoCommand.StaticExecute("action_choice");
    Command.GotoScriptCommand.StaticExecute("forbidden_interaction", new VsnArgument[] { new VsnString(waypointToLoad) });
    VsnController.instance.GotCustomInput();
  }
}
