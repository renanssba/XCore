using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour {
  public static UIController instance;

  [Header("- UI and Daytime -")]
  public Panel uiControllerPanel;
  public TextMeshProUGUI dayText;
  public Image daytimeIcon;
  public TextMeshProUGUI moneyText;

  public RelationshipCard relationshipUpAnimationCard;

  
  [Header("- Item Selector Screen -")]
  public ItemSelectorScreen itemSelectorScreen;

  [Header("- Battle Panel -")]
  public Panel battleInfoPanel;

  [Header("- Info Panels -")]
  public BattlerInfoPanel[] heroesInfoPanels;
  public BattlerInfoPanel enemyInfoPanels;

  [Header("- Actions Panel -")]
  public ActionsPanel actionsPanel;
  public GameObject selectTargetPanel;
  public GameObject[] selectTargets;

  [Header("- Map Panel -")]
  public Panel interactionPinsBoard;
  public InteractionPin[] interactionPins;


  [Header("- Tactical Battler INFO Panel -")]
  public BattlerInfoPanel battlerInfoPanel;

  [Header("- Tactical Buttons -")]
  public GameObject skipTurnButton;
  public GameObject clickMapButton;



  public void Awake() {
    instance = this;
  }



  public void Update() {
    skipTurnButton.SetActive(GameController.instance.gameState != GameState.noInput &&
                             GameController.instance.gameState != GameState.battlePhase);
  }

  public void UpdateUI() {
    GlobalData gb = GlobalData.instance;

    dayText.text = Lean.Localization.LeanLocalization.GetTranslationText("ui/day") + " " + VsnSaveSystem.GetIntVariable("day") +
      "<size=50%>/" + VsnSaveSystem.GetIntVariable("max_days") + "</size>";
    int daytime = VsnSaveSystem.GetIntVariable("daytime");
    daytimeIcon.sprite = ResourcesManager.instance.daytimeSprites[daytime];
    moneyText.text = "<sprite=\"Attributes\" index=4>" + VsnSaveSystem.GetIntVariable("money");

    UpdateBattleUI();
  }

  public void UpdateBattleUI() {
    if(BattleController.instance == null || BattleController.instance.partyMembers.Length == 0) {
      return;
    }

    for(int i=0; i<3; i++) {
      if(BattleController.instance.partyMembers.Length > i) {
        heroesInfoPanels[i].gameObject.SetActive(true);
        heroesInfoPanels[i].Initialize(BattleController.instance.partyMembers[i]);
      } else {
        heroesInfoPanels[i].gameObject.SetActive(false);
      }
    }

    if(BattleController.instance.enemyMembers != null &&
      BattleController.instance.enemyMembers.Length > 0 &&
      BattleController.instance.enemyMembers[0] != null) {
      enemyInfoPanels.Initialize(BattleController.instance.enemyMembers[0]);
    }
  }


  public void SkipHpBarAnimations() {
    Debug.LogWarning("SKIPPING HP BAR ANIM FOR HEROES");
    for(int i = 0; i < 3; i++) {
      if(BattleController.instance.partyMembers.Length > i) {
        heroesInfoPanels[i].SkipHpBarAnimation();
      }
    }

    if(BattleController.instance.enemyMembers != null &&
      BattleController.instance.enemyMembers.Length > 0 &&
      BattleController.instance.enemyMembers[0] != null) {
      Debug.LogWarning("SKIPPING HP BAR ANIM FOR ENEMY");
      enemyInfoPanels.SkipHpBarAnimation();
    }
  }


  public void SetScreenLayout(string state) {
    TheaterController theater = TheaterController.instance;
    Button firstButton = null;

    Debug.LogWarning("SETTING SCREEN LAYOUT: " + state);
    switch(state) {
      case "hide_all":
        uiControllerPanel.HidePanel();
        break;
      case "interact_with_board":
        uiControllerPanel.ShowPanel();
        break;
      case "tactical_view":
        uiControllerPanel.ShowPanel();
        battleInfoPanel.HidePanel();
        UIController.instance.EndBattlePhase();
        TheaterController.instance.gameObject.SetActive(false);
        BoardController.instance.gameObject.SetActive(true);

        CameraController.instance.SetActiveCamera(0);
        CameraController.instance.GoToDefaultPosition();
        break;
      case "battle":
        uiControllerPanel.HidePanel();
        battleInfoPanel.ShowPanel();
        UIController.instance.EnterBattlePhase();
        TheaterController.instance.gameObject.SetActive(true);
        BoardController.instance.gameObject.SetActive(false);

        CameraController.instance.SetActiveCamera(1);
        break;
    }
    SetupContext(state, firstButton);
    UpdateUI();
  }


  public void SetupContext(string state, Button firstButton) {
    //if(firstButton != null) {
    //  JoystickController.instance.GetContext("Basic Context").lastSelectedObject = firstButton.gameObject;
    //} else {
    //  JoystickController.instance.GetContext("Basic Context").lastSelectedObject = menuButtons[0].gameObject;
    //}
  }


  public void SetupCurrentCharacterUi(int currentPartyMember) {
    int partyLength = BattleController.instance.partyMembers.Length;

    // set no character's turn
    if(currentPartyMember == -1) {
      return;
    }

    // position actions panel
    actionsPanel.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f - 320f*currentPartyMember, 0f, 0f);
  }

  public void ShowBattleUI(bool value) {
    if(value == true) {
      battleInfoPanel.gameObject.SetActive(true);
      battleInfoPanel.canvasGroup.alpha = 1f;
    } else {
      battleInfoPanel.gameObject.SetActive(false);
    }
  }


  public void ResetPinsBoard(string bgName) {
    interactionPinsBoard.GetComponent<Image>().sprite = Resources.Load<Sprite>("Bg/" + bgName);
    foreach(InteractionPin pin in interactionPins) {
      pin.gameObject.SetActive(false);
    }
  }

  public void SetInteractionPin(int id, bool active, string scriptToLoad = "", string location = "") {
    interactionPins[id].gameObject.SetActive(active);
    if(active) {
      interactionPins[id].SetPinContent(scriptToLoad, location);
      //interactionPins[id].ResetSprite();
    }
  }

  public void SetInteractionPinLocationName(int id, string locationName) {
    interactionPins[id].SetLocation(locationName);
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
}
