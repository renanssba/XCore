using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum PersonCardLayout {
  single,
  couple,
  date
}


public class PersonCard : MonoBehaviour {

  public Person person = null;
  public Image bgImage;
  public TextMeshProUGUI nameText;
  public Image faceImage;
  public TextMeshProUGUI[] attributeTexts;
  public Image skillIcon;
  public TextMeshProUGUI skillText;
  public GameObject shade;

  public TextMeshProUGUI spText;

  public GameObject heartsPanel;
  public Image[] heartIcons;

  public PersonCardLayout coupleEntryLayout = PersonCardLayout.single;


  public void Initialize(Person p){
    person = p;
    UpdateUI();
  }

  public void UpdateUI() {
    //Debug.Log("updating "+person.name);

    if(person == null || gameObject.activeSelf == false) {
      return;
    }

    /// BG AND FACE / NAME
    bgImage.sprite = ResourcesManager.instance.cardSprites[(person.isMale ? 0 : 1)];
    nameText.text = person.name;
    faceImage.sprite = ResourcesManager.instance.GetFaceSprite(person.faceId);

    if(coupleEntryLayout == PersonCardLayout.date) {
      spText.text = "SP: "+person.sp + "<size=16>/" + person.maxSp+ "</size>";
      return;
    }

    RectTransform rect = GetComponent<RectTransform>();
    if(heartsPanel != null) {
      if(person.id == 0 || coupleEntryLayout != PersonCardLayout.single) {
        heartsPanel.SetActive(false);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 144f);
      } else {
        heartsPanel.SetActive(true);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 184f);
        for(int i = 0; i < heartIcons.Length; i++) {
          heartIcons[i].color = (i < GlobalData.instance.relationships[person.id - 1].hearts ? Color.white : new Color(0f, 0f, 0f, 0.5f));
        }
      }
    }


    /// ATTRIBUTES
    for(int i=0; i<4; i++){
      attributeTexts[i].text = person.AttributeValue(i).ToString();
    }
    //Debug.Log("Person: " + person.name);

    /// SKILL
    //if(skillText != null) {
    //  if(person.skillIds != -1) {
    //    skillText.text =  CardsDatabase.instance.GetCardById(person.skillIds).name;
    //  } else {
    //    skillText.text = "---";
    //  }
    //}
    //skillIcon.sprite = CardsDatabase.instance.GetCardById(person.skillIds).sprite;
  }

  public void ShowShade(bool active) {
    shade.SetActive(active);
  }
}
