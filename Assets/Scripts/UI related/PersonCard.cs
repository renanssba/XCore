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
  public Image equipIcon;
  public TextMeshProUGUI equipmentText;
  public Image addEquipIcon;
  public GameObject shade;

  public GameObject heartsPanel;
  public Image[] heartIcons;

  public bool canEquipItems = true;

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
    if(coupleEntryLayout == PersonCardLayout.couple) {
      bgImage.sprite = ResourcesManager.instance.cardSprites[(person.isMale ? 0 : 1)];
    }
    RectTransform rect = GetComponent<RectTransform>();
    if(heartsPanel != null) {
      if(person.id == 0 || coupleEntryLayout != PersonCardLayout.single) {
        heartsPanel.SetActive(false);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 166f);
      } else {
        heartsPanel.SetActive(true);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 206f);
        for(int i = 0; i < heartIcons.Length; i++) {
          heartIcons[i].color = (i < GlobalData.instance.relationships[person.id - 1].hearts ? Color.white : new Color(0f, 0f, 0f, 0.5f));
        }
      }
    }
    nameText.text = person.name;
    faceImage.sprite = ResourcesManager.instance.GetFaceSprite(person.faceId);


    /// ATTRIBUTES
    for(int i=0; i<3; i++){
      attributeTexts[i].text = person.AttributeValue(i).ToString();
    }
    //Debug.Log("Person: " + person.name);

    /// SKILL
    if(skillText != null) {
      if(person.skillId != -1) {
        skillText.text =  CardsDatabase.instance.GetCardById(person.skillId).name;
      } else {
        skillText.text = "---";
      }
    }
    skillIcon.sprite = CardsDatabase.instance.GetCardById(person.skillId).sprite;

    /// EQUIPMENT
    if(person.id == 0 && coupleEntryLayout == PersonCardLayout.couple) {
      SetEquipableItems(true);
      addEquipIcon.gameObject.SetActive(person.EquipsCount() == 0);
    } else {
      SetEquipableItems(false);
      addEquipIcon.gameObject.SetActive(false);
    }

    if(coupleEntryLayout == PersonCardLayout.single) {
      if(person.equipment != null) {
        equipIcon.sprite = person.equipment.sprite;
        equipIcon.gameObject.SetActive(true);
        equipmentText.text = person.equipment.name;
      } else {
        equipIcon.gameObject.SetActive(false);
        equipmentText.text = "---";
      }
    }
  }

  public void SetEquipableItems(bool canEquipItems) {
    equipIcon.transform.parent.gameObject.SetActive(canEquipItems && 0 <= person.EquipsCount());
  }

  public void ShowShade(bool active) {
    shade.SetActive(active);
  }

  public void ClickPersonSlot(int slotId){
    if(!canEquipItems){
      return;
    }

    if(person.equipment != null){
      person.UnequipItemInSlot(slotId);
      return;
    }else{
      VsnSaveSystem.SetVariable("person_equip_selected", person.id);
      VsnSaveSystem.SetVariable("slot_id", slotId);
      ItemSelectorScreen.instance.OpenEquipSelect();
    }
  }
}
