using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {
  public static BattleController instance;

  public List<DateEvent> allDateEvents;
  public TextAsset dateEventsFile;

  public Person[] partyMembers;
  public int dateLength;

  public DateEvent[] dateSegments;


  public void Awake() {
    instance = this;
    InitializeDateEvents();
    dateLength = 3;
  }

  public void StartBattle() {
    UIController.instance.ShowPartyPeopleCards();
  }



  public void ShowDateUiPanel(bool value) {
    UIController.instance.dateUiPanel.SetActive(value);
    UIController.instance.UpdateUI();
  }



  public DateEvent GetCurrentDateEvent() {
    int currentDateEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if(dateSegments.Length <= currentDateEvent) {
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



  public void GenerateDate(int dateNumber = 1) {
    List<int> selectedEvents = new List<int>();
    int dateLocation = Random.Range(0, 2);
    VsnSaveSystem.SetVariable("date_location", dateLocation);

    switch(dateNumber) {
      case 1:
        dateLength = 3;
        break;
      case 2:
        dateLength = 5;
        break;
      case 3:
        dateLength = 7;
        break;
    }

    dateSegments = new DateEvent[dateLength];
    for(int i = 0; i < dateLength; i++) {
      int selectedId = GetNewDateEvent(selectedEvents);
      dateSegments[i] = allDateEvents[selectedId];
      selectedEvents.Add(selectedId);
    }
    System.Array.Sort(dateSegments, new System.Comparison<DateEvent>(
                                  (event1, event2) => event1.stage.CompareTo(event2.stage)));
    SetDifficultyForEvents();
  }

  public int GetNewDateEvent(List<int> selectedEvents) {
    int selectedId;
    string dateLocationName = ((DateLocation)VsnSaveSystem.GetIntVariable("date_location")).ToString();
    do {
      selectedId = Random.Range(0, allDateEvents.Count);

      Debug.LogWarning("selected location: " + allDateEvents[selectedId].location);
      Debug.LogWarning("date location: " + dateLocationName);

    } while(selectedEvents.Contains(selectedId) ||
            (string.Compare(allDateEvents[selectedId].location, dateLocationName) != 0 &&
             string.Compare(allDateEvents[selectedId].location, "generico") != 0));

    return selectedId;
  }

  public void SetDifficultyForEvents() {
    for(int i = 0; i < dateLength; i++) {
      if(i < 3) {
        dateSegments[i].difficulty = 4;
      } else if(i < 5) {
        dateSegments[i].difficulty = 5;
      } else {
        dateSegments[i].difficulty = 6;
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
    dateSegments[positionId] = allDateEvents[selectedId];
    currentUsedEvents.Clear();

    SetDifficultyForEvents();
  }




  public void InitializeDateEvents() {
    allDateEvents = new List<DateEvent>();

    float guts, intelligence, charisma;
    DateEventInteractionType interaction = DateEventInteractionType.male;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(dateEventsFile, 1);
    foreach(Dictionary<string, string> dic in spreadsheetData.data) {
      guts = GetEffectivityByName(dic["Efetividade Valentia"]);
      intelligence = GetEffectivityByName(dic["Efetividade Inteligencia"]);
      charisma = GetEffectivityByName(dic["Efetividade Carisma"]);
      switch(dic["Tipo de Interação"]) {
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
      allDateEvents.Add(new DateEvent {
        id = int.Parse(dic["Id"]),
        scriptName = dic["Nome do Script"],
        difficulty = int.Parse(dic["Dificuldade"]),
        attributeEffectivity = new float[3] { guts, intelligence, charisma },
        spriteName = dic["Nome Sprite"],
        stage = int.Parse(dic["Etapa"]),
        location = dic["Localidade"],
        interactionType = interaction
      });
    }
  }

  public float GetEffectivityByName(string name) {
    switch(name) {
      case "baixa":
        return 0.5f;
      case "normal":
        return 1f;
      case "super":
        return 2f;
    }
    return 1f;
  }
}
