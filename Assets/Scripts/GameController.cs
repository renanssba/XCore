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
  public TextMeshProUGUI apText;

  public GameObject coupleEntryPrefab;
  public CoupleEntry[] coupleEntries;
  public Transform couplesPanelContent;
  public GameObject couplesPanelEmptyIcon;
  
  public ObservationEvent[] observationSegments;
  public ObservationTile[] observationTiles;
  public GameObject playerToken;
  public TextMeshProUGUI energyText;
  public Image playerTokenImage;
  public Color[] observationTilesColors;

  public DateEvent[] dateSegments;

  public Slider progressSlider;
  public TextMeshProUGUI progressText;
  public TextMeshProUGUI objectiveText;
  public TextMeshProUGUI moneyText;

  public GameObject bgImage;
  public ScreenTransitions progressPanel;
  public ScreenTransitions peoplePanel;
  public ScreenTransitions couplesPanel;
  public ScreenTransitions buttonsPanel;
  public ScreenTransitions observationMap;
  public ScreenTransitions actionPersonCard;
  //public GameObject miniFertililel;

  public ItemSelectorScreen itemSelectorScreen;

  public GameObject dateUiPanel;
  public TextMeshProUGUI dateTitleText;
  public Toggle[] dateEventToggles;
  public Image[] successIcons;
  public Image[] failIcons;
  public Image[] unresolvedIcons;

  public Button[] menuButton;

  public ParticleGenerator babiesParticleGenerator;
  public Image[] engagementScreenImages;
  public GameObject engagementScreen;



  public void Awake() {
    instance = this;
  }

  public void Start() {
    VsnAudioManager.instance.PlayMusic("observacao_intro", "observacao_loop");

    if (VsnSaveSystem.GetIntVariable("minigame_ended") == 1) {
      VsnSaveSystem.SetVariable("minigame_ended", 0);
      UpdateUI();
      VsnController.instance.StartVSN("back_from_minigame");
    } else {
      GlobalData.instance.InitializeChapter();
      GlobalData.instance.PassDay();
      UpdateUI();

      UpdateCouplesPanelContent();

      if(GlobalData.instance.hideTutorials) {
        VsnSaveSystem.SetVariable("tutorial_date", 1);
        VsnSaveSystem.SetVariable("tutorial_date2", 1);
        VsnSaveSystem.SetVariable("tutorial_shop", 1);
        VsnSaveSystem.SetVariable("tutorial_choose_date", 1);
        VsnSaveSystem.SetVariable("tutorial_observation", 1);
      }
      VsnController.instance.StartVSN("tutorial_intro");
      //VsnController.instance.StartVSN("check_end_game");
      //VsnController.instance.StartVSN("spend_day");
    }
  }

  public Button UpdateCouplesPanelContent() {
    int childCount = couplesPanelContent.transform.childCount;
    Person boy;
    Person girl;
    GameObject newobj;
    Button first = null;

    // reset couples panel content
    for(int i = 0; i < childCount; i++) {
      Destroy(couplesPanelContent.transform.GetChild(i).gameObject);
    }

    // create viable couples entries
    for(int i = 0; i < GlobalData.instance.boysToGenerate; i++) {
      for(int j = 0; j < GlobalData.instance.girlsToGenerate; j++) {
        boy = GlobalData.instance.people[i];
        girl = GlobalData.instance.people[j + GlobalData.instance.boysToGenerate];
        if(GlobalData.instance.viableCouple[i, j] == true &&
          boy.state != PersonState.shipped &&
          girl.state != PersonState.shipped) {
          newobj = Instantiate(coupleEntryPrefab, couplesPanelContent);
          newobj.GetComponent<CoupleEntry>().Initialize(boy, girl);

          if(first == null) {
            first = newobj.GetComponentInChildren<Button>();
          }
        }
      }
    }

    couplesPanelEmptyIcon.SetActive(couplesPanelContent.childCount == 0);


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


  public bool SpendAP(int cost){
    GlobalData gb = GlobalData.instance;
    if(gb.ap < cost){
      return false;
    }
    gb.ap -= cost;
    UpdateUI();
    return true;
  }
  

  public void UpdateUI() {
    GlobalData gb = GlobalData.instance;
    for(int i=0; i<personCards.Length; i++) {
      personCards[i].Initialize(GlobalData.instance.people[i]);
    }
    //personCards[0].Initialize(GlobalData.instance.people[coupleId * 2]);
    //personCards[1].Initialize(GlobalData.instance.people[coupleId * 2 + 1]);
    dayText.text = Lean.Localization.LeanLocalization.GetTranslationText("ui/day") + " " + gb.day + " /" + VsnSaveSystem.GetIntVariable("max_days");
    apText.text = "AP: " + gb.ap + " /" + gb.maxAp;
    progressText.text = GlobalData.instance.shippedCouples.Count.ToString();
    progressSlider.value = GlobalData.instance.shippedCouples.Count;
    progressSlider.maxValue = VsnSaveSystem.GetIntVariable("objective");
    objectiveText.text = "/"+ VsnSaveSystem.GetIntVariable("objective");
    moneyText.text = (VsnSaveSystem.GetIntVariable("money")).ToString();

    energyText.text = Lean.Localization.LeanLocalization.GetTranslationText("ui/energy")+" " + VsnSaveSystem.GetIntVariable("observation_energy");
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
    int selectedId;
    int dateLocation = Random.Range(0, 2);
    //int dateLocation = Random.Range(0, 1);

    VsnSaveSystem.SetVariable("date_location", dateLocation);

    dateSegments = new DateEvent[dateSize];
    for(int i=0; i<dateSize; i++){
      do {
        selectedId = Random.Range(0, GlobalData.instance.allDateEvents.Count);

        Debug.LogWarning("selected location: " + GlobalData.instance.allDateEvents[selectedId].location);
        Debug.LogWarning("date location: " + ((DateLocation)dateLocation).ToString());

      } while (selectedEvents.Contains(selectedId) ||
              (string.Compare(GlobalData.instance.allDateEvents[selectedId].location, ((DateLocation)dateLocation).ToString())!=0 &&
               string.Compare(GlobalData.instance.allDateEvents[selectedId].location, "generico")!=0) );
      dateSegments[i] = GlobalData.instance.allDateEvents[selectedId];
      selectedEvents.Add(selectedId);
    }
    System.Array.Sort(dateSegments, new System.Comparison<DateEvent>(
                                  (event1, event2) => event1.stage.CompareTo(event2.stage) ));
  }
  
  public void GenerateObservation() {
    int observationSize = Mathf.Min(GlobalData.instance.allObservationEvents.Count, 3);
    List<int> selectedEvents = new List<int>();
    List<ObservationEventType> allowedEventTypes = new List<ObservationEventType>();
    int selectedId;

    allowedEventTypes.Add(ObservationEventType.attributeTraining);
    allowedEventTypes.Add(ObservationEventType.bet);
    allowedEventTypes.Add(ObservationEventType.homeStalking);
    allowedEventTypes.Add(ObservationEventType.itemOnSale);
    if(GlobalData.instance.ObservedPerson().isMale) {
      allowedEventTypes.Add(ObservationEventType.femaleInTrouble);
    } else{
      allowedEventTypes.Add(ObservationEventType.maleInTrouble);
    }

    observationSegments = new ObservationEvent[observationSize];
    for (int i = 0; i < observationSize; i++) {
      do {
        selectedId = Random.Range(0, GlobalData.instance.allObservationEvents.Count);
      } while (selectedEvents.Contains(selectedId) || !allowedEventTypes.Contains(GlobalData.instance.allObservationEvents[selectedId].eventType));
      selectedEvents.Add(selectedId);
      observationSegments[i] = GlobalData.instance.allObservationEvents[selectedId];
      if(observationSegments[i].eventType == ObservationEventType.femaleInTrouble ||
         observationSegments[i].eventType == ObservationEventType.maleInTrouble) {
        //observationSegments[i].personInEvent = GlobalData.instance.GetDateablePerson(GlobalData.instance.ObservedPerson());
      }
    }
  }

  public DateEvent GetCurrentDateEvent(){
    int currentDateEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if (dateSegments.Length <= currentDateEvent) {
      return null;
    }
    return dateSegments[currentDateEvent];
  }

  public ObservationEvent GetCurrentObservationEvent() {
    int currentEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if (observationSegments.Length <= currentEvent) {
      return null;
    }
    return observationSegments[currentEvent];
  }

  public string GetCurrentDateEventName() {
    if(GetCurrentDateEvent() == null) {
      return "";
    }
    return dateSegments[VsnSaveSystem.GetIntVariable("currentDateEvent")].scriptName;
  }

  public string GetCurrentObservationEventName() {
    if (GetCurrentObservationEvent() == null) {
      return "";
    }
    return observationSegments[VsnSaveSystem.GetIntVariable("currentDateEvent")].scriptName;
  }

  public void ShowDateUiPanel(bool value) {
    dateUiPanel.SetActive(value);
    UpdateUI();
  }

  public void ShowEngagementScreen(int babies) {
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
        firstButton = p.transform.GetChild(6).GetChild(0).GetComponent<Button>();
      }
    }

    Debug.LogWarning("SETTING SCREEN LAYOUT: " + state);
  
    switch(state) {
      case "hide_all":
        titleText.gameObject.SetActive(false);
        bgImage.SetActive(true);
        peoplePanel.HidePanel();
        couplesPanel.HidePanel();
        buttonsPanel.HidePanel();
        progressPanel.ShowPanel();
        observationMap.HidePanel();
        break;
      case "choose_observation_target":
        titleText.gameObject.SetActive(true);
        titleText.text = Lean.Localization.LeanLocalization.GetTranslationText("gameplay/title_1");
        bgImage.SetActive(true);
        peoplePanel.ShowPanel();
        couplesPanel.HidePanel();
        buttonsPanel.ShowPanel();
        progressPanel.ShowPanel();
        observationMap.HidePanel();
        break;
      case "observation":
        titleText.gameObject.SetActive(false);
        theater.SetEvent(TheaterEvent.observation);
        bgImage.SetActive(false);
        ShowOnlyObservedPerson();
        peoplePanel.HidePanel();
        couplesPanel.HidePanel();
        buttonsPanel.HidePanel();
        progressPanel.HidePanel();
        observationMap.HidePanel();
        break;
      case "observation_map":
        titleText.gameObject.SetActive(true);
        titleText.text = Lean.Localization.LeanLocalization.GetTranslationText("gameplay/title_2");
        bgImage.SetActive(true);
        peoplePanel.HidePanel();
        couplesPanel.HidePanel();
        buttonsPanel.HidePanel();
        playerTokenImage.sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.ObservedPerson().faceId);
        progressPanel.ShowPanel();
        observationMap.ShowPanel();
        break;
      case "choose_date_target":
        titleText.gameObject.SetActive(true);
        titleText.text = Lean.Localization.LeanLocalization.GetTranslationText("gameplay/title_3");
        bgImage.SetActive(true);
        peoplePanel.HidePanel();
        firstButton = UpdateCouplesPanelContent();
        couplesPanel.ShowPanel();
        buttonsPanel.ShowPanel();
        progressPanel.ShowPanel();
        observationMap.HidePanel();
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
        peoplePanel.HidePanel();
        couplesPanel.HidePanel();
        buttonsPanel.HidePanel();
        progressPanel.HidePanel();
        observationMap.HidePanel();
        break;
    }
    SetupContext(state, firstButton);
    UpdateUI();
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

  public void SetupObservationSegmentTiles(){
    GlobalData global = GlobalData.instance;
    ObservationTile selectedTile;
    List<ObservationEventType> allowedEventTypes = new List<ObservationEventType>();
    int selectedId;
    List<ObservationTile> tilesNotSet = new List<ObservationTile>();
    const int startingTile = 12;

    tilesNotSet.AddRange(observationTiles);
    tilesNotSet.Remove(observationTiles[startingTile]);


    allowedEventTypes.Add(ObservationEventType.attributeTraining);
    allowedEventTypes.Add(ObservationEventType.bet);
    allowedEventTypes.Add(ObservationEventType.homeStalking);
    allowedEventTypes.Add(ObservationEventType.itemOnSale);

    // add dateable people to observation board
    foreach(Person p in global.people) {
      if(global.ObservedPerson().isMale && !p.isMale && p.state != PersonState.shipped) {
        selectedTile = tilesNotSet[Random.Range(0, tilesNotSet.Count)];
        selectedTile.evt = global.GetEventOfType(ObservationEventType.femaleInTrouble);
        selectedTile.personInEvent = p;
        selectedTile.wasUsed = false;
        tilesNotSet.Remove(selectedTile);
      } else if(!global.ObservedPerson().isMale && p.isMale && p.state != PersonState.shipped) {
        selectedTile = tilesNotSet[Random.Range(0, tilesNotSet.Count)];
        selectedTile.evt = global.GetEventOfType(ObservationEventType.maleInTrouble);
        selectedTile.personInEvent = p;
        selectedTile.wasUsed = false;
        tilesNotSet.Remove(selectedTile);
      }
    }
    

    //SetScreenLayout("during_observation");

    foreach(ObservationTile tile in tilesNotSet) {
      do {
        selectedId = Random.Range(0, global.allObservationEvents.Count);
      } while (!allowedEventTypes.Contains(global.allObservationEvents[selectedId].eventType));
      tile.evt = global.allObservationEvents[selectedId];
      
      if(tile.evt.eventType == ObservationEventType.femaleInTrouble ||
         tile.evt.eventType == ObservationEventType.maleInTrouble) {
        tile.personInEvent = global.GetDateablePerson(global.ObservedPerson());
        Debug.Log("Dateable Person in tile: " + tile.personInEvent.name);
      }else{
        tile.personInEvent = null;
      }
      tile.wasUsed = false;
    }

    observationTiles[startingTile].wasUsed = true;
    playerToken.transform.position = observationTiles[startingTile].transform.position; // starting player position

    JoystickController.instance.AddContext(observationMap.context);
  }

  public void WalkToObservationTile(ObservationTile tile){
    playerToken.transform.DOMove(tile.transform.position, 1f).OnComplete( ()=> {
      tile.wasUsed = true;
      VsnController.instance.StartVSN("observation");
    } );
  }
}
