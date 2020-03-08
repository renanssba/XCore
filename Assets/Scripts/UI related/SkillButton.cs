using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour {

  public Image skillIcon;
  public TextMeshProUGUI skillName;
  public Person person;
  public Skill skill;
  public Image bg;


  public void Initialize(Person argperson, Skill argskill) {
    person = argperson;
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
}
