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
  public TextMeshProUGUI skillText;
  public Image equipIcon;
  public TextMeshProUGUI equipmentText;
  public Image addEquipIcon;

  public GameObject heartsPanel;
  public Image[] heartIcons;

  public bool canEquipItems = true;

  public bool coupleEntryLayout = false;


  public void Initialize(Person p){
    //Debug.Log("initializing " + person.name);
    person = p;
    UpdateUI();
  }

  public void UpdateUI() {
    //Debug.Log("updating "+person.name);

    if(person == null) {
      return;
    }


    /// BG AND FACE / NAME
    if(coupleEntryLayout == false) {
      bgImage.sprite = ResourcesManager.instance.cardSprites[(person.isMale ? 0 : 1)];
    }
    RectTransform rect = GetComponent<RectTransform>();
    if(heartsPanel != null) {
      if(person.id == 0 || coupleEntryLayout) {
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
    for (int i=0; i<3; i++){
      attributeTexts[i].text = person.AttributeValue(i).ToString();
    }
    Debug.Log("Person: " + person.name);

    /// SKILL
    if(skillText != null) {
      if(person.skill != Skill.Nenhum) {
        skillText.text = person.skill.ToString();
      } else {
        skillText.text = "---";
      }
    }        

    /// EQUIPMENT
    if(person.id == 0 && coupleEntryLayout) {
      SetEquipableItems(true);
      addEquipIcon.gameObject.SetActive(person.EquipsCount() == 0);
    } else {
      SetEquipableItems(false);
      addEquipIcon.gameObject.SetActive(false);
    }

    if(coupleEntryLayout == false) {
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
      ItemSelectorScreen.instance.OpenInput();
    }
  }
}
