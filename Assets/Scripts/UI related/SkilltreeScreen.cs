using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkilltreeScreen : MonoBehaviour {

  public static SkilltreeScreen instance;

  public ScreenTransitions screenTransitions;
  public Relationship relationship;

  public Image[] characterImages;

  public SkilltreeIcon[] skilltreeIcons;
  public Image[] skillRequirementPaths;

  public TextMeshProUGUI skillNameText;
  public TextMeshProUGUI skillDescriptionText;

  public TextMeshProUGUI bondPointsText;

  public int selectedSkillId;
  public ScreenTransitions buySkillConfirmationPanel;
  public TextMeshProUGUI confirmPanelText;

  public Color lockedPathColor;
  public Color unlockableSkillColor;
  public Color unlockableIconColor;


  public void Awake() {
    instance = this;
  }

  public void Initialize() {
    instance = this;
    lockedPathColor = new Color(0.92f, 0.5f, 0.52f);
    unlockableSkillColor = new Color(0.6f, 0.42f, 0.42f);
    unlockableIconColor = new Color(0f, 0f, 0f, 0.3f);
    relationship = CoupleStatusScreen.instance.relationship;
    UpdateUI();
  }

  public void UpdateUI() {
    for(int i=0; i<skilltreeIcons.Length; i++){
      skilltreeIcons[i].Initialize(relationship, i);
    }

    bondPointsText.text = relationship.bondPoints.ToString();

    /// character sprites
    characterImages[0].sprite = ResourcesManager.instance.GetCharacterSprite(relationship.GetBoy().id, CharacterSpritePart.body);
    characterImages[1].sprite = ResourcesManager.instance.GetCharacterSprite(relationship.GetBoy().id, CharacterSpritePart.school);
    characterImages[2].sprite = ResourcesManager.instance.GetCharacterSprite(relationship.GetGirl().id, CharacterSpritePart.body);
    characterImages[3].sprite = ResourcesManager.instance.GetCharacterSprite(relationship.GetGirl().id, CharacterSpritePart.school);


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

    VsnAudioManager.instance.PlaySfx("ui_menu_open");
    Initialize();
    screenTransitions.ShowPanel();
  }

  public void Close() {
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
    screenTransitions.HidePanel();
  }


  public void SelectSkill(int id) {
    selectedSkillId = id;
    Skill skill = BattleController.instance.GetSkillById(relationship.skilltree.skills[selectedSkillId].id);
    SetSkillDescription(skill);
  }

  public void SetSkillDescription(Skill skill) {
    if(skill != null) {
      skillNameText.text = skill.GetPrintableName();
      skillDescriptionText.text = FullSkilltreeIconDescription(skill);
    } else {
      skillNameText.text = "";
      skillDescriptionText.text = "";
    }
  }

  public string FullSkilltreeIconDescription(Skill skill) {
    string text = "";

    if(skill.type == SkillType.active || skill.type == SkillType.attack) {
      text += "[Ativa] ";
    } else {
      text += "[Passiva] ";
    }
    if(selectedSkillId < 4) {
      text += "para "+relationship.GetBoy().name + "\n";
    } else if(selectedSkillId < 8) {
      text += "para " + relationship.GetGirl().name + "\n";
    } else {
      text += "para o casal\n";
    }
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
    confirmPanelText.text = "Você gostaria de comprar a habilidade <color=yellow>"+
      BattleController.instance.GetSkillById(relationship.skilltree.skills[selectedSkillId].id).GetPrintableName() + "</color>?";
    buySkillConfirmationPanel.ShowPanel();
  }


  public void HealCharactersSP() {
    relationship.GetBoy().HealSp(100);
    relationship.GetGirl().HealSp(100);
  }

  public void ConfirmSkillBuy() {
    relationship.bondPoints--;
    Skill unlockedSkill = BattleController.instance.GetSkillById(relationship.skilltree.skills[selectedSkillId].id);
    relationship.skilltree.skills[selectedSkillId].isUnlocked = true;

    HealCharactersSP();

    UpdateUI();
    CoupleStatusScreen.instance.UpdateUI();

    SfxManager.StaticPlayBigConfirmSfx();
    buySkillConfirmationPanel.HidePanel();
  }

  public void CancelSkillBuy() {
    SfxManager.StaticPlayCancelSfx();
    buySkillConfirmationPanel.HidePanel();
  }
}
