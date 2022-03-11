using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoupleEntry : MonoBehaviour {

  public Relationship relationship;
  public PilotCard[] coupleCards;
  public Image[] heartIcons;
  public Image[] skillIcons;
  public TextMeshProUGUI[] skillNameTexts;

  public void Initialize(Relationship rel) {
    relationship = rel;
    coupleCards[0].Initialize(rel.people[0]);
    coupleCards[1].Initialize(rel.people[1]);
  }

  public void OnEnable() {
    UpdateUI();
  }


  public void UpdateUI() {
    foreach(PilotCard pcard in coupleCards) {
      pcard.UpdateUI();
    }
    // heart icons
    for(int i = 0; i < heartIcons.Length; i++) {
      heartIcons[i].color = (i < relationship.level ? Color.white : new Color(0f, 0f, 0f, 0.5f));
    }

    //for(int i=0; i<3; i++) {
    //  skillIcons[i].gameObject.SetActive(i<relationship.skillIds.Count);
    //  if(i < relationship.skillIds.Count) {
    //    // print bond skill name
    //    //skillNameTexts[i].text = "skill name";
    //  } else {
    //    skillNameTexts[i].text = "---";
    //  }
    //}
  }
}
