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
  public GameObject lockedIcon;


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
      if(skillId == 3 || skillId == 7) {
        bg.color = new Color(0.77f, 0.19f, 0.19f);
      } else {
        bg.color = Color.white;
      }      
      skillIcon.gameObject.SetActive(true);
      skillIcon.color = Color.white;
      lockedIcon.SetActive(false);
    } else {
      if(IsRequisiteUnlocked()) {
        bg.color = CoupleStatusScreen.instance.skilltreeScreen.unlockableSkillColor;
        skillIcon.gameObject.SetActive(true);
        skillIcon.color = CoupleStatusScreen.instance.skilltreeScreen.unlockableIconColor;
        lockedIcon.SetActive(true);
      } else {
        bg.color = CoupleStatusScreen.instance.skilltreeScreen.lockedPathColor;
        skillIcon.gameObject.SetActive(false);
        lockedIcon.SetActive(false);
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
    SelectSkill(skillId);
  }


  public void SelectSkill(int selectedSkillId) {
    CoupleStatusScreen.instance.skilltreeScreen.selectedSkillId = selectedSkillId;
    Skill skill = BattleController.instance.GetSkillById(relationship.skilltree.skills[selectedSkillId].id);

    if(IsRequisiteUnlocked() || relationship.skilltree.skills[selectedSkillId].isUnlocked) {
      CoupleStatusScreen.instance.skilltreeScreen.SetSkillDescription(skill, relationship.skilltree.skills[selectedSkillId].isUnlocked);
    } else {
      CoupleStatusScreen.instance.skilltreeScreen.SetSkillLocked(Skilltree.skillRequisites[selectedSkillId] != -1);
    }
  }


  public void Clicked() {
    if(IsRequisiteUnlocked() == false) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    if(relationship.skilltree.skills[skillId].isUnlocked == false) {
      CoupleStatusScreen.instance.skilltreeScreen.OpenBuySkillConfirmationScreen(skillId);
    }    
  }
}
