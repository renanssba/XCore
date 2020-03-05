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
    Color c = bg.color;

    if(skill != null) {
      c.a = 1f;
      bg.color = c;
      skillName.text = skill.GetPrintableName();
      skillIcon.sprite = skill.sprite;
      skillIcon.gameObject.SetActive(true);
    } else {
      c.a = 0.4f;
      bg.color = c;
      skillName.text = "";
      skillIcon.gameObject.SetActive(false);
    }
  }
}
