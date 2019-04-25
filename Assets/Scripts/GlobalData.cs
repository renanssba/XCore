using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

  public List<Person> people;
  public int currentCouple;
  public List<int> shippedCouples;
  public Inventory inventory;

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
    currentCouple = 0;

    for (int i = 0; i < 5; i++) {
      auxName = GetNewName(usedNames, true);
      newPerson = new Person { isMale = true, name = auxName};
      newPerson.Initialize();
      people.Add(newPerson);

      auxName = GetNewName(usedNames, false);
      newPerson = new Person { isMale = false, name = auxName };
      newPerson.Initialize();
      people.Add(newPerson);
    }
    usedNames.Clear();
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

  public string CurrentCoupleName(){
    if(VsnSaveSystem.GetStringVariable("language")=="pt_br"){
      return people[currentCouple * 2].name + " e " + people[currentCouple * 2 + 1].name;
    }else{
      return people[currentCouple * 2].name + " and " + people[currentCouple * 2 + 1].name;
    }
  }

  public Person GetCurrentBoy(){
    return people[currentCouple * 2];
  }

  public Person GetCurrentGirl() {
    return people[currentCouple * 2+1];
  }

  public Person GetCurrentObservationPerson(){
    return GetCurrentBoy();
  }

  public Personality CurrentPersonPersonality(){
    switch (GameController.instance.GetCurrentDateEvent().interactionType) {
      case DateEventInteractionType.male:
        return GetCurrentBoy().personality;
      case DateEventInteractionType.female:
        return GetCurrentGirl().personality;
      case DateEventInteractionType.couple:
        return GetCurrentGirl().personality;
    }
    return Personality.emotivo;
  }

  public int EventSolvingAttributeLevel(int attr){

    switch(VsnSaveSystem.GetIntVariable("situation")) {
      case 1:
        if (GetCurrentObservationPerson() != null) {
          return GetCurrentObservationPerson().attributes[attr];
        }
        break;
      case 2:
        if (GameController.instance.GetCurrentDateEvent() == null) {
          return 0;
        }
        switch (GameController.instance.GetCurrentDateEvent().interactionType) {
          case DateEventInteractionType.male:
            return GetCurrentBoy().AttributeValue(attr);
          case DateEventInteractionType.female:
            return GetCurrentGirl().AttributeValue(attr);
          case DateEventInteractionType.couple:
            return GetCurrentGirl().AttributeValue(attr) + GetCurrentBoy().AttributeValue(attr);
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
}
