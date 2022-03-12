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

  public RelationshipCard relationshipUpAnimationCard;

  public CoupleEntry[] coupleEntries;

  public TextMeshProUGUI moneyText;

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


  public StatusScreen coupleStatusScreen;


  public Transform enemyStatusConditionsContent;


  public GirlInteractionScreen girlInteractionScreen;

  public GameObject statusConditionIconPrefab;



  public void Awake() {
    instance = this;
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
        heroesInfoPanels[i].Initialize(BattleController.instance.partyMembers[i]);
        heroesInfoPanels[i].gameObject.SetActive(true);
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



  public void SetScreenLayout(string state) {
    TheaterController theater = TheaterController.instance;
    Button firstButton = null;

    Debug.LogWarning("SETTING SCREEN LAYOUT: " + state);
    switch(state) {
      case "hide_all":
        //interactionPinsBoard.HidePanel();
        uiControllerPanel.HidePanel();
        break;
      case "interact_with_board":
        //interactionPinsBoard.ShowPanel();
        uiControllerPanel.ShowPanel();
        break;
      case "battle":
        //interactionPinsBoard.HidePanel();
        uiControllerPanel.HidePanel();
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
}
