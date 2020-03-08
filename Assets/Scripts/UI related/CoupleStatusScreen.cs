using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoupleStatusScreen : MonoBehaviour {

  public static CoupleStatusScreen instance;

  public ScreenTransitions panel;
  public Relationship relationship;
  public RelationshipCard relationshipCard;
  public PersonCard[] personCards;

  public TextMeshProUGUI coupleHpText;

  public void Awake() {
    instance = this;
  }

  public void Initialize(Relationship newRelationship) {
    instance = this;
    relationship = newRelationship;
    relationshipCard.Initialize(relationship);
    personCards[0].Initialize(relationship.GetBoy());
    personCards[1].Initialize(relationship.GetGirl());
  }

  public void UpdateUI() {
    relationshipCard.UpdateUI();
    personCards[0].UpdateUI();
    personCards[1].UpdateUI();
    coupleHpText.text = "HP: "+relationship.GetMaxHp().ToString();
  }

  public void ClickRightCoupleButton() {
    int initialRelationship = relationship.id;
    int relationshipId = initialRelationship;

    SfxManager.StaticPlaySelectSfx();
    do {
      relationshipId++;
      if(relationshipId >= GlobalData.instance.relationships.Length) {
        relationshipId = 0;
      }
    } while(GlobalData.instance.relationships[relationshipId].exp <= 0 && relationshipId != initialRelationship);
    Initialize(GlobalData.instance.relationships[relationshipId]);
  }

  public void ClickLeftCoupleButton() {
    int initialRelationship = relationship.id;
    int relationshipId = initialRelationship;

    SfxManager.StaticPlaySelectSfx();
    do {
      relationshipId--;
      if(relationshipId < 0) {
        relationshipId = GlobalData.instance.relationships.Length-1;
      }
    } while(GlobalData.instance.relationships[relationshipId].exp <= 0 && relationshipId != initialRelationship);
    Initialize(GlobalData.instance.relationships[relationshipId]);
  }

  public void ClickExitButton() {
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
    panel.HidePanel();
  }
}
