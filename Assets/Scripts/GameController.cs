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

  public ScreenTransitions peoplePanel;
  public ScreenTransitions buttonsPanel;
  public ScreenTransitions observationMap;
  //public GameObject miniFertililel;

  public ItemSelectorScreen itemSelectorScreen;

  public GameObject dateUiPanel;
  public Toggle[] dateEventToggles;
  public Image[] successIcons;
  public Image[] failIcons;
  public Image[] unresolvedIcons;

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

      if(GlobalData.instance.hideTutorials) {
        VsnSaveSystem.SetVariable("tutorial_date", 1);
        VsnSaveSystem.SetVariable("tutorial_date2", 1);
      }
      VsnController.instance.StartVSN("tutorial_intro");
    }
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
    int coupleId = GlobalData.instance.currentCouple;
    personCards[0].Initialize(GlobalData.instance.people[coupleId * 2]);
    personCards[1].Initialize(GlobalData.instance.people[coupleId * 2 + 1]);
    if (VsnSaveSystem.GetStringVariable("language") == "pt_br") {
      dayText.text = "Dia " + gb.day + " /" + VsnSaveSystem.GetIntVariable("max_days");
    }else{
      dayText.text = "Day " + gb.day + " /" + VsnSaveSystem.GetIntVariable("max_days");
    }
    apText.text = "AP: " + gb.ap + " /" + gb.maxAp;
    progressText.text = GlobalData.instance.shippedCouples.Count.ToString();
    progressSlider.value = GlobalData.instance.shippedCouples.Count;
    progressSlider.maxValue = VsnSaveSystem.GetIntVariable("objective");
    objectiveText.text = "/"+ VsnSaveSystem.GetIntVariable("objective");
    moneyText.text = (VsnSaveSystem.GetIntVariable("money")).ToString();
    personCards[0].UpdateUI();
    personCards[1].UpdateUI();

    energyText.text = "Energia: " + VsnSaveSystem.GetIntVariable("observation_energy");

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
    engagementScreenImages[0].sprite = personCards[0].faceImage.sprite;
    engagementScreenImages[1].sprite = personCards[1].faceImage.sprite;
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
      if(p.person == GlobalData.instance.ObservedPerson()){
        p.gameObject.SetActive(true);
        p.GetComponent<CanvasGroup>().alpha = 1f;
      } else{
        p.gameObject.SetActive(false);
      }
    }
  }

  public void SetScreenLayout(string state){
    personCards[0].gameObject.SetActive(true);
    personCards[1].gameObject.SetActive(true);
  
    switch (state) {
      case "hide":
        peoplePanel.HidePanel();
        buttonsPanel.HidePanel();
        observationMap.HidePanel();
        break;
      case "show":
        personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
        personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
        peoplePanel.ShowPanel();
        buttonsPanel.ShowPanel();
        observationMap.HidePanel();
        break;
      case "observation":
        ShowOnlyObservedPerson();
        peoplePanel.ShowPanel();
        buttonsPanel.HidePanel();
        observationMap.HidePanel();
        break;
      case "during_observation":
        peoplePanel.HidePanel();
        buttonsPanel.HidePanel();
        playerTokenImage.sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.ObservedPerson().faceId);
        observationMap.ShowPanel();
        break;
      case "date":
        personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
        personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
        peoplePanel.ShowPanel();
        buttonsPanel.HidePanel();
        observationMap.HidePanel();
        break;
      case "event":
        peoplePanel.ShowPanel();
        buttonsPanel.HidePanel();
        observationMap.HidePanel();
        switch (GetCurrentDateEvent().interactionType) {
          case DateEventInteractionType.male:
            personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
            personCards[1].GetComponent<CanvasGroup>().alpha = 0.65f;
            break;
          case DateEventInteractionType.female:
            personCards[0].GetComponent<CanvasGroup>().alpha = 0.65f;
            personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
            break;
          case DateEventInteractionType.couple:
            personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
            personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
            break;
        }
        break;
    }
    UpdateUI();
  }

  public void HidePeople(){
    foreach (PersonCard p in personCards) {
      p.gameObject.SetActive(false);
    }
  }

  public void SetupObservationSegmentTiles(){
    List<ObservationEventType> allowedEventTypes = new List<ObservationEventType>();
    int selectedId;
    
    allowedEventTypes.Add(ObservationEventType.attributeTraining);
    allowedEventTypes.Add(ObservationEventType.bet);
    allowedEventTypes.Add(ObservationEventType.homeStalking);
    allowedEventTypes.Add(ObservationEventType.itemOnSale);
    if (GlobalData.instance.ObservedPerson().isMale) {
      allowedEventTypes.Add(ObservationEventType.femaleInTrouble);
    } else {
      allowedEventTypes.Add(ObservationEventType.maleInTrouble);
    }

    //SetScreenLayout("during_observation");

    foreach (ObservationTile tile in observationTiles) {
      do {
        selectedId = Random.Range(0, GlobalData.instance.allObservationEvents.Count);
      } while (!allowedEventTypes.Contains(GlobalData.instance.allObservationEvents[selectedId].eventType));
      tile.evt = GlobalData.instance.allObservationEvents[selectedId];
      
      if(tile.evt.eventType == ObservationEventType.femaleInTrouble ||
         tile.evt.eventType == ObservationEventType.maleInTrouble) {
        tile.personInEvent = GlobalData.instance.GetDateablePerson(GlobalData.instance.ObservedPerson());
        Debug.Log("Dateable Person in tile: " + tile.personInEvent.name);
      }else{
        tile.personInEvent = null;
      }
      tile.wasUsed = false;
    }

    observationTiles[14].wasUsed = true;
    playerToken.transform.position = observationTiles[14].transform.position; // starting player position
  }

  public void WalkToObservationTile(ObservationTile tile){
    playerToken.transform.DOMove(tile.transform.position, 1f).OnComplete( ()=> {
      tile.wasUsed = true;
      VsnController.instance.StartVSN("observation");
    } );
  }




  public void ClickSelectNewCouple(){
    if(GlobalData.instance.ap >= 1){
      GlobalData.instance.SelectNewCouple();
      SpendAP(1);
      VsnController.instance.StartVSN("change_couple");
    }else{
      VsnController.instance.StartVSN("not_enough_ap");
    }
  }
}
