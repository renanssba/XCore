using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoupleStatusScreen : MonoBehaviour {

  public ScreenTransitions panel;
  public Relationship relationship;
  public RelationshipCard relationshipCard;
  public PersonCard[] personCards;

	public void Initialize(Relationship newRelationship) {
    relationship = newRelationship;
    relationshipCard.Initialize(relationship);
    personCards[0].Initialize(relationship.GetBoy());
    personCards[1].Initialize(relationship.GetGirl());
  }

  public void UpdateUI() {
    relationshipCard.UpdateUI();
    personCards[0].UpdateUI();
    personCards[1].UpdateUI();
  }

  public void ClickRightCoupleButton() {
    int initialRelationship = GetCurrentRelationshipId();
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
    int initialRelationship = GetCurrentRelationshipId();
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

  public int GetCurrentRelationshipId() {
    for(int i = 0; i < GlobalData.instance.relationships.Length; i++) {
      if(GlobalData.instance.relationships[i] == relationship) {
        return i;
      }
    }
    return -1;
  }

  public void ClickExitButton() {
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
    panel.HidePanel();
  }
}
