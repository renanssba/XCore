using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkilltreeScreen : MonoBehaviour {

  public static SkilltreeScreen instance;

  public ScreenTransitions screenTransitions;
  public Relationship relationship;
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


  public void Awake() {
    instance = this;
  }

  public void Initialize() {
    instance = this;
    lockedPathColor = new Color(0.92f, 0.5f, 0.52f);
    unlockableSkillColor = new Color(0.6f, 0.42f, 0.42f);
    relationship = CoupleStatusScreen.instance.relationship;
    UpdateUI();
  }

  public void UpdateUI() {
    for(int i=0; i<skilltreeIcons.Length; i++){
      skilltreeIcons[i].Initialize(relationship, i);
    }

    bondPointsText.text = relationship.bondPoints.ToString();

    /// show requisite paths
    if(relationship.unlockedSkill[0]) {
      skillRequirementPaths[0].color = Color.white;
      skillRequirementPaths[1].color = Color.white;
      skillRequirementPaths[2].color = Color.white;
    } else {
      skillRequirementPaths[0].color = lockedPathColor;
      skillRequirementPaths[1].color = lockedPathColor;
      skillRequirementPaths[2].color = lockedPathColor;
    }

    if(relationship.unlockedSkill[1]) {
      skillRequirementPaths[3].color = Color.white;
      skillRequirementPaths[4].color = Color.white;
    } else {
      skillRequirementPaths[3].color = lockedPathColor;
      skillRequirementPaths[4].color = lockedPathColor;
    }

    if(relationship.unlockedSkill[2]) {
      skillRequirementPaths[5].color = Color.white;
      skillRequirementPaths[6].color = Color.white;
    } else {
      skillRequirementPaths[5].color = lockedPathColor;
      skillRequirementPaths[6].color = lockedPathColor;
    }

    if(relationship.unlockedSkill[3]) {
      skillRequirementPaths[7].color = Color.white;
      skillRequirementPaths[8].color = Color.white;
    } else {
      skillRequirementPaths[7].color = lockedPathColor;
      skillRequirementPaths[8].color = lockedPathColor;
    }
  }


  public void Open() {
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
    Skill skill = BattleController.instance.GetSkillById(relationship.skillIds[selectedSkillId]);
    skillNameText.text = skill.GetPrintableName();
    skillDescriptionText.text = skill.GetPrintableDescription();
  }

  public void OpenBuySkillConfirmationScreen(int id) {
    if(relationship.bondPoints <= 0) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    SfxManager.StaticPlayConfirmSfx();
    selectedSkillId = id;
    confirmPanelText.text = "Você gostaria de comprar a habilidade <color=yellow>"+
      BattleController.instance.GetSkillById(relationship.skillIds[selectedSkillId]).GetPrintableName() + "</color>?";
    buySkillConfirmationPanel.ShowPanel();
  }


  public void ConfirmSkillBuy() {
    relationship.bondPoints--;
    relationship.unlockedSkill[selectedSkillId] = true;

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
