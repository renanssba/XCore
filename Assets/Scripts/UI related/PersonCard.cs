using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum PersonCardLayout {
  statusScreen,
  dateUI
}


public class PersonCard : MonoBehaviour {

  public Person person = null;
  public Image bgImage;
  public TextMeshProUGUI nameText;
  public Image faceImage;
  public Image[] bodyImages;
  public TextMeshProUGUI[] attributeNamesTexts;
  public TextMeshProUGUI attributeValuesText;
  public GameObject shade;

  public TextMeshProUGUI spText;

  public Transform statusConditionsContent;

  public SkillButton[] skillButtons;

  public PersonCardLayout coupleEntryLayout = PersonCardLayout.statusScreen;


  public void Initialize(Person p){
    person = p;
    UpdateUI();
  }

  public void UpdateUI() {
    //Debug.Log("updating "+person.name);
    int relationShipId = CoupleStatusScreen.instance.relationship.id;

    if(person == null || gameObject.activeSelf == false) {
      return;
    }

    /// BG AND FACE / NAME
    bgImage.sprite = ResourcesManager.instance.cardSprites[(person.isMale ? 0 : 1)];
    nameText.text = person.name;

    if(coupleEntryLayout == PersonCardLayout.dateUI) {
      faceImage.sprite = ResourcesManager.instance.GetFaceSprite(person.faceId);
      spText.text = "<sprite=\"Attributes\" index=3 tint>SP: " + person.sp + "<size=16>/" + person.maxSp+ "</size>";

      return;
    }

    bodyImages[0].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.body);
    bodyImages[1].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.school);

    /// STATUS CONDITIONS
    UpdateStatusConditions();

    /// ATTRIBUTES
    string attrString = "";
    for(int i=0; i<4; i++){
      attributeNamesTexts[i].gameObject.SetActive(false);
      attributeNamesTexts[i].gameObject.SetActive(true);
      attrString += person.AttributeValue(i).ToString() + "\n";
    }
    attrString += person.sp+"/"+person.maxSp;
    attributeValuesText.text = attrString;


    /// SKILLS
    for(int i=0; i<skillButtons.Length; i++) {
      if(i < person.GetAllSkills(relationShipId).Length) {
        skillButtons[i].Initialize(person, person.GetAllSkills(relationShipId)[i]);
      } else {
        skillButtons[i].Initialize(person, null);
      }      
    }
  }

  public void UpdateStatusConditions() {
    ClearStatusConditionIcons();

    for(int i = 0; i < person.statusConditions.Count; i++) {
      GameObject newObj = Instantiate(UIController.instance.statusConditionIconPrefab, statusConditionsContent);
      newObj.GetComponent<StatusConditionIcon>().Initialize(person.statusConditions[i]);
    }
  }

  public void ClearStatusConditionIcons() {
    int childCount = statusConditionsContent.transform.childCount;

    for(int i = 0; i < childCount; i++) {
      Destroy(statusConditionsContent.transform.GetChild(i).gameObject);
    }
  }

  public void ShowShade(bool active) {
    shade.SetActive(active);
  }

  public void ClickDateUiPersonCard() {
    VsnAudioManager.instance.PlaySfx("ui_menu_open");
    UIController.instance.coupleStatusScreen.Initialize(GlobalData.instance.GetCurrentRelationship());
    UIController.instance.coupleStatusScreen.panel.ShowPanel();
  }
}
