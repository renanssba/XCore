using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour {
  public static UIController instance;

  [Header("- UI and Daytime -")]
  public ScreenTransitions uiControllerPanel;
  public TextMeshProUGUI dayText;
  public Image daytimeIcon;

  public RelationshipCard relationshipUpAnimationCard;

  public CoupleEntry[] coupleEntries;

  public TextMeshProUGUI moneyText;

  [Header("- Menu Buttons -")]
  public Button[] menuButtons;
  public GameObject[] menuButtonAlertIcons;

  [Header("- Dating Panel -")]
  public ScreenTransitions datingPeoplePanel;
  public HpSlider partyHpSlider;
  public TextMeshProUGUI partyHpText;
  public HpSlider enemyHpSlider;
  public TextMeshProUGUI enemyHpText;
  public Image[] stealthEyeIcons;
  public TextMeshProUGUI enemyLevelText;

  public ScreenTransitions dateProgressPanel;
  public Toggle[] dateEventToggles;
  public Image[] successIcons;
  public Image[] failIcons;
  public Image[] unresolvedIcons;

  public ScreenTransitions datingPeopleInfoPanel;
  public PersonCard[] partyPeopleCards;

  [Header("- Actions Panel -")]
  public ActionsPanel actionsPanel;
  public GameObject selectTargetPanel;
  public GameObject[] selectTargets;

  [Header("- Help Panel -")]
  public ScreenTransitions helpMessagePanel;
  public TextMeshProUGUI helpMessageText;

  [Header("- Map Panel -")]
  public ScreenTransitions interactionPinsBoard;
  public InteractionPin[] interactionPins;


  public CoupleStatusScreen coupleStatusScreen;


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

    SetBoardMenuButtons();

    UpdateDateUI();
  }

  public void UpdateDateUI() {
    if(BattleController.instance.partyMembers.Length == 0) {
      return;
    }

    partyHpSlider.SetMaxValue(BattleController.instance.maxHp);
    partyHpSlider.SetSliderValueWithoutAnimation(BattleController.instance.hp);
    partyHpText.text = BattleController.instance.hp.ToString();

    for(int i=0; i<3; i++) {
      if(BattleController.instance.partyMembers.Length > i) {
        partyPeopleCards[i].Initialize(BattleController.instance.partyMembers[i]);
        partyPeopleCards[i].gameObject.SetActive(true);
      } else {
        partyPeopleCards[i].gameObject.SetActive(false);
      }
    }

    // set HP slider width
    // TODO: update HP slider
    //partyHpSlider.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(partyPeopleCards[0].transform.parent.GetComponent<RectTransform>().rect.width, 32f);
    
    //partyHpSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(partyPeopleCards[0].transform.parent.GetComponent<RectTransform>().sizeDelta.x, 32f);
    //partyPeopleCards[0].transform.parent.GetComponent<RectTransform>().sizeDelta.x;
    //ShowDateProgressUI();

    if(BattleController.instance.GetCurrentEnemy() != null) {
      BattleController.instance.GetCurrentEnemy().UpdateStatusConditions();
    }
  }


  public void AnimatePartyHpChange(int initialHp, int finalHp) {
    partyHpSlider.SetSliderValue(finalHp);
    partyHpText.text = finalHp.ToString();

    //float currentShownHp = initialHp;
    //DOTween.To(() => currentShownHp, x => currentShownHp = x, finalHp, 1f).OnUpdate( ()=> {
    //  //partyHpSlider.value = currentShownHp;
    //  partyHpText.text = ((int)currentShownHp).ToString();
    //} );
  }

  public void AnimateEnemyHpChange(int initialHp, int finalHp) {
    enemyHpSlider.SetSliderValue(finalHp);
    enemyHpText.text = finalHp.ToString();

    //float currentShownHp = initialHp;
    //DOTween.To(() => currentShownHp, x => currentShownHp = x, finalHp, 1f).OnUpdate( ()=> {
    //  //enemyHpSlider.value = currentShownHp;
    //  enemyHpText.text = ((int)currentShownHp).ToString();
    //} );
  }

  public void AnimateStealthValueChange(int previousValue, int currentValue) {
    for(int i=0; i<stealthEyeIcons.Length; i++) {
      if(i < 3-currentValue) {
        stealthEyeIcons[i].sprite = ResourcesManager.instance.stealthEyeSprites[1];
        if(i >= 3-previousValue) {
          stealthEyeIcons[i].transform.DOScale(1.3f, 0.5f).SetLoops(2, LoopType.Yoyo);
        }
      } else {
        stealthEyeIcons[i].sprite = ResourcesManager.instance.stealthEyeSprites[0];
      }
    }
  }

  public void ShowDateProgressUI() {
    int currentEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if(currentEvent < dateEventToggles.Length) {
      dateEventToggles[currentEvent].isOn = true;
    } else {
      foreach(Toggle t in dateEventToggles) {
        t.isOn = false;
      }
    }
    for(int i = 0; i < dateEventToggles.Length; i++) {
      switch(VsnSaveSystem.GetIntVariable("date_event_result_" + i)) {
        case 0:
          successIcons[i].gameObject.SetActive(false);
          failIcons[i].gameObject.SetActive(false);
          unresolvedIcons[i].gameObject.SetActive(true);
          break;
        case 1:
          successIcons[i].gameObject.SetActive(true);
          failIcons[i].gameObject.SetActive(false);
          unresolvedIcons[i].gameObject.SetActive(false);
          break;
        case 2:
          successIcons[i].gameObject.SetActive(false);
          failIcons[i].gameObject.SetActive(true);
          unresolvedIcons[i].gameObject.SetActive(false);
          break;
      }
    }
  }


  public void SetScreenLayout(string state) {
    TheaterController theater = TheaterController.instance;
    Button firstButton = null;

    Debug.LogWarning("SETTING SCREEN LAYOUT: " + state);
    switch(state) {
      case "hide_all":
        interactionPinsBoard.HidePanel();
        uiControllerPanel.HidePanel();
        datingPeoplePanel.HidePanel();
        break;
      case "interact_with_board":
        interactionPinsBoard.ShowPanel();
        uiControllerPanel.ShowPanel();
        datingPeoplePanel.HidePanel();
        break;
      case "girl_interaction_screen":
        interactionPinsBoard.HidePanel();
        uiControllerPanel.HidePanel();
        datingPeoplePanel.HidePanel();
        break;
      case "date":
      case "date_challenge":
        interactionPinsBoard.HidePanel();
        uiControllerPanel.HidePanel();
        datingPeoplePanel.HidePanel();
        break;
    }
    SetupContext(state, firstButton);
    UpdateUI();
  }


  public void SetupContext(string state, Button firstButton) {
    if(firstButton != null) {
      JoystickController.instance.GetContext("Basic Context").lastSelectedObject = firstButton.gameObject;
    } else {
      JoystickController.instance.GetContext("Basic Context").lastSelectedObject = menuButtons[0].gameObject;
    }
  }


  public void SetupCurrentCharacterUi(int currentPartyMember) {
    int partyLength = BattleController.instance.partyMembers.Length;

    // set no character's turn
    if(currentPartyMember == -1) {
      for(int i = 0; i < 3; i++) {
        partyPeopleCards[i].ShowShade(false);
      }
      return;
    }

    // position actions panel
    actionsPanel.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f - 320f*currentPartyMember, 0f, 0f);

    // turn characters UI
    //for(int i = 0; i < 3; i++) {
    //  partyPeopleCards[i].ShowShade(true);
    //}
    partyPeopleCards[currentPartyMember].ShowShade(false);
  }

  public void ShowDateUI(bool value) {
    if(value == true) {
      UpdateDateUI();
      datingPeoplePanel.OpenMenuScreen();
    } else {
      datingPeoplePanel.CloseMenuScreen();
    }    
  }

  public void SetHelpMessageText(string helpMessage) {
    helpMessageText.text = helpMessage;
    helpMessageText.maxVisibleCharacters = helpMessage.Length;
  }

  public void CleanHelpMessagePanel() {
    helpMessageText.text = "";
  }


  public void SetBoardMenuButtons() {
    //bool partTimeUnlocked = false;
    //bool shopButton = false;

    for(int i=0; i<menuButtonAlertIcons.Length; i++) {
      menuButtonAlertIcons[i].SetActive(false);
      menuButtons[i].gameObject.SetActive(true); // activate all menu buttons
    }

    //if(VsnSaveSystem.GetIntVariable("day") >= 2) {
    //  shopButton = true;
    //  if(VsnSaveSystem.GetIntVariable("shop_level") == 0 ||
    //     VsnSaveSystem.GetIntVariable("shop_unlock_advance") == 1 ||
    //     VsnSaveSystem.GetIntVariable("shop_unlock_final") == 1) {
    //    menuButtonAlertIcons[1].SetActive(true);
    //  }
    //}

    //if(VsnSaveSystem.GetIntVariable("day") >= 3) {
    //  partTimeUnlocked = true;
    //  if(VsnSaveSystem.GetBoolVariable("part_time_intro") == false) {
    //    menuButtonAlertIcons[0].SetActive(true);
    //  }
    //}

    //menuButtons[0].gameObject.SetActive(partTimeUnlocked);
    //menuButtons[1].gameObject.SetActive(shopButton);
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




  public void ShowPartyPeopleCards() {
    for(int i=0; i<BattleController.instance.partyMembers.Length; i++) {
      partyPeopleCards[i].Initialize( BattleController.instance.partyMembers[i]);
    }

  }

  public void ShowDateProgressPanel(bool value) {
    if(value) {
      dateProgressPanel.ShowPanel();
    } else {
      dateProgressPanel.HidePanel();
    }
    UpdateUI();
  }
}
