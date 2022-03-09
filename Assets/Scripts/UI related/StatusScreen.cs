using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusScreen : MonoBehaviour {

  public static StatusScreen instance;

  [Header("- Data -")]
  public Person statusPerson;

  [Header("- Panels -")]
  public ScreenTransitions panel;
  public PersonCard personCard;
  public SkilltreeScreen skilltreeScreen;

  [Header("- Relationship Cards -")]
  //public RelationshipCard relationshipCard;
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

  public TextMeshProUGUI coupleHpText;


  public void Awake() {
    instance = this;
  }

  public void Initialize(Person personToAnalyze) {
    instance = this;

    statusPerson = personToAnalyze;
    UpdateUI();
  }

  public void UpdateUI() {
    personCard.Initialize(statusPerson);
    coupleHpText.text = statusPerson.MaxHP().ToString();

    if(BattleController.instance.IsBattleHappening()){
      skilltreeButtonShade.gameObject.SetActive(true);
      unusedBondPointsIcon.SetActive(false);
    } else {
      skilltreeButtonShade.gameObject.SetActive(false);
      unusedBondPointsIcon.SetActive(statusPerson.skillPoints != 0);
    }

    /// relationship cards
    relationshipsContent.ClearChildren();
    foreach(Relationship rel in statusPerson.GetRelationships()) {
      GameObject newobj = Instantiate(relationshipCardPrefab, relationshipsContent);
      newobj.GetComponentInChildren<RelationshipCard>().Initialize(rel);
    }
    //relationshipCard.Initialize(statusPerson);
  }



  public void ClickRightCoupleButton() {
    int initialPerson = statusPerson.id;
    int personId = initialPerson;

    SfxManager.StaticPlaySelectSfx();
    personId++;
    if(personId >= GlobalData.instance.people.Count) {
      personId = 0;
    }
    Initialize(GlobalData.instance.people[personId]);
  }

  public void ClickLeftCoupleButton() {
    int initialPerson = statusPerson.id;
    int personId = initialPerson;

    SfxManager.StaticPlaySelectSfx();
    personId--;
    if(personId < 0) {
      personId = GlobalData.instance.people.Count - 1;
    }
    Initialize(GlobalData.instance.people[personId]);
  }

  public void ClickExitButton() {
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
    panel.HidePanel();
  }
}
