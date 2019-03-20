﻿using System.Collections;
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
  public Image[] equipIcons;

  public bool canEquipItems = true;


  public void Initialize(Person p){
    person = p;
    UpdateUI();
  }

  public void UpdateUI() {
    nameText.text = person.name;
    for(int i=0; i<3; i++){
      attributeTexts[i].text = person.AttributeValue(i).ToString();
      attributeTexts[i].alpha = 0.4f;
      attributeTexts[i].transform.parent.GetComponent<Image>().DOFade(0.5f, 0f);
      if(person.equips[i] != null){
        equipIcons[i].sprite = person.equips[i].sprite;
        equipIcons[i].gameObject.SetActive(true);
      } else{
        equipIcons[i].gameObject.SetActive(false);
      }
    }
    attributeTexts[(int)person.personality].alpha = 1f;
    attributeTexts[(int)person.personality].transform.parent.GetComponent<Image>().DOFade(1f, 0f);

    faceImage.sprite = ResourcesManager.instance.faceSprites[person.faceId];
    bgImage.sprite = ResourcesManager.instance.cardSprites[(person.isMale?0:1)];
    traitTexts[0].text = person.personality.ToString();
  }

  public void SetEquipableItems(bool value) {
    //canEquipItems = value;
    foreach(Image img in equipIcons){
      img.transform.parent.GetComponent<Button>().interactable = value;
    }
    UpdateUI();
  }

  public void ClickPersonSlot(int slotId){
    if(!canEquipItems){
      return;
    }

    if(person.equips[slotId] != null){
      person.UnequipItemInSlot(slotId);
      return;
    }else{
      VsnSaveSystem.SetVariable("person_equip_selected", person.isMale ? 1 : 0);
      VsnSaveSystem.SetVariable("slot_id", slotId);
      ItemSelectorScreen.instance.OpenInput();
    }    
  }
}
