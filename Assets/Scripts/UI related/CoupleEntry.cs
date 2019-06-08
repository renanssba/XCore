using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoupleEntry : MonoBehaviour {

  public PersonCard[] coupleCards;
  //public Person[] couple;

  public void Initialize(Person a, Person b) {
    //couple[0] = a;
    //couple[1] = b;
    coupleCards[0].Initialize(a);
    coupleCards[1].Initialize(b);
  }

  public void OnEnable() {
    UpdateUI();
  }


  public void UpdateUI() {
    foreach(PersonCard p in coupleCards) {
      p.UpdateUI();
    }
  }

  public void ClickedDateButton() {
    Debug.LogWarning("Clicked date button to "+ coupleCards[0].person.name +" and " + coupleCards[1].person.name);
    GlobalData.instance.observedPeople[0] = coupleCards[0].person;
    GlobalData.instance.observedPeople[1] = coupleCards[1].person;
    VsnController.instance.StartVSN("date");
  }
}
