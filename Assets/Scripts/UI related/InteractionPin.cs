using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionPin : MonoBehaviour {
  public Image characterIcon;
  public CanvasGroup descriptionPanel;
  public TextMeshProUGUI descriptionText;

  public ExampleEntryPoint vsnEntryPoint;
  public int charId = -1;


  public void SetPinContent(string scriptToLoad, string location) {
    vsnEntryPoint.scriptToPlay = scriptToLoad;
    if(GetPerson() != null) {
      descriptionText.text = GetPerson().GetName() + "\n<size=16>" + location + "</size>";
    } else {
      descriptionText.text = location;
    }
  }

  public void SetSprite(Sprite s) {
    characterIcon.sprite = s;
  }

  public void SetLocation(string locationName) {
    descriptionText.text = locationName;
  }

  public Pilot GetPerson() {
    if(charId <0 || charId > GlobalData.instance.pilots.Count-1) {
      return null;
    }
    return GlobalData.instance.pilots[charId];
  }


  public void ClickInteractionPin() {
    SfxManager.StaticPlayConfirmSfx();
    GlobalData.instance.currentRelationshipId = charId-1;
    //UIController.instance.relationshipUpAnimationCard.Initialize(GlobalData.instance.GetCurrentRelationship());
    vsnEntryPoint.LoadScript();
  }
}
