using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour {

  public Image skillIcon;
  public TextMeshProUGUI skillName;
  public PersonCard personCard;
  public Person person;
  public Skill skill;
  public Image bg;


  public void Initialize(PersonCard argPersonCard, Skill argskill) {
    personCard = argPersonCard;
    person = personCard.person;
    skill = argskill;
    UpdateUI();
  }

  public void UpdateUI() {
    if(skill != null) {
      bg.SetAlpha(1f);
      skillName.text = skill.GetPrintableName();
      skillIcon.sprite = skill.sprite;
      skillIcon.gameObject.SetActive(true);
    } else {
      bg.SetAlpha(0.4f);
      skillName.text = "";
      skillIcon.gameObject.SetActive(false);
    }
  }

  public void SelectedButton() {
    Utils.SelectUiElement(gameObject);
    personCard.UpdateUI();
  }

  public void UnselectButton() {
    if(EventSystem.current.currentSelectedGameObject == gameObject) {
      Utils.SelectUiElement(null);
      personCard.UpdateUI();
    }
  }
}
