using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkilltreeScreen : MonoBehaviour {

  public Panel screenTransitions;
  public Relationship relationship;

  public Image[] characterImages;

  public SkilltreeIcon[] skilltreeIcons;
  public Image[] skillRequirementPaths;

  public TextMeshProUGUI skillNameText;
  public TextMeshProUGUI skillDescriptionText;

  public TextMeshProUGUI bondPointsText;

  public int selectedSkillId;
  public Panel buySkillConfirmationPanel;
  public TextMeshProUGUI confirmPanelText;

  public Color lockedPathColor;
  public Color unlockableSkillColor;
  public Color unlockableIconColor;
  public Color disadvantageSkillColor;


  public void Initialize() {
    // TODO: fix this
    //relationship = CoupleStatusScreen.instance.statusPerson;
    UpdateUI();
  }

  public void UpdateUI() {
    for(int i=0; i<skilltreeIcons.Length; i++){
      skilltreeIcons[i].Initialize(relationship, i);
    }

    bondPointsText.text = relationship.bondPoints.ToString();

    /// fullbody sprite
    characterImages[0].sprite = ResourcesManager.instance.GetCharacterSprite(relationship.people[0].id, CharacterSpritePart.fullBody);

    /// show requisite paths
    if(relationship.skilltree.skills[8].isUnlocked) {
      skillRequirementPaths[0].color = Color.white;
      skillRequirementPaths[3].color = Color.white;
      skillRequirementPaths[6].color = Color.white;
    } else {
      skillRequirementPaths[0].color = lockedPathColor;
      skillRequirementPaths[3].color = lockedPathColor;
      skillRequirementPaths[6].color = lockedPathColor;
    }

    if(relationship.skilltree.skills[0].isUnlocked) {
      skillRequirementPaths[1].color = Color.white;
      skillRequirementPaths[2].color = Color.white;
    } else {
      skillRequirementPaths[1].color = lockedPathColor;
      skillRequirementPaths[2].color = lockedPathColor;
    }

    if(relationship.skilltree.skills[9].isUnlocked) {
      skillRequirementPaths[4].color = Color.white;
      skillRequirementPaths[5].color = Color.white;
    } else {
      skillRequirementPaths[4].color = lockedPathColor;
      skillRequirementPaths[5].color = lockedPathColor;
    }

    if(relationship.skilltree.skills[4].isUnlocked) {
      skillRequirementPaths[7].color = Color.white;
      skillRequirementPaths[8].color = Color.white;
    } else {
      skillRequirementPaths[7].color = lockedPathColor;
      skillRequirementPaths[8].color = lockedPathColor;
    }
  }


  public void Open() {
    if(BattleController.instance.IsBattleHappening()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    if(VsnSaveSystem.GetBoolVariable("tutorial_skilltree") == false) {
      VsnController.instance.StartVSN("tutorials", new VsnArgument[] { new VsnString("skilltree") });
    }

    VsnAudioManager.instance.PlaySfx("ui_menu_open");
    Initialize();
    screenTransitions.ShowPanel();
    MenuController.instance.BlockTabsNavigation(true);
  }

  public void Close() {
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
    screenTransitions.HidePanel();
    MenuController.instance.BlockTabsNavigation(false);
  }


  public void SetSkillLocked(bool hasSkillRequisite) {
    skillNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("skilltree/locked");
    if(hasSkillRequisite) {
      skillDescriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText("skilltree/unlockRequisiteSkill");
    } else {
      skillDescriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText("skilltree/noRequisiteSkill");
    }
  }

  public void SetSkillDescription(Skill skill, bool isUnlocked) {
    if(skill != null) {
      skillNameText.text = skill.GetPrintableName();
      if(!isUnlocked) {
        skillNameText.text += " " + Lean.Localization.LeanLocalization.GetTranslationText("skilltree/locked");
      }
      skillDescriptionText.text = FullSkilltreeIconDescription(skill);
    } else {
      skillNameText.text = "";
      skillDescriptionText.text = "";
    }
  }


  public string FullSkilltreeIconDescription(Skill skill) {
    string text = "";

    if(skill.type == SkillType.active || skill.type == SkillType.attack) {
      text += Lean.Localization.LeanLocalization.GetTranslationText("skilltree/active") + " ";
    } else {
      text += Lean.Localization.LeanLocalization.GetTranslationText("skilltree/passive") + " ";
    }

    //switch(relationship.skilltree.skills[selectedSkillId].affectsPerson) {
    //  case SkillAffectsCharacter.boy:
    //    text += Lean.Localization.LeanLocalization.GetTranslationText("skilltree/affects") + " " + relationship.GetBoy().GetName() + "\n";
    //    break;
    //  case SkillAffectsCharacter.girl:
    //    text += Lean.Localization.LeanLocalization.GetTranslationText("skilltree/affects") + " " + relationship.GetGirl().GetName() + "\n";
    //    break;
    //  case SkillAffectsCharacter.couple:
    //    text += Lean.Localization.LeanLocalization.GetTranslationText("skilltree/affects_couple") + "\n";
    //    break;
    //}
    text = text + skill.GetPrintableDescription();

    return text;
  }

  public void OpenBuySkillConfirmationScreen(int id) {
    if(relationship.bondPoints <= 0) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    selectedSkillId = id;
    VsnSaveSystem.SetVariable("selected_skill", BattleController.instance.GetSkillById(relationship.skilltree.skills[selectedSkillId].id).GetPrintableName());
    confirmPanelText.text = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("skilltree/confirmMessage"));
    buySkillConfirmationPanel.ShowPanel();
  }


  public void HealCharactersSP() {
    if(GlobalData.instance.GetCurrentRelationship() == null) {
      return;
    }
    relationship.people[0].HealSp(100);
    relationship.people[1].HealSp(100);
  }

  public void ConfirmSkillBuy() {
    relationship.bondPoints--;
    Skill unlockedSkill = BattleController.instance.GetSkillById(relationship.skilltree.skills[selectedSkillId].id);
    relationship.skilltree.skills[selectedSkillId].isUnlocked = true;

    HealCharactersSP();

    UpdateUI();
    StatusScreen.instance.UpdateUI();
    UIController.instance.UpdateUI();
    //UIController.instance.girlInteractionScreen.relationshipCard.Initialize(GlobalData.instance.GetCurrentRelationship());
    //UIController.instance.girlInteractionScreen.relationshipCard.UpdateUI();

    SfxManager.StaticPlayBigConfirmSfx();
    buySkillConfirmationPanel.HidePanel();
  }

  public void CancelSkillBuy() {
    SfxManager.StaticPlayCancelSfx();
    buySkillConfirmationPanel.HidePanel();
  }
}
