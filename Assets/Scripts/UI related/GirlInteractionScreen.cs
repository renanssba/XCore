using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GirlInteractionScreen : MonoBehaviour {

  public ScreenTransitions girlInteractionPanel;
  public ScreenTransitions girlInteractionButtonsPanel;

  public RelationshipCard relationshipCard;

  public ScreenTransitions dateSelectPanel;
  public Button[] girlInteractionButtons;
  public Button[] dateSelectButtons;


  public void ShowGirlInteractionScreen() {
    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();
    if(currentRelationship == null) {
      Debug.LogError("Error getting current relationship");
    }

    //UIController.instance.SetScreenLayout("girl_interaction_screen");
    UIController.instance.relationshipUpAnimationCard.Initialize(currentRelationship);
    VsnController.instance.BlockExternalInput(false);

    relationshipCard.Initialize(currentRelationship);
    SetButtonsGraphics();
    girlInteractionPanel.ShowPanel();
    girlInteractionButtonsPanel.canvasGroup.alpha = 0f;
    girlInteractionButtonsPanel.ShowPanel();
    Utils.SelectUiElement(girlInteractionButtons[0].gameObject);
    dateSelectPanel.gameObject.SetActive(false);
  }

  public void SetButtonsGraphics() {
    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();

    if(currentRelationship.heartLocksOpened == 0) {
      Utils.SetButtonDisabledGraphics(girlInteractionButtons[0]);
    } else {
      Utils.SetButtonEnabledGraphics(girlInteractionButtons[0]);
    }

    if(currentRelationship.level < 2) {
      Utils.SetButtonDisabledGraphics(girlInteractionButtons[2]);
    } else {
      Utils.SetButtonEnabledGraphics(girlInteractionButtons[2]);
    }

    if(currentRelationship.level < 4) {
      Utils.SetButtonDisabledGraphics(dateSelectButtons[1]);
    } else {
      Utils.SetButtonEnabledGraphics(dateSelectButtons[1]);
    }

    if(currentRelationship.level < 7) {
      Utils.SetButtonDisabledGraphics(dateSelectButtons[2]);
    } else {
      Utils.SetButtonEnabledGraphics(dateSelectButtons[2]);
    }
  }

  public void HideGirlInteractionScreen() {
    girlInteractionPanel.HidePanel();
  }



  public void ShowDateSelectionPanel() {
    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();

    if(currentRelationship.level < 2) {
      ShowForbiddenMessage("date_1");
      return;
    }

    girlInteractionButtonsPanel.HidePanel();
    dateSelectPanel.ShowPanel();
    Utils.SelectUiElement(dateSelectButtons[0].gameObject);
  }

  public void ExitDateSelectionPanel() {
    girlInteractionButtonsPanel.ShowPanel();
    Utils.SelectUiElement(girlInteractionButtons[0].gameObject);
    dateSelectPanel.HidePanel();
  }

  public void ClickConversationButton() {
    VsnController.instance.BlockExternalInput(true);
    SfxManager.StaticPlayConfirmSfx();
    HideGirlInteractionScreen();
    Command.GotoCommand.StaticExecute("conversation");
    VsnController.instance.GotCustomInput();
  }

  public void ClickGiveGiftButton() {
    if(relationshipCard.relationship.heartLocksOpened < 1) {
      ShowForbiddenMessage("give_gift");
      return;
    }

    VsnController.instance.BlockExternalInput(true);
    SfxManager.StaticPlayConfirmSfx();
    HideGirlInteractionScreen();
    Command.GotoCommand.StaticExecute("give_gift");
    VsnController.instance.GotCustomInput();
  }


  public void ClickedDateButton(int dateId) {
    Relationship currentRelationship = relationshipCard.relationship;
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

    VsnController.instance.BlockExternalInput(true);
    Debug.LogWarning("Clicked date button "+dateId+" to " + currentRelationship.GetBoy().GetName() + " and " + currentRelationship.GetGirl().GetName());
    SfxManager.StaticPlayBigConfirmSfx();
    VsnSaveSystem.SetVariable("dateId", dateId);

    BattleController.instance.SetupBattleStart(dateId);

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
