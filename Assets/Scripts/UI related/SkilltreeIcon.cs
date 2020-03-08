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
    if(relationship.unlockedSkill[skillId]) {
      bg.color = Color.white;
      skillIcon.sprite = BattleController.instance.GetSkillById(relationship.skillIds[skillId]).sprite;
      skillIcon.gameObject.SetActive(true);
    } else {
      if(IsRequisiteUnlocked()) {
        bg.color = SkilltreeScreen.instance.unlockableSkillColor;
        skillIcon.gameObject.SetActive(false);
      } else {
        bg.color = SkilltreeScreen.instance.lockedPathColor;
        skillIcon.gameObject.SetActive(false);
      }
    }
  }

  public bool IsRequisiteUnlocked() {
    int requisite = Relationship.skillRequisites[skillId];
    if(skillId == 11) {
      return false;
    }
    return (requisite == -1 || relationship.unlockedSkill[requisite]);
  }

  public void Selected() {
    SkilltreeScreen.instance.SelectSkill(skillId);
  }

  public void Clicked() {
    if(IsRequisiteUnlocked() == false) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    if(relationship.unlockedSkill[skillId] == false) {
      SkilltreeScreen.instance.OpenBuySkillConfirmationScreen(skillId);
    }    
  }
}
