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

  public TextAsset dateEventsFile;
  public List<DateEvent> allDateEvents;
  public DateEvent[] date;

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
    allDateEvents = new List<DateEvent>();

    int id;
    int guts;
    int intelligence;
    int charisma;
    EventInteractionType interaction = EventInteractionType.male;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(dateEventsFile, 1);
    foreach(Dictionary<string, string> dic in spreadsheetData.data) {
      id = int.Parse(dic["Id"]);
      guts = int.Parse(dic["Dificuldade Guts"]);
      intelligence = int.Parse(dic["Dificuldade Intelligence"]);
      charisma = int.Parse(dic["Dificuldade Charisma"]);
      switch(dic["Tipo de Interação"]){
        case "male":
          interaction = EventInteractionType.male;
          break;
        case "female":
          interaction = EventInteractionType.female;
          break;
        case "couple":
          interaction = EventInteractionType.couple;
          break;
        case "compatibility":
          interaction = EventInteractionType.compatibility;
          break;
      }
      allDateEvents.Add(new DateEvent(id, dic["Nome do Script"], guts, intelligence, charisma, interaction));
    }
    PassDay();
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
    dayText.text = "Dia " + day + " /" + VsnSaveSystem.GetIntVariable("max_days");
    apText.text = "AP: " + ap+" /"+maxAp;
    progressText.text = GlobalData.instance.shippedCouples.Count.ToString();
    progressSlider.value = GlobalData.instance.shippedCouples.Count;
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

    date = new DateEvent[dateSize];
    for(int i=0; i<dateSize; i++){
      do{
        selectedId = Random.Range(0, allDateEvents.Count);
      } while(selectedEvents.Contains(selectedId));
      date[i] = allDateEvents[selectedId];
      selectedEvents.Add(selectedId);
    }
  }

  public DateEvent GetCurrentEvent(){
    int currentDateEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if (date.Length <= currentDateEvent) {
      return null;
    }
    return date[currentDateEvent];
  }

  public string GetCurrentEventName() {
    if(GetCurrentEvent() == null) {
      return "";
    }
    return date[VsnSaveSystem.GetIntVariable("currentDateEvent")].scriptName;
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
