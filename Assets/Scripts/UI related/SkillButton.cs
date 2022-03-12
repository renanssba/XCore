using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour {

  [Header("- References -")]
  public Pilot pilot;
  public Skill skill;

  [Header("- Visuals -")]
  public Image bg;
  public Image skillIcon;
  public TextMeshProUGUI skillName;
  public PilotDetailsCard pilotCard;


  public void Initialize(PilotDetailsCard argPersonCard, Skill argskill) {
    pilotCard = argPersonCard;
    pilot = ((Pilot)pilotCard.character);
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
