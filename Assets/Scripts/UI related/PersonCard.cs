using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

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

  public ScreenTransitions skillDescriptionPanel;
  public TextMeshProUGUI skillNameText;
  public TextMeshProUGUI skillDescriptionText;
  public SkillButton[] skillButtons;

  public PersonCardLayout coupleEntryLayout = PersonCardLayout.statusScreen;


  public void Initialize(Person p){
    person = p;
    UpdateUI();
  }

  public void UpdateUI() {
    if(person == null || gameObject.activeSelf == false) {
      return;
    }


    /// BG AND FACE / NAME
    bgImage.sprite = ResourcesManager.instance.cardSprites[(person.isMale ? 0 : 1)];
    nameText.text = person.name;


    /// STATUS CONDITIONS
    UpdateStatusConditions();


    if(coupleEntryLayout == PersonCardLayout.dateUI) {
      faceImage.sprite = ResourcesManager.instance.GetFaceSprite(person.faceId);
      spText.text = "<sprite=\"hpANDsp\" index=1 tint>SP: " + person.sp + "<size=16>/" + person.GetMaxSp(GlobalData.instance.GetCurrentRelationship().id) + "</size>";
      return;
    }


    int relationshipId = CoupleStatusScreen.instance.relationship.id;

    bodyImages[0].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.body);
    bodyImages[1].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.school);


    /// ATTRIBUTES
    string attrString = "";
    for(int i=0; i<4; i++){
      attributeNamesTexts[i].gameObject.SetActive(false);
      attributeNamesTexts[i].gameObject.SetActive(true);
      string colorTag = ColorTag(i);
      if(colorTag == null) {
        attrString += person.AttributeValue(i).ToString() + "\n";
      }else {
        attrString += colorTag+person.AttributeValue(i).ToString() + "</color>\n";
      }
      
    }
    attrString += person.sp+"/"+person.GetMaxSp(relationshipId);
    attributeValuesText.text = attrString;


    /// SKILLS
    Skill[] allSkills = person.GetAllCharacterSpecificSkills(relationshipId);
    for(int i=0; i<skillButtons.Length; i++) {
      if(i < allSkills.Length) {
        skillButtons[i].Initialize(this, allSkills[i]);
      } else {
        skillButtons[i].Initialize(this, null);
      }      
    }

    /// SKILL DESCRIPTION
    GameObject selected = EventSystem.current.currentSelectedGameObject;
    if(selected != null && selected.GetComponent<SkillButton>() != null) {
      SetSkillDescription(selected.GetComponent<SkillButton>().skill);
    } else {
      SetSkillDescription(null);
    }
  }

  public string ColorTag(int attributeId) {
    int currentValue = person.AttributeValue(attributeId);
    int baseValue = person.attributes[attributeId];

    if(currentValue > baseValue) {
      return "<color=#00D900>";
    }
    if(currentValue < baseValue) {
      return "<color=red>";
    }
    return null;
  }

  public void SetSkillDescription(Skill skill) {
    if(skill != null) {
      skillNameText.text = skill.GetPrintableName();
      skillDescriptionText.text = StatusScreenSkillDescription(skill);
      skillDescriptionPanel.ShowPanel();
    } else {
      skillNameText.text = "";
      skillDescriptionText.text = "";
      skillDescriptionPanel.HidePanel();
    }
  }

  public string StatusScreenSkillDescription(Skill skill) {
    string text = "";

    if(skill.type == SkillType.active || skill.type == SkillType.attack) {
      text += "[Ativa] ";
    } else {
      text += "[Passiva] ";
    }
    text = text + skill.GetPrintableDescription();

    return text;
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
