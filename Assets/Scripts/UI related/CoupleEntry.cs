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
    foreach(PersonCard p in coupleCards) {
      p.UpdateUI();
    }
    // heart icons
    for(int i = 0; i < heartIcons.Length; i++) {
      heartIcons[i].color = (i < relationship.hearts ? Color.white : new Color(0f, 0f, 0f, 0.5f));
    }

    for(int i=0; i<3; i++) {
      skillIcons[i].gameObject.SetActive(i<relationship.bondSkills.Count);
      if(i < relationship.bondSkills.Count) {
        skillNameTexts[i].text = relationship.bondSkills[i].ToString();
      } else {
        skillNameTexts[i].text = "---";
      }      
    }
  }

  public void ClickedDateButton() {
    Debug.LogWarning("Clicked date button to "+ coupleCards[0].person.name +" and " + coupleCards[1].person.name);
    GlobalData.instance.observedPeople[0] = coupleCards[0].person;
    GlobalData.instance.observedPeople[1] = coupleCards[1].person;
    //TheaterController.instance.mainActor.GetComponent<Actor3D>().SetGraphics(GlobalData.instance.observedPeople[0]);
    //TheaterController.instance.supportActor.GetComponent<Actor3D>().SetGraphics(GlobalData.instance.observedPeople[1]);
    VsnController.instance.StartVSN("date");
  }
}
