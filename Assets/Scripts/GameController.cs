using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour {

  public static GameController instance;
  public PersonCard[] personCards;
  public TextMeshProUGUI dayText;
  public TextMeshProUGUI apText;
  
  public int maxAp;
  public int ap;

  public int day;
  public int maxDays;
  public int objective;

  public TextAsset observationEventsFile;
  public List<ObservationEvent> allObservationEvents;
  public ObservationEvent[] observationSegments;

  public TextAsset dateEventsFile;
  public List<DateEvent> allDateEvents;
  public DateEvent[] dateSegments;

  public Slider progressSlider;
  public TextMeshProUGUI progressText;
  public TextMeshProUGUI objectiveText;
  public TextMeshProUGUI moneyText;

  public GameObject buttonsPanel;
  public GameObject miniFertililel;

  public ItemSelectorScreen itemSelectorScreen;

  public GameObject dateUiPanel;
  public Toggle[] dateEventToggles;
  public Image[] successIcons;
  public Image[] failIcons;
  public Image[] unresolvedIcons;



  public void Awake() {
    instance = this;
  }

  public void Start() {
    VsnSaveSystem.SetVariable("objective", objective);
    VsnSaveSystem.SetVariable("max_days", maxDays);
    GlobalData.instance.InitializeChapter();
    Initialize();
    UpdateUI();
    VsnAudioManager.instance.PlayMusic("observacao_intro", "observacao_loop");
    VsnController.instance.StartVSN("tutorial_intro");
  }

  public void Initialize(){
    day = 0;

    InitializeDateEvents();
    InitializeObservationEvents();

    PassDay();
  }

  public void InitializeDateEvents() {
    allDateEvents = new List<DateEvent>();

    int id, guts, intelligence, charisma, stage;
    string location;
    DateEventInteractionType interaction = DateEventInteractionType.male;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(dateEventsFile, 1);
    foreach (Dictionary<string, string> dic in spreadsheetData.data) {
      id = int.Parse(dic["Id"]);
      guts = int.Parse(dic["Dificuldade Guts"]);
      intelligence = int.Parse(dic["Dificuldade Intelligence"]);
      charisma = int.Parse(dic["Dificuldade Charisma"]);
      location = dic["Localidade"];
      stage = int.Parse(dic["Etapa"]);
      switch (dic["Tipo de Interação"]) {
        case "male":
          interaction = DateEventInteractionType.male;
          break;
        case "female":
          interaction = DateEventInteractionType.female;
          break;
        case "couple":
          interaction = DateEventInteractionType.couple;
          break;
        case "compatibility":
          interaction = DateEventInteractionType.compatibility;
          break;
      }
      allDateEvents.Add(new DateEvent(id, dic["Nome do Script"], guts, intelligence, charisma, stage, location, interaction));
    }
  }

  public void InitializeObservationEvents() {
    allObservationEvents = new List<ObservationEvent>();

    int eventId;
    int value = 0;
    int aux;
    ObservationEventType interaction = ObservationEventType.otherGenderPerson;
    Attributes relevantAttribute = Attributes.guts;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(observationEventsFile, 1);
    foreach (Dictionary<string, string> dic in spreadsheetData.data) {
      eventId = int.Parse(dic["Id"]);
      switch (dic["Interação"]) {
        case "otherGenderPerson":
          interaction = ObservationEventType.otherGenderPerson;
          break;
        case "attributeTraining":
          interaction = ObservationEventType.attributeTraining;
          break;
        case "sameGenderPerson":
          interaction = ObservationEventType.sameGenderPerson;
          break;
        case "itemOnSale":
          interaction = ObservationEventType.itemOnSale;
          break;
        case "homeStalking":
          interaction = ObservationEventType.homeStalking;
          break;
      }

      switch (dic["Atributo"]) {
        case "guts":
          relevantAttribute = Attributes.guts;
          break;
        case "intelligence":
          relevantAttribute = Attributes.intelligence;
          break;
        case "charisma":
          relevantAttribute = Attributes.charisma;
          break;
      }
      if(int.TryParse(dic["Valor"], out aux)){
        value = int.Parse(dic["Valor"]);
      }   

      allObservationEvents.Add(new ObservationEvent {
        id = eventId,
        eventType = interaction,
        scriptName = dic["Nome do Script"],

        personInEvent = GlobalData.instance.people[1],
        //itemInEvent

        //itemCategory
        challengedAttribute = relevantAttribute,
        challengeDifficulty = value
      });
    }
  }

  public void PassDay(){
    ap = maxAp;
    day++;
    UpdateUI();
    VsnController.instance.StartVSN("check_game_end");
  }

  public void Update() {
    if(Input.GetKeyDown(KeyCode.F5)){
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
  }


  public bool SpendAP(int cost){
    if(ap < cost){
      return false;
    }  
    ap -= cost;
    UpdateUI();
    return true;
  }
  

  public void UpdateUI() {
    int coupleId = GlobalData.instance.currentCouple;
    personCards[0].Initialize(GlobalData.instance.people[coupleId * 2]);
    personCards[1].Initialize(GlobalData.instance.people[coupleId * 2 + 1]);
    if (VsnSaveSystem.GetStringVariable("language") == "pt_br") {
      dayText.text = "Dia " + day + " /" + VsnSaveSystem.GetIntVariable("max_days");
    }else{
      dayText.text = "Day " + day + " /" + VsnSaveSystem.GetIntVariable("max_days");
    }
    apText.text = "AP: " + ap+" /"+maxAp;
    progressText.text = GlobalData.instance.shippedCouples.Count.ToString();
    progressSlider.value = GlobalData.instance.shippedCouples.Count;
    progressSlider.maxValue = VsnSaveSystem.GetIntVariable("objective");
    objectiveText.text = "/"+ VsnSaveSystem.GetIntVariable("objective");
    moneyText.text = (VsnSaveSystem.GetIntVariable("money")).ToString();
    personCards[0].UpdateUI();
    personCards[1].UpdateUI();

    bool showButtons = (VsnSaveSystem.GetIntVariable("hide_buttons") == 0);
    buttonsPanel.SetActive(showButtons);

    bool showMiniFertiliel = (VsnSaveSystem.GetIntVariable("show_mini_fertiliel") == 1);
    miniFertililel.SetActive(showMiniFertiliel);

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
    int dateSize = Mathf.Min(allDateEvents.Count, 7);
    List<int> selectedEvents = new List<int>();
    int selectedId;
    int dateLocation = Random.Range(0, 2);
    //int dateLocation = Random.Range(0, 1);

    VsnSaveSystem.SetVariable("date_location", dateLocation);

    dateSegments = new DateEvent[dateSize];
    for(int i=0; i<dateSize; i++){
      do {
        selectedId = Random.Range(0, allDateEvents.Count);

        Debug.LogWarning("selected location: " + allDateEvents[selectedId].location);
        Debug.LogWarning("date location: " + ((DateLocation)dateLocation).ToString());

      } while (selectedEvents.Contains(selectedId) ||
              (string.Compare(allDateEvents[selectedId].location, ((DateLocation)dateLocation).ToString())!=0 &&
               string.Compare(allDateEvents[selectedId].location, "generico")!=0) );
      dateSegments[i] = allDateEvents[selectedId];
      selectedEvents.Add(selectedId);
    }
    System.Array.Sort(dateSegments, new System.Comparison<DateEvent>(
                                  (event1, event2) => event1.stage.CompareTo(event2.stage) ));
  }
  
  public void GenerateObservation() {
    int observationSize = Mathf.Min(allObservationEvents.Count, 5);
    List<int> selectedEvents = new List<int>();
    int selectedId;

    observationSegments = new ObservationEvent[observationSize];
    for (int i = 0; i < observationSize; i++) {
      do {
        selectedId = Random.Range(0, allObservationEvents.Count);
      } while (selectedEvents.Contains(selectedId));
      observationSegments[i] = allObservationEvents[selectedId];
      selectedEvents.Add(selectedId);
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

  public void ShowButtons(bool value){
    buttonsPanel.SetActive(value);
  }

  public void ShowDateUiPanel(bool value) {
    dateUiPanel.SetActive(value);
    UpdateUI();
  }




  public void ClickSelectNewCouple(){
    if(ap >= 1){
      GlobalData.instance.SelectNewCouple();
      SpendAP(1);
      VsnController.instance.StartVSN("change_couple");
    }else{
      VsnController.instance.StartVSN("not_enough_ap");
    }
  }
}
