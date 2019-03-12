using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

  public List<Person> people;
  public int currentCouple;
  public List<int> matchedCouples;

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


  public void MatchCurrentCouple(){
    matchedCouples.Add(currentCouple);
    SelectNewCouple();
  }

  public void SelectNewCouple(){
    if(matchedCouples.Count >= people.Count/2){
      currentCouple = -1;
    }

    do{
      currentCouple++;
      if(currentCouple> (people.Count/2-1) )
        currentCouple = 0;
    }while (matchedCouples.Contains(currentCouple));

    GameController.instance.UpdateUI();
  }
}
