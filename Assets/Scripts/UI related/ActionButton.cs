using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour {

  public Person person;
  public Skill skill;

  public TextMeshProUGUI nameText;
  public TextMeshProUGUI spCostText;
  public Image iconImage;
  public Image improvementIconImage;
  public GameObject shade;


  public void Initialize(Person p, Skill newSkill) {
    person = p;
    skill = newSkill;
    UpdateUI();
  }

  public void UpdateUI() {
    nameText.text = skill.name;
    if(skill.type == SkillType.attack && skill.id != 9) {
      nameText.text = SpecialCodes.InterpretStrings("\\vsn[" + skill.attribute.ToString() + "_action_name]");
      iconImage.sprite = ResourcesManager.instance.attributeSprites[(int)skill.attribute];
      iconImage.color = ResourcesManager.instance.attributeColor[(int)skill.attribute];
    } else {
      iconImage.sprite = skill.sprite;
      iconImage.color = Color.white;
    }

    if(skill.spCost > 0) {
      spCostText.text = "SP: "+skill.spCost;
      spCostText.gameObject.SetActive(true);
      shade.gameObject.SetActive(!SkillCanBeUsed());
    } else {
      spCostText.gameObject.SetActive(false);
      shade.gameObject.SetActive(false);
    }
  }


  public void ClickedActionButton() {
    if(!SkillCanBeUsed()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");

    BattleController.instance.selectedSkills[currentPlayerTurn] = skill;

    UIController.instance.actionsPanel.HidePanel();
    VsnController.instance.GotCustomInput();
  }

  public bool SkillCanBeUsed() {
    return person.sp >= skill.spCost;
  }
}
