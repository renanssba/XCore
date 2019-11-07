using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

  public List<Person> people;
  public Relationship[] relationships;
  public List<int> shippedCouples;

  public Person[] observedPeople;

  public List<ObservationEvent> allObservationEvents;
  public TextAsset observationEventsFile;
  public List<DateEvent> allDateEvents;
  public TextAsset dateEventsFile;

  public int boysToGenerate = 5;
  public int girlsToGenerate = 5;

  public int day;
  public int maxDays;
  public int objective;

  public int currentDateHearts;
  public int maxDateHearts;

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

    observedPeople = new Person[1];
  }

  public void InitializeChapter(){
    Person newPerson;
    List<string> usedNames = new List<string>();
    string auxName;

    people = new List<Person>();
    ResetCurrentCouples();


    relationships = new Relationship[boysToGenerate + girlsToGenerate -1];

    VsnSaveSystem.SetVariable("money", 0);
    VsnSaveSystem.SetVariable("objective", objective);
    VsnSaveSystem.SetVariable("max_days", maxDays);
    VsnSaveSystem.SetVariable("observation_played", 0);
    day = 1;

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

    InitializeChapterAlpha();

    observedPeople[0] = people[0];

    InitializeDateEvents();
    InitializeObservationEvents();
  }

  public void InitializeChapterAlpha() {
    boysToGenerate = 1;
    girlsToGenerate = 3;

    people = new List<Person>();
    Person p = new Person() {
      name = "Ricardo",
      isMale = true,
      id = 0,
      faceId = 4,
      attributes = new int[]{2, 3, 1},
      skillId = 10
  };
    people.Add(p);
    p = new Person() {
      name = "Ana",
      isMale = false,
      id = 1,
      faceId = 9,
      attributes = new int[] {4, 1, 1},
      skillId = 11
    };
    people.Add(p);
    p = new Person() {
      name = "'B'",
      isMale = false,
      id = 2,
      faceId = 10,
      attributes = new int[] {1, 4, 1},
      skillId = 12
    };
    people.Add(p);
    p = new Person() {
      name = "Clara",
      isMale = false,
      id = 3,
      faceId = 5,
      attributes = new int[] {2, 2, 3},
      skillId = 13
    };
    people.Add(p);


    relationships = new Relationship[3];
    for(int i = 0; i < girlsToGenerate; i++) {
      relationships[i] = new Relationship {
        people = new Person[] {people[0], people[i+1]}
      };
    }
    //relationships[0].hearts = 1;


    /// INITIAL INVENTORIES
    /// ANA
    people[1].inventory.AddItemWithOwnership("old_teddy_bear", 1, 1);
    people[1].inventory.AddItemWithOwnership("delicate_key", 1, 1);
    people[1].inventory.AddItem("sports_clothes", 1);
    //people[1].inventory.AddItemWithOwnership("sports_clothes", 2, 1);

    /// BEATRICE
    people[2].inventory.AddItemWithOwnership("delicate_key", 1, 2);
    people[2].inventory.AddItemWithOwnership("script_drafts", 1, 2);
    people[2].inventory.AddItemWithOwnership("fancy_dagger", 1, 2);
    people[2].inventory.AddItem("dark_dress", 1);

    /// CLARA
    //people[3].inventory.AddItemWithOwnership("delicate_key", 1, 1);
    people[3].inventory.AddItem("flower_dress", 2);


    /// DEBUG: TESTING SKILLS IN BATTLE
    relationships[0].hearts = 3;
    relationships[1].hearts = 3;
    relationships[2].hearts = 3;
  }

  public void InitializeDateEvents() {
    allDateEvents = new List<DateEvent>();

    float guts, intelligence, charisma;
    DateEventInteractionType interaction = DateEventInteractionType.male;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(dateEventsFile, 1);
    foreach (Dictionary<string, string> dic in spreadsheetData.data) {
      guts = GetEffectivityByName(dic["Efetividade Valentia"]);
      intelligence = GetEffectivityByName(dic["Efetividade Inteligencia"]);
      charisma = GetEffectivityByName(dic["Efetividade Carisma"]);
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
      allDateEvents.Add(new DateEvent {
        id = int.Parse(dic["Id"]),
        scriptName = dic["Nome do Script"],
        difficulty = int.Parse(dic["Dificuldade"]),
        attributeEffectivity = new float[3] { guts, intelligence, charisma},
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
    if(observedPeople.Length < 2) {
      return "";
    }
    return Lean.Localization.LeanLocalization.GetTranslationText("char_name/couple");
  }

  public Person CurrentBoy(){
    if(observedPeople.Length < 1) {
      return null;
    }
    return observedPeople[0];
  }

  public Person CurrentGirl() {
    if(observedPeople.Length < 2) {
      return null;
    }
    return observedPeople[1];
  }

  public Person ObservedPerson(){
    if(observedPeople.Length<1) {
      return null;
    }
    return observedPeople[0];
  }

  public Person EncounterPerson() {
    if(observedPeople.Length < 2) {
      return null;
    }
    return observedPeople[1];
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
            return (CurrentGirl().AttributeValue(attr) + CurrentBoy().AttributeValue(attr));
        }
        break;
    }
    
    return 0;
  }

  public void PassTime() {
    int daytime = VsnSaveSystem.GetIntVariable("daytime");
    if(daytime >= 2) {
      VsnSaveSystem.SetVariable("daytime", 0);
      day++;
    } else {
      VsnSaveSystem.SetVariable("daytime", daytime+1);
    }
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

  public Relationship[] GetCurrentDateableCouples() {
    List<Relationship> availableCouples = new List<Relationship>();

    foreach(Relationship r in relationships) {
      if(r.hearts >= 2) {
        availableCouples.Add(r);
      }
    }

    return availableCouples.ToArray();
  }

  public Relationship GetCurrentRelationship() {
    if(CurrentGirl() == null) {
      return null;
    }
    foreach(Relationship relationship in relationships) {
      if(relationship.GetGirl() == CurrentGirl()) {
        return relationship;
      }
    }
    return null;
  }

  public ObservationEvent GetEventOfType(ObservationEventType type) {
    ObservationEvent evt;
    do {
      evt = allObservationEvents[Random.Range(0, allObservationEvents.Count)];
    } while(evt.eventType != type);
    return evt;
  }

  public void UnlockDateableCouple(Person a, Person b) {
    //if(a.isMale) {
    //  viableCouple[a.id, b.id - boysToGenerate] = true;
    //} else {
    //  viableCouple[b.id, a.id - boysToGenerate] = true;
    //}
  }

  public void AddHeart(int relationshipId) {
    relationships[relationshipId].hearts++;
  }

  public void AddBondSkill(int relationshipId, int skillId) {
    relationships[relationshipId].bondSkills.Add(skillId);
  }

  public Sprite GetFaceByName(string name) {
    if(name == "Fertiliel") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[0];
    }
    if(name == "Carta") {
      return ResourcesManager.instance.faceSprites[10];
    }
    foreach(Person p in people) {
      if(p.name == name) {
        return ResourcesManager.instance.faceSprites[p.faceId];
      }      
    }
    return null;
  }
}
