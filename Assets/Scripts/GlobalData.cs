﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

  public List<Person> people;
  public int currentCouple;
  public List<int> shippedCouples;
  public Inventory inventory;

  public Person[] currentObservationPeople;

  public List<ObservationEvent> allObservationEvents;
  public TextAsset observationEventsFile;
  public List<DateEvent> allDateEvents;
  public TextAsset dateEventsFile;

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
      if (VsnSaveSystem.GetStringVariable("language") == "") {
        VsnSaveSystem.SetVariable("language", "pt_br");
      }
    } else if (instance != this) {
      Destroy(gameObject);
      return;
    }
    DontDestroyOnLoad(gameObject);

    inventory = new Inventory();
  }

  public void InitializeChapter(){
    Person newPerson;
    List<string> usedNames = new List<string>();
    string auxName;

    people = new List<Person>();
    currentObservationPeople = new Person[2];
    currentCouple = 0;
    ResetCurrentCouples();

    VsnSaveSystem.SetVariable("money", 0);
    VsnSaveSystem.SetVariable("objective", objective);
    VsnSaveSystem.SetVariable("max_days", maxDays);
    day = 0;
    inventory.items.Clear();


    for (int i = 0; i < 5; i++) {
      auxName = GetNewName(usedNames, true);
      if(ModsManager.instance.setNames != null){
        //Debug.Log("ModsManager.instance.setNames is not null");
        auxName = ModsManager.instance.setNames[0];
      }
      newPerson = new Person { isMale = true, name = auxName};
      newPerson.Initialize();
      people.Add(newPerson);

      auxName = GetNewName(usedNames, false);
      if (ModsManager.instance.setNames != null) {
        auxName = ModsManager.instance.setNames[1];
      }
      newPerson = new Person { isMale = false, name = auxName };
      newPerson.Initialize();
      people.Add(newPerson);
    }
    usedNames.Clear();


    InitializeDateEvents();
    InitializeObservationEvents();
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
    shippedCouples.Add(currentCouple);
    SelectNewCouple();
  }

  public void ResetCurrentCouples(){
    shippedCouples.Clear();
  }

  public string CurrentCoupleName(){
    if(VsnSaveSystem.GetStringVariable("language")=="pt_br"){
      return people[currentCouple * 2].name + " e " + people[currentCouple * 2 + 1].name;
    }else{
      return people[currentCouple * 2].name + " and " + people[currentCouple * 2 + 1].name;
    }
  }

  public Person CurrentBoy(){
    return people[currentCouple * 2];
  }

  public Person CurrentGirl() {
    return people[currentCouple * 2+1];
  }

  public Person ObservedPerson(){
    return currentObservationPeople[0];
  }

  public Person EncounterPerson() {
    if(GameController.instance.GetCurrentObservationEvent() != null) {
      return GameController.instance.GetCurrentObservationEvent().personInEvent;
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

  public void SelectNewCouple(){
    if(shippedCouples.Count >= people.Count/2){
      currentCouple = -1;
    }

    do{
      currentCouple++;
      if(currentCouple> (people.Count/2-1) )
        currentCouple = 0;
    }while (shippedCouples.Contains(currentCouple));

    GameController.instance.UpdateUI();
  }

  public void PassDay() {
    ap = maxAp;
    day++;
    GameController.instance.UpdateUI();
    VsnController.instance.StartVSN("check_game_end");
  }

  public Person GetDateablePerson(Person p){
    List<Person> dateable = new List<Person>();
    foreach(Person p2 in people){
      if(p.isMale != p2.isMale){
        dateable.Add(p2);
      }
    }
    
    if (dateable.Count > 0) {
      return dateable[Random.Range(0, dateable.Count)];
    } else {
      return null;
    }
  }


}
