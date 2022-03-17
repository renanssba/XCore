using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum PilotCardLayout {
  statusScreen,
  dateUI
}


public class PilotDetailsCard : BattlerInfoPanel {
  [Header("- Visuals -")]
  public Image bodyImage;

  [Header("- Attributes -")]
  public TextMeshProUGUI[] attributeNamesTexts;
  public TextMeshProUGUI attributeValuesText;

  [Header("- Skills Listed -")]
  public SkillButton[] skillButtons;

  [Header("- Skill Description Panel -")]
  public Panel skillDescriptionPanel;
  public TextMeshProUGUI skillNameText;
  public TextMeshProUGUI skillDescriptionText;

  public PilotCardLayout coupleEntryLayout = PilotCardLayout.statusScreen;


  public override void SetSelectedUnit(Battler myBattler){
    character = myBattler;
    UpdateUI();
  }

  public void UpdateUI() {
    if(character == null || gameObject.activeSelf == false) {
      return;
    }

    /// Update Basic Info, like HP, SP and 
    UpdateBattlerUI();


    int relationshipId = StatusScreen.instance.pilot.id;
    bodyImage.sprite = ResourcesManager.instance.GetCharacterSprite(character.id, CharacterSpritePart.fullBody);


    /// ATTRIBUTES
    string attrString = "";
    for(int i=0; i<4; i++){
      attributeNamesTexts[i].gameObject.SetActive(false);
      attributeNamesTexts[i].gameObject.SetActive(true);
      string colorTag = ColorTag(i);
      if(colorTag == null) {
        attrString += character.AttributeValue(i+1).ToString() + "\n";
      } else {
        attrString += colorTag + character.AttributeValue(i+1).ToString() + "</color>\n";
      }
      
    }
    attrString += ((Pilot)character).sp + " /" + ((Pilot)character).GetMaxSp();
    attributeValuesText.text = attrString;


    /// SKILLS
    Skill[] allSkills = ((Pilot)character).GetAllCharacterSpecificSkills();
    Array.Sort(allSkills, (a, b) => a.type.CompareTo(b.type));

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
    int currentValue = character.AttributeValue(attributeId);
    int baseValue = character.attributes[attributeId];

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
      text += Lean.Localization.LeanLocalization.GetTranslationText("skilltree/active") + " ";
    } else {
      text += Lean.Localization.LeanLocalization.GetTranslationText("skilltree/passive") + " ";
    }
    text = text + skill.GetPrintableDescription();

    return text;
  }
}
