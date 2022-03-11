﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour {

  public Image skillIcon;
  public TextMeshProUGUI skillName;
  public PilotCard pilotCard;
  public Pilot person;
  public Skill skill;
  public Image bg;


  public void Initialize(PilotCard argPersonCard, Skill argskill) {
    pilotCard = argPersonCard;
    person = pilotCard.person;
    skill = argskill;
    UpdateUI();
  }

  public void UpdateUI() {
    if(skill != null) {
      if(skill.HasTag("disadvantage")) {
        bg.color = StatusScreen.instance.skilltreeScreen.disadvantageSkillColor;
      } else if(skill.type == SkillType.active || skill.type == SkillType.attack) {
        bg.color = StatusScreen.instance.activeSkillButtonColor;
      } else {
        bg.color = StatusScreen.instance.passiveSkillButtonColor;
      }
      skillName.text = skill.GetPrintableName();
      skillIcon.sprite = skill.sprite;
      skillIcon.gameObject.SetActive(true);
    } else {
      bg.color = StatusScreen.instance.lockedSkillButtonColor;
      skillName.text = "";
      skillIcon.gameObject.SetActive(false);
    }
  }

  public void SelectedButton() {
    Utils.SelectUiElement(gameObject);
    pilotCard.UpdateUI();
  }

  public void UnselectButton() {
    if(EventSystem.current.currentSelectedGameObject == gameObject) {
      Utils.SelectUiElement(null);
      pilotCard.UpdateUI();
    }
  }
}
