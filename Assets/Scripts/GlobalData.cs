using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

  public List<Person> people;
  public bool[,] viableCouple;
  public List<int> shippedCouples;
  public Inventory inventory;

  public Person[] observedPeople;

  public List<ObservationEvent> allObservationEvents;
  public TextAsset observationEventsFile;
  public List<DateEvent> allDateEvents;
  public TextAsset dateEventsFile;

  public int boysToGenerate = 5;
  public int girlsToGenerate = 5;

  public int maxAp;
  public int ap;

  public int day;
  public int maxDays;
  public int objective;

  public bool hideTutorials = false;



  public static GlobalData instance;

  void Awake () {
    if (instance == null) {
      instance = this;
    } else if (instance != this) {
      Destroy(gameObject);
      return;
    }
    DontDestroyOnLoad(gameObject);

    observedPeople = new Person[2];
    inventory = new Inventory();
  }

  public void InitializeChapter(){
    Person newPerson;
    List<string> usedNames = new List<string>();
    string auxName;

    people = new List<Person>();
    ResetCurrentCouples();


    viableCouple = new bool[boysToGenerate, girlsToGenerate];
    for(int i = 0; i < boysToGenerate; i++) {
      for(int j = 0; j < girlsToGenerate; j++) {
        viableCouple[i, j] = false;
      }
    }
    //viableCouple[0, 0] = true;

    VsnSaveSystem.SetVariable("money", 0);
    VsnSaveSystem.SetVariable("objective", objective);
    VsnSaveSystem.SetVariable("max_days", maxDays);
    VsnSaveSystem.SetVariable("observation_played", 0);
    day = 0;
    inventory.items.Clear();


    for(int i = 0; i < boysToGenerate; i++) {
      auxName = GetNewName(usedNames, true);
      if(ModsManager.instance.GetName(i) != null){
        auxName = ModsManager.instance.GetName(2 * i);
      }
      newPerson = new Person { isMale = true, name = auxName};
      newPerson.Initialize(i);
      people.Add(newPerson);
    }
    for(int i = 0; i < girlsToGenerate; i++) {
      auxName = GetNewName(usedNames, false);
      if(ModsManager.instance.GetName(5+i) != null){
        auxName = ModsManager.instance.GetName(5 + i);
      }
      newPerson = new Person { isMale = false, name = auxName };
      newPerson.Initialize(5 + i);
      people.Add(newPerson);
    }
    usedNames.Clear();

    people[0].state = PersonState.available;
    people[5].state = PersonState.available;

    InitializeDateEvents();
    InitializeObservationEvents();
  }

  public void InitializeDateEvents() {
    allDateEvents = new List<DateEvent>();

    int id, guts, intelligence, charisma, stage;
    string location, spriteName;
    DateEventInteractionType interaction = DateEventInteractionType.male;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(dateEventsFile, 1);
    foreach (Dictionary<string, string> dic in spreadsheetData.data) {
      id = int.Parse(dic["Id"]);
      guts = int.Parse(dic["Dificuldade Guts"]);
      intelligence = int.Parse(dic["Dificuldade Intelligence"]);
      charisma = int.Parse(dic["Dificuldade Charisma"]);
      location = dic["Localidade"];
      spriteName = dic["Nome Sprite"];
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
      allDateEvents.Add(new DateEvent(id, dic["Nome do Script"], guts, intelligence, charisma, stage, location, spriteName, interaction));
    }
  }

  public void InitializeObservationEvents() {
    allObservationEvents = new List<ObservationEvent>();

    int eventId;
    int value = 0;
    int aux;
    ObservationEventType interaction = ObservationEventType.femaleInTrouble;
    Attributes relevantAttribute = Attributes.guts;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(observationEventsFile, 1);
    foreach (Dictionary<string, string> dic in spreadsheetData.data) {
      eventId = int.Parse(dic["Id"]);
      switch (dic["Interação"]) {
        case "femaleInTrouble":
          interaction = ObservationEventType.femaleInTrouble;
          break;
        case "maleInTrouble":
          interaction = ObservationEventType.maleInTrouble;
          break;
        case "attributeTraining":
          interaction = ObservationEventType.attributeTraining;
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
      if (int.TryParse(dic["Valor"], out aux)) {
        value = int.Parse(dic["Valor"]);
      }

      allObservationEvents.Add(new ObservationEvent {
        id = eventId,
        eventType = interaction,
        scriptName = dic["Nome do Script"],
        challengedAttribute = relevantAttribute,
        location = dic["Localidade"],
        challengeDifficulty = value
      });
    }
  }


  public string GetNewName(List<string> usedNames, bool isBoy) {
    string newName;

    do {
      newName = (isBoy) ? Utils.GetRandomBoyName() : Utils.GetRandomGirlName();
    } while (usedNames.Contains(newName));

    usedNames.Add(newName);
    return newName;
  }


  public void ShipCurrentCouple(){
    shippedCouples.Add(Random.Range(0,9999));
    CurrentBoy().state = PersonState.shipped;
    CurrentGirl().state = PersonState.shipped;
  }

  public void ResetCurrentCouples(){
    shippedCouples.Clear();
  }

  public string CurrentCoupleName(){
    if(observedPeople[0] == null || observedPeople[1]==null) {
      return "";
    }
    return Lean.Localization.LeanLocalization.GetTranslationText("char_name/couple");
  }

  public Person CurrentBoy(){
    return observedPeople[0];
  }

  public Person CurrentGirl() {
    return observedPeople[1];
  }

  public Person ObservedPerson(){
    return observedPeople[0];
  }

  public Person EncounterPerson() {
    if(observedPeople[1] != null) {
      return observedPeople[1];
    } else{
      return null;
    }    
  }

  public Personality CurrentPersonPersonality(){
    switch (GameController.instance.GetCurrentDateEvent().interactionType) {
      case DateEventInteractionType.male:
        return CurrentBoy().personality;
      case DateEventInteractionType.female:
        return CurrentGirl().personality;
      case DateEventInteractionType.couple:
        return CurrentGirl().personality;
    }
    return Personality.emotivo;
  }

  public int EventSolvingAttributeLevel(int attr){

    switch(VsnSaveSystem.GetIntVariable("situation")) {
      case 1:
        if (ObservedPerson() != null) {
          return ObservedPerson().attributes[attr];
        }
        break;
      case 2:
        if (GameController.instance.GetCurrentDateEvent() == null) {
          return 0;
        }
        switch (GameController.instance.GetCurrentDateEvent().interactionType) {
          case DateEventInteractionType.male:
            return CurrentBoy().AttributeValue(attr);
          case DateEventInteractionType.female:
            return CurrentGirl().AttributeValue(attr);
          case DateEventInteractionType.couple:
            return CurrentGirl().AttributeValue(attr) + CurrentBoy().AttributeValue(attr);
        }
        break;
    }
    
    return 0;
  }

  public void PassDay() {
    ap = maxAp;
    day++;
    VsnSaveSystem.SetVariable("observation_played", 0);
    GameController.instance.UpdateUI();
  }

  public Person GetDateablePerson(Person p){
    List<Person> dateable = new List<Person>();
    foreach(Person p2 in people){
      if(p.isMale != p2.isMale){
        dateable.Add(p2);
      }
    }
    
    if (dateable.Count > 0) {
      int selected = Random.Range(0, dateable.Count);
      //Debug.Log("Dateable: selected "+selected+" from "+ dateable.Count);
      return dateable[selected];
    } else {
      return null;
    }
  }

  public ObservationEvent GetEventOfType(ObservationEventType type) {
    ObservationEvent evt;
    do {
      evt = allObservationEvents[Random.Range(0, allObservationEvents.Count)];
    } while(evt.eventType != type);
    return evt;
  }

  public void UnlockDateableCouple(Person a, Person b) {
    if(a.isMale) {
      viableCouple[a.id, b.id - boysToGenerate] = true;
    } else {
      viableCouple[b.id, a.id - boysToGenerate] = true;
    }
  }
}
