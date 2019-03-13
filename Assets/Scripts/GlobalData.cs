using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

  public List<Person> people;
  public int currentCouple;
  public List<int> shippedCouples;

  public static GlobalData instance;

  void Awake () {
    instance = this;
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
    return people[currentCouple * 2].name + " e " + people[currentCouple * 2+1].name;
  }

  public Person GetCurrentBoy(){
    return people[currentCouple * 2];
  }

  public Person GetCurrentGirl() {
    return people[currentCouple * 2+1];
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
