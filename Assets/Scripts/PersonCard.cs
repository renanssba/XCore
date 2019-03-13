using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PersonCard : MonoBehaviour {

  public Person person;
  public Image bgImage;
  public TextMeshProUGUI nameText;
  public Image faceImage;
  public TextMeshProUGUI[] attributeTexts;
  public TextMeshProUGUI[] traitTexts;


  public void Initialize(Person p){
    person = p;
    UpdateUI();
  }

  public void UpdateUI() {
    nameText.text = person.name;
    for(int i=0; i<3; i++){
      attributeTexts[i].text = person.attributes[i].ToString();
      attributeTexts[i].alpha = 0.5f;
      attributeTexts[i].transform.parent.GetComponent<Image>().DOFade(0.5f, 0f);
    }
    attributeTexts[(int)person.personality].alpha = 1f;
    attributeTexts[(int)person.personality].transform.parent.GetComponent<Image>().DOFade(1f, 0f);

    faceImage.sprite = ResourcesManager.instance.faceSprites[person.faceId];
    bgImage.sprite = ResourcesManager.instance.cardSprites[(person.isMale?0:1)];
    traitTexts[0].text = person.personality.ToString();
  }
}
