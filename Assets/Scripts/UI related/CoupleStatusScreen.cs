using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoupleStatusScreen : MonoBehaviour {

  public static CoupleStatusScreen instance;

  public ScreenTransitions panel;
  public Relationship relationship;
  public RelationshipCard relationshipCard;
  public PersonCard personCard;
  public SkilltreeScreen skilltreeScreen;

  [Header("- Skilltree Button -")]
  public Button skilltreeButton;
  public Image skilltreeButtonShade;
  public GameObject unusedBondPointsIcon;

  public Color activeSkillButtonColor;
  public Color passiveSkillButtonColor;
  public Color debuffSkillButtonColor;
  public Color lockedSkillButtonColor;

  public TextMeshProUGUI coupleHpText;


  public void Awake() {
    instance = this;
  }

  public void Initialize(Relationship newRelationship) {
    instance = this;

    relationship = newRelationship;
    UpdateUI();
  }

  public void UpdateUI() {
    relationshipCard.Initialize(relationship);
    personCard.Initialize(relationship.GetBoy());
    coupleHpText.text = relationship.GetMaxHp().ToString();

    if(BattleController.instance.IsBattleHappening()){
      skilltreeButtonShade.gameObject.SetActive(true);
      unusedBondPointsIcon.SetActive(false);
    } else {
      skilltreeButtonShade.gameObject.SetActive(false);
      unusedBondPointsIcon.SetActive(relationship.bondPoints != 0);
    }    
  }



  public void ClickRightCoupleButton() {
    int initialRelationship = relationship.id;
    int relationshipId = initialRelationship;

    SfxManager.StaticPlaySelectSfx();
    relationshipId++;
    if(relationshipId >= GlobalData.instance.relationships.Length) {
      relationshipId = 0;
    }
    Initialize(GlobalData.instance.relationships[relationshipId]);
  }

  public void ClickLeftCoupleButton() {
    int initialRelationship = relationship.id;
    int relationshipId = initialRelationship;

    SfxManager.StaticPlaySelectSfx();
    relationshipId--;
    if(relationshipId < 0) {
      relationshipId = GlobalData.instance.relationships.Length - 1;
    }
    Initialize(GlobalData.instance.relationships[relationshipId]);
  }

  public void ClickExitButton() {
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
    panel.HidePanel();
  }
}
