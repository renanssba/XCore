using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkilltreeIcon : MonoBehaviour {
  public Relationship relationship;
  public int skillId;

  public Image bg;
  public Image skillIcon;


  public void Initialize(Relationship argrelationship, int id) {
    relationship = argrelationship;
    skillId = id;
    UpdateUI();
  }

  public void UpdateUI() {
    Skill skill = BattleController.instance.GetSkillById(relationship.skilltree.skills[skillId].id);
    Debug.Log("Updating skill: " + skill.name);

    skillIcon.sprite = skill.sprite;
    if(relationship.skilltree.skills[skillId].isUnlocked) {
      bg.color = Color.white;
      skillIcon.gameObject.SetActive(true);
      skillIcon.color = Color.white;
    } else {
      if(IsRequisiteUnlocked()) {
        bg.color = SkilltreeScreen.instance.unlockableSkillColor;
        skillIcon.gameObject.SetActive(true);
        skillIcon.color = SkilltreeScreen.instance.unlockableIconColor;
      } else {
        bg.color = SkilltreeScreen.instance.lockedPathColor;
        skillIcon.gameObject.SetActive(false);
      }
    }
  }

  public bool IsRequisiteUnlocked() {
    int requisite = Skilltree.skillRequisites[skillId];
    if(skillId == 12) {
      return false;
    }
    return (requisite == -1 || relationship.skilltree.skills[requisite].isUnlocked);
  }

  public void Selected() {
    SkilltreeScreen.instance.SelectSkill(skillId);
  }

  public void Clicked() {
    if(IsRequisiteUnlocked() == false) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    if(relationship.skilltree.skills[skillId].isUnlocked == false) {
      SkilltreeScreen.instance.OpenBuySkillConfirmationScreen(skillId);
    }    
  }
}
