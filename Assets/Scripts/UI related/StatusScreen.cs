using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusScreen : MonoBehaviour {

  public static StatusScreen instance;

  [Header("- Data -")]
  public Pilot pilot;

  [Header("- Panels -")]
  public Panel panel;
  public PilotDetailsCard pilotCard;
  public SkilltreeScreen skilltreeScreen;

  [Header("- Relationship Cards -")]
  public GameObject relationshipCardPrefab;
  public Transform relationshipsContent;

  [Header("- Skilltree Button -")]
  public Button skilltreeButton;
  public Image skilltreeButtonShade;
  public GameObject unusedBondPointsIcon;

  [Header("- Skill Colors -")]
  public Color activeSkillButtonColor;
  public Color passiveSkillButtonColor;
  public Color debuffSkillButtonColor;
  public Color lockedSkillButtonColor;


  public void Awake() {
    instance = this;
  }

  public void Initialize(Pilot currentPilot) {
    instance = this;

    pilot = currentPilot;
    UpdateUI();
  }

  public void UpdateUI() {
    pilotCard.Initialize(pilot);

    if(BattleController.instance.IsBattleHappening()){
      skilltreeButtonShade.gameObject.SetActive(true);
      unusedBondPointsIcon.SetActive(false);
    } else {
      skilltreeButtonShade.gameObject.SetActive(false);
      unusedBondPointsIcon.SetActive(pilot.skillPoints != 0);
    }

    /// relationship cards
    relationshipsContent.ClearChildren();
    foreach(Relationship rel in pilot.GetRelationships()) {
      GameObject newobj = Instantiate(relationshipCardPrefab, relationshipsContent);
      newobj.GetComponentInChildren<RelationshipCard>().Initialize(rel);
    }
    //relationshipCard.Initialize(statusPerson);
  }



  public void ClickRightCoupleButton() {
    int initialPerson = pilot.id;
    int personId = initialPerson;

    SfxManager.StaticPlaySelectSfx();
    personId++;
    if(personId >= GlobalData.instance.pilots.Count) {
      personId = 0;
    }
    Initialize(GlobalData.instance.pilots[personId]);
  }

  public void ClickLeftCoupleButton() {
    int initialPerson = pilot.id;
    int personId = initialPerson;

    SfxManager.StaticPlaySelectSfx();
    personId--;
    if(personId < 0) {
      personId = GlobalData.instance.pilots.Count - 1;
    }
    Initialize(GlobalData.instance.pilots[personId]);
  }

  public void ClickExitButton() {
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
    panel.HidePanel();
  }
}
