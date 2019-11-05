using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameController : MonoBehaviour {

  public static GameController instance;
  public TextMeshProUGUI titleText;

  public PersonCard[] personCards;
  public TextMeshProUGUI dayText;

  public GameObject coupleEntryPrefab;
  public CoupleEntry[] coupleEntries;
  public Transform couplesPanelContent;

  public DateEvent[] dateSegments;

  public List<DateCardContent> cardsDeck;
  public List<DateCardContent> cardsHand;
  public List<DateCardContent> cardsDiscard;

  public Slider progressSlider;
  public TextMeshProUGUI progressText;
  public TextMeshProUGUI objectiveText;
  public TextMeshProUGUI moneyText;

  public GameObject bgImage;

  public ScreenTransitions peopleButtonPanel;
  public ScreenTransitions peopleInfoPanel;
  public ScreenTransitions couplesPanel;
  public ScreenTransitions buttonsPanel;
  public ScreenTransitions observationMap;
  public ScreenTransitions datingPeoplePanel;
  public PersonCard[] datingPeopleCards;

  public ItemSelectorScreen itemSelectorScreen;

  public GameObject dateUiPanel;
  public TextMeshProUGUI dateTitleText;
  public Toggle[] dateEventToggles;
  public Image[] successIcons;
  public Image[] failIcons;
  public Image[] unresolvedIcons;

  public ScreenTransitions girlInteractionPanel;
  public CoupleEntry coupleEntry;

  public ScreenTransitions dateCardsPanel;
  public DateCard[] dateCards;
  public TextMeshProUGUI dateHeartsCountText;

  public TextMeshProUGUI deckCountText;
  public CanvasGroup discardCanvasGroup;
  public TextMeshProUGUI discardCountText;

  public ScreenTransitions interactionPinsBoard;
  public InteractionPin[] interactionPins;

  public Button[] menuButton;

  public ParticleGenerator babiesParticleGenerator;
  public Image[] engagementScreenImages;
  public GameObject engagementScreen;




  public void Awake() {
    instance = this;
  }

  public void Start() {
    //VsnAudioManager.instance.PlayMusic("observacao_intro", "observacao_loop");

    if (VsnSaveSystem.GetIntVariable("minigame_ended") == 1) {
      //datingPeopleCards[0].Initialize(GlobalData.instance.people[0]);
      //datingPeopleCards[1].Initialize(GlobalData.instance.people[1]);

      VsnSaveSystem.SetVariable("minigame_ended", 0);
      UpdateUI();
      VsnController.instance.StartVSN("back_from_minigame");
    } else {
      GlobalData.instance.InitializeChapter();
      GlobalData.instance.PassTime();

      //VsnSaveSystem.SetVariable("observation_played", 1);
      //VsnController.instance.StartVSN("show_people_screen");

      UpdateUI();

      UpdateCouplesPanelContent();

      if(GlobalData.instance.hideTutorials) {
        VsnSaveSystem.SetVariable("tutorial_date", 1);
        VsnSaveSystem.SetVariable("tutorial_date2", 1);
        VsnSaveSystem.SetVariable("tutorial_shop", 1);
        VsnSaveSystem.SetVariable("tutorial_choose_date", 1);
        VsnSaveSystem.SetVariable("tutorial_observation", 1);
      }
      VsnController.instance.StartVSN("cap1_manha");
      //VsnController.instance.StartVSN("tutorial_intro");
      //VsnController.instance.StartVSN("check_end_game");
      //VsnController.instance.StartVSN("spend_day");
    }
  }

  public Button UpdateCouplesPanelContent() {
    int childCount = couplesPanelContent.transform.childCount;
    GameObject newobj;
    Button first = null;

    // reset couples panel content
    for(int i = 0; i < childCount; i++) {
      Destroy(couplesPanelContent.transform.GetChild(i).gameObject);
    }

    // create viable couples entries
    Relationship[] availableCouples = GlobalData.instance.GetCurrentDateableCouples();

    for(int i = 0; i < availableCouples.Length; i++) {
      newobj = Instantiate(coupleEntryPrefab, couplesPanelContent);
      newobj.GetComponent<CoupleEntry>().Initialize(availableCouples[i]);
      if(first == null) {
        first = newobj.GetComponentInChildren<Button>();
      }
    }

    return first;
  }


  public void Update() {
    if(Input.GetKeyDown(KeyCode.F4)){
      GlobalData.instance.ResetCurrentCouples();
      SceneManager.LoadScene(StageName.TitleScreen.ToString());
    }
    if (Input.GetKeyDown(KeyCode.F5)) {
      GlobalData.instance.ResetCurrentCouples();
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }
  

  public void UpdateUI() {
    GlobalData gb = GlobalData.instance;
    for(int i=0; i<personCards.Length; i++) {
      if(i < GlobalData.instance.people.Count) {
        personCards[i].gameObject.SetActive(true);
        personCards[i].Initialize(GlobalData.instance.people[i]);
      } else {
        personCards[i].gameObject.SetActive(false);
      }
    }


    dayText.text = Lean.Localization.LeanLocalization.GetTranslationText("ui/day") + " " + gb.day + " /" + VsnSaveSystem.GetIntVariable("max_days");
    progressText.text = GlobalData.instance.shippedCouples.Count.ToString();
    progressSlider.value = GlobalData.instance.shippedCouples.Count;
    progressSlider.maxValue = VsnSaveSystem.GetIntVariable("objective");
    objectiveText.text = "/"+ VsnSaveSystem.GetIntVariable("objective");
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
    for(int i = 0; i < 7; i++) {
      dateCards[i].UpdateUI();
    }

    deckCountText.text = cardsDeck.Count.ToString();
    if(cardsDiscard.Count > 0) {
      discardCanvasGroup.alpha = 1f;
      discardCountText.text = cardsDiscard.Count.ToString();
    } else {
      discardCanvasGroup.alpha = 0.5f;
      discardCountText.text = "";
    }

    dateHeartsCountText.text = GlobalData.instance.currentDateHearts + " /" + GlobalData.instance.maxDateHearts;

    ShowDateProgressUI();
  }

  public void ShowDateProgressUI(){
    int currentEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if (currentEvent < dateEventToggles.Length) {
      dateEventToggles[currentEvent].isOn = true;
    } else {
      foreach (Toggle t in dateEventToggles) {
        t.isOn = false;
      }
    }
    for (int i = 0; i < dateEventToggles.Length; i++) {
      switch (VsnSaveSystem.GetIntVariable("date_event_result_" + i)) {
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

  public void GenerateDate(int location){
    int dateSize = Mathf.Min(GlobalData.instance.allDateEvents.Count, 7);
    List<int> selectedEvents = new List<int>();
    int dateLocation = Random.Range(0, 2);
    VsnSaveSystem.SetVariable("date_location", dateLocation);

    dateSegments = new DateEvent[dateSize];
    for(int i=0; i<dateSize; i++){
      int selectedId = GetNewDateEvent(selectedEvents);
      dateSegments[i] = GlobalData.instance.allDateEvents[selectedId];
      selectedEvents.Add(selectedId);
    }
    System.Array.Sort(dateSegments, new System.Comparison<DateEvent>(
                                  (event1, event2) => event1.stage.CompareTo(event2.stage) ));
    SetDifficultyForEvents();

    GenerateDateDeck();
  }

  public int GetNewDateEvent(List<int> selectedEvents) {
    int selectedId;
    string dateLocationName = ((DateLocation)VsnSaveSystem.GetIntVariable("date_location")).ToString();
    do {
      selectedId = Random.Range(0, GlobalData.instance.allDateEvents.Count);

      Debug.LogWarning("selected location: " + GlobalData.instance.allDateEvents[selectedId].location);
      Debug.LogWarning("date location: " + dateLocationName);

    } while(selectedEvents.Contains(selectedId) ||
            (string.Compare(GlobalData.instance.allDateEvents[selectedId].location, dateLocationName) != 0 &&
             string.Compare(GlobalData.instance.allDateEvents[selectedId].location, "generico") != 0));

    return selectedId;
  } 

  public void SetDifficultyForEvents() {
    for(int i = 0; i < 7; i++) {
      if(i < 3) {
        dateSegments[i].difficulty = 4;
      } else if(i >= 5) {
        dateSegments[i].difficulty = 6;
      } else {
        dateSegments[i].difficulty = 5;
      }
    }
  }

  public void FleeDateSegment(int positionId) {
    List<int> currentUsedEvents = new List<int>();
    foreach(DateEvent d in dateSegments) {
      currentUsedEvents.Add(d.id);
    }

    Debug.LogWarning("currentUsedEvents: ");
    foreach(int i in currentUsedEvents) {
      Debug.Log("i: " + i);
    }

    int selectedId = GetNewDateEvent(currentUsedEvents);
    dateSegments[positionId] = GlobalData.instance.allDateEvents[selectedId];
    currentUsedEvents.Clear();

    SetDifficultyForEvents();
  }
  
  public void GenerateObservation() {
    int observationSize = Mathf.Min(GlobalData.instance.allObservationEvents.Count, 3);
    List<int> selectedEvents = new List<int>();
    List<ObservationEventType> allowedEventTypes = new List<ObservationEventType>();

    allowedEventTypes.Add(ObservationEventType.attributeTraining);
    allowedEventTypes.Add(ObservationEventType.bet);
    allowedEventTypes.Add(ObservationEventType.homeStalking);
    allowedEventTypes.Add(ObservationEventType.itemOnSale);
    if(GlobalData.instance.ObservedPerson().isMale) {
      allowedEventTypes.Add(ObservationEventType.femaleInTrouble);
    } else{
      allowedEventTypes.Add(ObservationEventType.maleInTrouble);
    }
  }

  public DateEvent GetCurrentDateEvent(){
    int currentDateEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if (dateSegments.Length <= currentDateEvent) {
      return null;
    }
    return dateSegments[currentDateEvent];
  }

  public string GetCurrentDateEventName() {
    if(GetCurrentDateEvent() == null) {
      return "";
    }
    return dateSegments[VsnSaveSystem.GetIntVariable("currentDateEvent")].scriptName;
  }

  public void ShowDateUiPanel(bool value) {
    dateUiPanel.SetActive(value);
    UpdateUI();
  }

  public void ShowEngagementScreen(int babies) {
    VsnAudioManager.instance.PlaySfx("date_success");

    engagementScreenImages[0].sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.CurrentBoy().faceId);
    engagementScreenImages[1].sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.CurrentGirl().faceId);
    babiesParticleGenerator.particlesToGenerate = babies;
    engagementScreen.SetActive(true);
    babiesParticleGenerator.DeleteSons();
    StartCoroutine(ShowEngagementScreenAnimation(5f+babies));
  }

  public IEnumerator ShowEngagementScreenAnimation(float waitTime){
    VsnController.instance.state = ExecutionState.WAITING;
    yield return new WaitForSeconds(waitTime);
    VsnController.instance.state = ExecutionState.PLAYING;
    HideEngagementScreen();
  }

  public void HideEngagementScreen() {
    engagementScreen.SetActive(false);
    babiesParticleGenerator.DeleteSons();
  }

  public void ShowOnlyObservedPerson(){
    foreach(PersonCard p in personCards){
      if(p.person == GlobalData.instance.ObservedPerson() ||
        p.person == GlobalData.instance.EncounterPerson()) {
        p.gameObject.SetActive(true);
      } else{
        p.gameObject.SetActive(false);
      }
    }
  }

  public void SetScreenLayout(string state){
    TheaterController theater = TheaterController.instance;
    Button firstButton = null;
    foreach(PersonCard p in personCards) {
      p.gameObject.SetActive(p.person.state == PersonState.available);
      if(firstButton == null) {
        firstButton = p.equipIcon.transform.parent.GetComponent<Button>();
      }
    }

    Debug.LogWarning("SETTING SCREEN LAYOUT: " + state);
  
    switch(state) {
      case "hide_all":
        titleText.gameObject.SetActive(false);
        bgImage.SetActive(true);
        datingPeoplePanel.HidePanel();
        couplesPanel.HidePanel();
        buttonsPanel.HidePanel();
        observationMap.HidePanel();
        interactionPinsBoard.HidePanel();
        break;
      case "interact_with_board":
        titleText.gameObject.SetActive(true);
        SetTitleText();
        bgImage.SetActive(true);
        if(VsnSaveSystem.GetIntVariable("daytime") == 2) {
          UpdateCouplesPanelContent();
        }
        datingPeoplePanel.HidePanel();
        couplesPanel.HidePanel();
        buttonsPanel.ShowPanel();
        observationMap.HidePanel();
        interactionPinsBoard.ShowPanel();
        break;
      case "observation":
        titleText.gameObject.SetActive(false);
        theater.SetEvent(TheaterEvent.observation);
        bgImage.SetActive(false);
        ShowOnlyObservedPerson();
        datingPeoplePanel.HidePanel();
        couplesPanel.HidePanel();
        buttonsPanel.HidePanel();
        observationMap.HidePanel();
        interactionPinsBoard.HidePanel();
        break;
      case "observation_map":
        titleText.gameObject.SetActive(true);
        SetTitleText();
        bgImage.SetActive(true);
        datingPeoplePanel.HidePanel();
        couplesPanel.HidePanel();
        buttonsPanel.HidePanel();
        observationMap.ShowPanel();
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
        datingPeoplePanel.ShowPanel();
        couplesPanel.HidePanel();
        buttonsPanel.HidePanel();
        observationMap.HidePanel();
        interactionPinsBoard.HidePanel();
        break;
    }
    SetupContext(state, firstButton);
    UpdateUI();
  }

  public void SetTitleText() {
    int id = VsnSaveSystem.GetIntVariable("daytime")+1;
    titleText.text = Lean.Localization.LeanLocalization.GetTranslationText("gameplay/title_"+id);
    Debug.LogWarning("SET TITLE TEXT TO: "+ titleText.text);
  }


  public void SetupContext(string state, Button firstButton) {
    if(firstButton != null) {
      JoystickController.instance.GetContext("Basic Context").lastSelectedObject = firstButton.gameObject;
    } else {
      JoystickController.instance.GetContext("Basic Context").lastSelectedObject = menuButton[0].gameObject;
    }
    //switch(state) {
    //  case "choose_observation_target":
    //    if(firstButton != null) {
    //      JoystickController.instance.CurrentContext().lastSelectedObject = firstButton.gameObject;
    //    } else {
    //      JoystickController.instance.CurrentContext().lastSelectedObject = menuButton[0].gameObject;
    //    }
    //    break;
    //  case "choose_date_target":
    //    if(firstButton != null) {
    //      JoystickController.instance.CurrentContext().lastSelectedObject = firstButton.gameObject;
    //    } else {
    //      JoystickController.instance.CurrentContext().lastSelectedObject = menuButton[0].gameObject;
    //    }
    //    break;
    //}
  }

  public void HidePeople(){
    foreach (PersonCard p in personCards) {
      p.gameObject.SetActive(false);
    }
  }


  public void ShowGirlInteractionScreen() {
    Relationship r = GlobalData.instance.GetCurrentRelationship();
    if(r == null){
      Debug.LogError("Error getting current relationship");
    }
    coupleEntry.Initialize(r);
    girlInteractionPanel.ShowPanel();
  }

  public void HideGirlInteractionScreen() {
    girlInteractionPanel.HidePanel();
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

  public void GenerateDateDeck() {
    cardsHand = new List<DateCardContent>();
    cardsDiscard = new List<DateCardContent>();
    cardsDeck = new List<DateCardContent>();
    for(int i = 0; i <= 8; i++) { /// NO ITEMS FOR NOW
    //for(int i = 0; i <= 9; i++) {
      cardsDeck.Add(CardsDatabase.instance.GetCardById(i));
    }
    cardsDeck.Add(CardsDatabase.instance.GetCardById(GlobalData.instance.observedPeople[0].skillId));
    cardsDeck.Add(CardsDatabase.instance.GetCardById(GlobalData.instance.observedPeople[1].skillId));
  }

  public void ShuffleNewCardHand() {

    // reshuffle hand back to deck
    if(cardsHand != null) {
      cardsDeck.AddRange(cardsHand);
    }

    cardsHand = new List<DateCardContent>();
    cardsDeck = cardsDeck.OrderBy(x => Random.value).ToList();

    cardsHand.AddRange(cardsDeck.GetRange(0, Mathf.Min(cardsDeck.Count, 7)));
    cardsHand = cardsHand.OrderBy(content => content.type).ToList();

    cardsDeck.RemoveRange(0, cardsHand.Count);

    for(int i = 0; i < 7; i++) {
      if(i < cardsHand.Count){
        dateCards[i].Initialize(i, cardsHand[i]);
      }      
      dateCards[i].CardGoToEndOfHand();
    }
    UpdateDateUI();
  }

  public void CardFromHandToDeck(DateCardContent card) {
    cardsDeck.Add(card);
    cardsHand.Remove(card);
    UpdateDateUI();
  }

  public void DiscardCardFromDeck(DateCardContent card) {
    cardsDiscard.Add(card);
    cardsDeck.Remove(card);
    UpdateDateUI();
  }

  public void DiscardCardFromHand(DateCardContent card) {
    cardsDiscard.Add(card);
    cardsHand.Remove(card);
    UpdateDateUI();
  }

  public void EndTurn() {
    foreach(Person p in GlobalData.instance.observedPeople) {
      p.EndTurn();
    }
    UpdateDateUI();
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

  public void ClickDateButton() {
    SfxManager.StaticPlayForbbidenSfx();
  }
}
