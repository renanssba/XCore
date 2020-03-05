using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

  public List<Person> people;
  public Relationship[] relationships;

  public Person[] observedPeople;

  public int boysToGenerate = 5;
  public int girlsToGenerate = 5;

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

    observedPeople = new Person[1];
  }

  public void InitializeChapter(){
    Person newPerson;
    List<string> usedNames = new List<string>();
    string auxName;

    people = new List<Person>();
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
  }

  public void InitializeChapterAlpha() {
    boysToGenerate = 1;
    girlsToGenerate = 3;

    people = new List<Person>();
    Person p = new Person() {
      name = "Ricardo",
      isMale = true,
      id = 0,
      faceId = 0,
      attributes = new int[] { 5, 5, 5, 4 },
      skillIds = new int[] { 0, 1, 2, 10 }
    };
    people.Add(p);
    p = new Person() {
      name = "Ana",
      isMale = false,
      id = 1,
      faceId = 5,
      attributes = new int[] { 6, 3, 2, 8 },
      skillIds = new int[] { 0, 1, 2, 11 }
    };
    people.Add(p);
    p = new Person() {
      name = "'B'",
      isMale = false,
      id = 2,
      faceId = 10,
      attributes = new int[] { 4, 9, 4, 1 },
      skillIds = new int[] { 0, 1, 2, 12 }
    };
    people.Add(p);
    p = new Person() {
      name = "Clara",
      isMale = false,
      id = 3,
      faceId = 7,
      attributes = new int[] { 4, 4, 7, 4 },
      skillIds = new int[] { 0, 1, 2, 13 }
    };
    people.Add(p);

    p = new Person() {
      name = "Fertiliel",
      isMale = false,
      isHuman = false,
      id = 10,
      faceId = 11,
      attributes = new int[] { 2, 2, 2, 2 },
      skillIds = new int[] { 9 }
    };
    people.Add(p);

    ResourcesManager.instance.GenerateCharacterSprites(new string[] {"ricardo", "ana", "beatrice", "clara", "fertiliel"});


    relationships = new Relationship[3];
    for(int i = 0; i < girlsToGenerate; i++) {
      relationships[i] = new Relationship {
        people = new Person[] {people[0], people[i+1]}
      };
    }


    /// INITIAL INVENTORIES
    /// PLAYER
    //people[0].inventory.AddItem("sports_clothes", 5);
    people[0].inventory.AddItem("chocolate_cake", 5);
    //people[0].inventory.AddItem("strawberry_cake", 5);
    //people[0].inventory.AddItem("pepper_cake", 5);

    /// ANA
    people[1].inventory.AddItemWithOwnership("old_teddy_bear", 1, 1);
    people[1].inventory.AddItemWithOwnership("delicate_key", 1, 1);
    people[1].inventory.AddItem("sports_clothes", 1);
    //people[1].inventory.AddItemWithOwnership("sports_clothes", 1, 1);

    /// BEATRICE
    people[2].inventory.AddItemWithOwnership("delicate_key", 1, 2);
    people[2].inventory.AddItemWithOwnership("script_drafts", 1, 2);
    people[2].inventory.AddItemWithOwnership("fancy_dagger", 1, 2);
    people[2].inventory.AddItem("dark_dress", 1);

    /// CLARA
    people[3].inventory.AddItemWithOwnership("flower_dress", 2, 3);
    people[3].inventory.AddItemWithOwnership("delicate_key", 1, 3);
    people[3].inventory.AddItem("chocolate_cake", 1);


    VsnSaveSystem.SetVariable("money", 500);


    /// DEBUG: TESTING SKILLS IN BATTLE
    //relationships[0].hearts = 1;
    //relationships[1].hearts = 1;
    //relationships[2].hearts = 1;
  }


  public string GetNewName(List<string> usedNames, bool isBoy) {
    string newName;

    do {
      newName = (isBoy) ? Utils.GetRandomBoyName() : Utils.GetRandomGirlName();
    } while (usedNames.Contains(newName));

    usedNames.Add(newName);
    return newName;
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

  public int CurrentCharacterAttribute(int attr){

    if(BattleController.instance.GetCurrentDateEvent() == null) {
      return 0;
    }

    int personId = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    if(personId >= BattleController.instance.partyMembers.Length) {
      return 0;
    }

    Person currentCharacter = BattleController.instance.partyMembers[personId];

    return currentCharacter.AttributeValue(attr);
  }

  public void PassTime() {
    int daytime = VsnSaveSystem.GetIntVariable("daytime");
    if(daytime >= 2) {
      VsnSaveSystem.SetVariable("daytime", 0);
      day++;
    } else {
      VsnSaveSystem.SetVariable("daytime", daytime+1);
    }
    UIController.instance.UpdateUI();
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
      if(r.level >= 2) {
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


  public void AddExpForRelationship(Relationship rel, int expToAdd) {
    UIController.instance.relationshipUpAnimationCard.Initialize(rel);
    UIController.instance.relationshipUpAnimationCard.RaiseExp(expToAdd);
  }

  public void AddBondSkill(int relationshipId, int skillId) {
    relationships[relationshipId].bondSkills.Add(skillId);
  }

  public Sprite GetFaceByName(string name) {
    if(name == "Fertiliel") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[0];
    }
    if(name == "Graciel") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[1];
    }
    //if(name == "Hardiel") {
    //  return ResourcesManager.instance.fixedCharactersFaceSprites[2];
    //}
    if(name == "Carta") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[3];
    }
    foreach(Person p in people) {
      if(p.name == name) {
        return ResourcesManager.instance.faceSprites[p.faceId];
      }      
    }
    return null;
  }
}
