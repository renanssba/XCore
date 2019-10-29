using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoupleEntry : MonoBehaviour {

  public Relationship relationship;
  public PersonCard[] coupleCards;
  public Image[] heartIcons;
  public Image[] skillIcons;
  public TextMeshProUGUI[] skillNameTexts;

  public void Initialize(Relationship rel) {
    relationship = rel;
    coupleCards[0].Initialize(rel.GetBoy());
    coupleCards[1].Initialize(rel.GetGirl());
  }

  public void OnEnable() {
    UpdateUI();
  }


  public void UpdateUI() {
    foreach(PersonCard pcard in coupleCards) {
      pcard.UpdateUI();
    }
    // heart icons
    for(int i = 0; i < heartIcons.Length; i++) {
      heartIcons[i].color = (i < relationship.hearts ? Color.white : new Color(0f, 0f, 0f, 0.5f));
    }

    for(int i=0; i<3; i++) {
      skillIcons[i].gameObject.SetActive(i<relationship.bondSkills.Count);
      if(i < relationship.bondSkills.Count) {
        skillNameTexts[i].text = CardsDatabase.instance.GetCardById(relationship.bondSkills[i]).name;
      } else {
        skillNameTexts[i].text = "---";
      }      
    }
  }

  public void ClickedDateButton() {
    Debug.LogWarning("Clicked date button to "+ coupleCards[0].person.name +" and " + coupleCards[1].person.name);
    GlobalData.instance.observedPeople[0] = coupleCards[0].person;
    GlobalData.instance.observedPeople[1] = coupleCards[1].person;
    GameController.instance.datingPeopleCards[0].Initialize(coupleCards[0].person);
    GameController.instance.datingPeopleCards[1].Initialize(coupleCards[1].person);

    GlobalData.instance.currentDateHearts = relationship.hearts;
    GlobalData.instance.maxDateHearts = relationship.hearts;

    VsnController.instance.StartVSN("date");
  }
}
