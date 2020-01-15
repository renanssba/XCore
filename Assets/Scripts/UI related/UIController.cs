using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour {
  public static UIController instance;

  public TextMeshProUGUI titleText;

  public ScreenTransitions uiControllerPanel;
  public PersonCard[] personCards;
  public TextMeshProUGUI dayText;

  public RelationshipCard relationshipCard;

  public CoupleEntry[] coupleEntries;

  public TextMeshProUGUI moneyText;

  public GameObject bgImage;

  public Button[] menuButtons;

  public ScreenTransitions peopleButtonPanel;
  public ScreenTransitions peopleInfoPanel;
  public ScreenTransitions mapMenuButtonsPanel;
  public ScreenTransitions datingPeoplePanel;
  public Slider partyHpSlider;
  public TextMeshProUGUI partyHpText;

  public ScreenTransitions dateProgressPanel;
  public TextMeshProUGUI dateTitleText;
  public Toggle[] dateEventToggles;
  public Image[] successIcons;
  public Image[] failIcons;
  public Image[] unresolvedIcons;

  public ScreenTransitions datingPeopleInfoPanel;
  public PersonCard[] partyPeopleCards;

  public ActionsPanel actionsPanel;
  public GameObject selectTargetPanel;

  public ScreenTransitions helpMessagePanel;
  public TextMeshProUGUI helpMessageText;

  public ScreenTransitions interactionPinsBoard;
  public InteractionPin[] interactionPins;

  public Image girlInteractionImage;
  public Image boyInteractionImage;

  public GameObject fertilielInMenu;

  public GameObject[] turnIndicators;

  public GameObject statusConditionIconPrefab;



  public void Awake() {
    instance = this;
  }



  public void UpdateUI() {
    GlobalData gb = GlobalData.instance;
    for(int i = 0; i < personCards.Length; i++) {
      if(i < GlobalData.instance.people.Count) {
        personCards[i].gameObject.SetActive(true);
        personCards[i].Initialize(GlobalData.instance.people[i]);
      } else {
        personCards[i].gameObject.SetActive(false);
      }
    }


    dayText.text = Lean.Localization.LeanLocalization.GetTranslationText("ui/day") + " " + gb.day;
    moneyText.text = "<sprite=\"Attributes\" index=4>" + VsnSaveSystem.GetIntVariable("money");

    UpdateDateUI();
  }

  public void UpdateDateUI() {
    if(GlobalData.instance.CurrentBoy() == null ||
       GlobalData.instance.CurrentGirl() == null) {
      return;
    }

    partyHpSlider.maxValue = BattleController.instance.maxHp;
    partyHpSlider.value = BattleController.instance.hp;
    partyHpText.text = BattleController.instance.hp.ToString();


    for(int i=0; i<3; i++) {
      if(BattleController.instance.partyMembers.Length > i) {
        partyPeopleCards[i].Initialize(BattleController.instance.partyMembers[i]);
        partyPeopleCards[i].gameObject.SetActive(true);
      } else {
        partyPeopleCards[i].gameObject.SetActive(false);
      }
    }
    ShowDateProgressUI();
  }

  public void AnimateHpChange(int initialHp, int finalHp) {
    float currentShownHp = initialHp;
    DOTween.To(() => currentShownHp, x => currentShownHp = x, finalHp, 1f).OnUpdate( ()=> {
      partyHpSlider.value = currentShownHp;
      partyHpText.text = ((int)currentShownHp).ToString();
    } );    
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

  public void ShowOnlyObservedPerson() {
    foreach(PersonCard p in personCards) {
      if(p.person == GlobalData.instance.ObservedPerson() ||
        p.person == GlobalData.instance.EncounterPerson()) {
        p.gameObject.SetActive(true);
      } else {
        p.gameObject.SetActive(false);
      }
    }
  }


  public void SetScreenLayout(string state) {
    TheaterController theater = TheaterController.instance;
    Button firstButton = null;
    foreach(PersonCard p in personCards) {
      p.gameObject.SetActive(p.person.state == PersonState.available);
    }

    Debug.LogWarning("SETTING SCREEN LAYOUT: " + state);

    switch(state) {
      case "hide_all":
        uiControllerPanel.HidePanel();
        titleText.gameObject.SetActive(false);
        bgImage.SetActive(true);
        peopleInfoPanel.HidePanel();
        peopleButtonPanel.HidePanel();
        datingPeoplePanel.HidePanel();
        mapMenuButtonsPanel.HidePanel();
        interactionPinsBoard.HidePanel();
        fertilielInMenu.SetActive(false);
        break;
      case "interact_with_board":
        uiControllerPanel.ShowPanel();
        titleText.gameObject.SetActive(true);
        SetTitleText();
        bgImage.SetActive(true);
        peopleInfoPanel.HidePanel();
        peopleButtonPanel.ShowPanel();
        datingPeoplePanel.HidePanel();
        mapMenuButtonsPanel.ShowPanel();
        interactionPinsBoard.ShowPanel();
        fertilielInMenu.SetActive(true);
        break;
      case "date":
      case "date_challenge":
        uiControllerPanel.HidePanel();
        titleText.gameObject.SetActive(false);
        //if(state == "date") {
        //  theater.SetEvent(TheaterEvent.date);
        //} else {
        //  theater.SetEvent(TheaterEvent.dateChallenge);
        //}
        bgImage.SetActive(false);
        peopleInfoPanel.HidePanel();
        peopleButtonPanel.HidePanel();
        datingPeoplePanel.ShowPanel();
        mapMenuButtonsPanel.HidePanel();
        interactionPinsBoard.HidePanel();
        fertilielInMenu.SetActive(false);
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
        turnIndicators[i].SetActive(false);
      }
      return;
    }


    // position actions panel
    actionsPanel.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f - 320f*currentPartyMember, 0f, 0f);

    // turn characters UI
    for(int i = 0; i < 3; i++) {
      partyPeopleCards[i].ShowShade(true);
      turnIndicators[i].SetActive(false);
    }
    partyPeopleCards[currentPartyMember].ShowShade(false);
    turnIndicators[currentPartyMember].SetActive(true);
  }

  public void ShowHelpMessagePanel(string helpMessage) {
    Debug.LogWarning("Show Help Message Panel!");
    helpMessageText.text = helpMessage;
    helpMessagePanel.ShowPanel();
  }

  public void HideHelpMessagePanel() {
    helpMessagePanel.HidePanel();
  }


  public void HidePeople() {
    foreach(PersonCard p in personCards) {
      p.gameObject.SetActive(false);
    }
  }

  public void SetTitleText() {
    int id = VsnSaveSystem.GetIntVariable("daytime") + 1;
    titleText.text = Lean.Localization.LeanLocalization.GetTranslationText("gameplay/title_" + id);
    Debug.LogWarning("SET TITLE TEXT TO: " + titleText.text);
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
      interactionPins[id].ResetSprite();
    }
  }

  public void SetInteractionPinSprite(int id, string spriteName) {
    Sprite s = Resources.Load<Sprite>("Characters/" + spriteName);
    if(s != null) {
      interactionPins[id].SetSprite(s);
    }
  }




  public void ShowPartyPeopleCards() {
    for(int i=0; i<3; i++) {
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
