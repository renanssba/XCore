using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour {
  public static UIController instance;


  public TextMeshProUGUI titleText;

  public PersonCard[] personCards;
  public TextMeshProUGUI dayText;

  public GameObject coupleEntryPrefab;
  public CoupleEntry[] coupleEntries;

  public TextMeshProUGUI moneyText;

  public GameObject bgImage;

  public Button[] menuButtons;

  public ScreenTransitions peopleButtonPanel;
  public ScreenTransitions peopleInfoPanel;
  public ScreenTransitions mapMenuButtonsPanel;
  public ScreenTransitions datingPeoplePanel;
  public PersonCard[] datingPeopleCards;

  public GameObject dateUiPanel;
  public TextMeshProUGUI dateTitleText;
  public Toggle[] dateEventToggles;
  public Image[] successIcons;
  public Image[] failIcons;
  public Image[] unresolvedIcons;

  public ScreenTransitions actionsPanel;

  public ScreenTransitions datingPeopleInfoPanel;
  public PersonCard[] partyPeopleCards;

  public ScreenTransitions interactionPinsBoard;
  public InteractionPin[] interactionPins;



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

    if(GlobalData.instance.ObservedPerson() != null) {
      datingPeopleCards[0].Initialize(GlobalData.instance.ObservedPerson());
    }
    if(GlobalData.instance.EncounterPerson() != null) {
      datingPeopleCards[1].Initialize(GlobalData.instance.EncounterPerson());
    }
    if(GlobalData.instance.EncounterPerson() != null) {
      datingPeopleCards[2].Initialize(GlobalData.instance.people[4]);
    }

    ShowDateProgressUI();
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
        titleText.gameObject.SetActive(false);
        bgImage.SetActive(true);
        datingPeoplePanel.HidePanel();
        mapMenuButtonsPanel.HidePanel();
        interactionPinsBoard.HidePanel();
        break;
      case "interact_with_board":
        titleText.gameObject.SetActive(true);
        SetTitleText();
        bgImage.SetActive(true);
        datingPeoplePanel.HidePanel();
        mapMenuButtonsPanel.ShowPanel();
        interactionPinsBoard.ShowPanel();
        break;
      case "observation":
        titleText.gameObject.SetActive(false);
        theater.SetEvent(TheaterEvent.observation);
        bgImage.SetActive(false);
        ShowOnlyObservedPerson();
        datingPeoplePanel.HidePanel();
        mapMenuButtonsPanel.HidePanel();
        interactionPinsBoard.HidePanel();
        break;
      case "date":
      case "date_challenge":
        titleText.gameObject.SetActive(false);
        if(state == "date") {
          theater.SetEvent(TheaterEvent.date);
        } else {
          theater.SetEvent(TheaterEvent.dateChallenge);
        }
        bgImage.SetActive(false);
        peopleInfoPanel.HidePanel();
        peopleButtonPanel.ShowPanel();
        datingPeoplePanel.ShowPanel();
        mapMenuButtonsPanel.HidePanel();
        interactionPinsBoard.HidePanel();
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

  public void ShowDateUiPanel(bool value) {
    dateUiPanel.SetActive(value);
    UpdateUI();
  }
}
