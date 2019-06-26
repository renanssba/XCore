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
  public Image[] favoriteAttributeIcons;
  public Image[] equipIcons;
  public Image[] addEquipIcons;

  public Button observationButton;

  public bool canEquipItems = true;

  public bool coupleEntryLayout = false;


  public void Initialize(Person p){
    //Debug.Log("initializing " + person.name);
    person = p;
    UpdateUI();
  }

  public void UpdateUI() {
    //Debug.Log("updating "+person.name);

    string state = VsnSaveSystem.GetStringVariable("people_ui_state");
    if(coupleEntryLayout == false) {
      switch(state) {
        case "choose_observation_target":
          SetEquipableItems(true);
          SetObservableButton(true);
          break;
        default:
          SetEquipableItems(false);
          SetObservableButton(false);
          break;
      }
    }
    

    nameText.text = person.name;
    for (int i=0; i<3; i++){
      attributeTexts[i].text = person.AttributeValue(i).ToString();
      favoriteAttributeIcons[i].gameObject.SetActive(false);
      //attributeTexts[i].alpha = 0.4f;
      //attributeTexts[i].transform.parent.GetComponent<Image>().DOFade(0.5f, 0f);

      if(coupleEntryLayout == false) {
        if(person.equips[i] != null) {
          equipIcons[i].sprite = person.equips[i].sprite;
          equipIcons[i].gameObject.SetActive(true);
        } else {
          equipIcons[i].gameObject.SetActive(false);
        }
        addEquipIcons[i].gameObject.SetActive(person.EquipsCount() == i && state == "choose_observation_target");
      }
    }
    //attributeTexts[(int)person.personality].alpha = 1f;
    //attributeTexts[(int)person.personality].transform.parent.GetComponent<Image>().DOFade(1f, 0f);
    favoriteAttributeIcons[(int)person.personality].gameObject.SetActive(true);

    faceImage.sprite = ResourcesManager.instance.GetFaceSprite(person.faceId);
    if(coupleEntryLayout == false) {
      bgImage.sprite = ResourcesManager.instance.cardSprites[(person.isMale ? 0 : 1)];
    }
    //traitTexts[0].text = person.PersonalityString();
  }

  public void SetEquipableItems(bool canEquipItems) {
    for(int i=0; i<equipIcons.Length; i++){
      equipIcons[i].transform.parent.GetComponent<Button>().interactable = canEquipItems && i<=person.EquipsCount();
    }
    //UpdateUI();
  }

  public void SetObservableButton(bool value){
    observationButton.gameObject.SetActive(value);
  }

  public void ClickPersonSlot(int slotId){
    if(!canEquipItems){
      return;
    }

    if(person.equips[slotId] != null){
      person.UnequipItemInSlot(slotId);
      return;
    }else{
      VsnSaveSystem.SetVariable("person_equip_selected", person.id);
      VsnSaveSystem.SetVariable("slot_id", slotId);
      ItemSelectorScreen.instance.OpenInput();
    }
  }

  public void ClickObserveButton(){
    //Debug.LogWarning("Observe button clicked");
    GlobalData.instance.observedPeople[0] = person;
    GameController.instance.SetupObservationSegmentTiles();
    VsnSaveSystem.SetVariable("observation_energy", 5);
    VsnController.instance.StartVSN("observation_intro");

    SfxManager.StaticPlayConfirmSfx();
  }
}
