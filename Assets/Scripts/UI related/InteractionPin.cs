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
      descriptionText.text = GetPerson().name + "\n<size=16>" + location + "</size>";
    } else {
      descriptionText.text = location;
    }
  }

  public void ResetSprite() {
    if(GetPerson() != null) {
      characterIcon.sprite = ResourcesManager.instance.faceSprites[GetPerson().faceId];
    }
  }

  public void SetSprite(Sprite s) {
    Person p = GlobalData.instance.people[charId];
    characterIcon.sprite = s;
  }

  public Person GetPerson() {
    if(charId <0 || charId > GlobalData.instance.people.Count-1) {
      return null;
    }
    return GlobalData.instance.people[charId];
  }


  public void ClickInteractionPin() {
    SfxManager.StaticPlayConfirmSfx();
    if(charId < GlobalData.instance.people.Count) {
      GlobalData.instance.observedPeople = new Person[] {GlobalData.instance.people[0],
                                                         GlobalData.instance.people[charId]};
    } else {
      GlobalData.instance.observedPeople = new Person[] { GlobalData.instance.people[0] };
    }
    UIController.instance.relationshipUpAnimationCard.Initialize(GlobalData.instance.GetCurrentRelationship());
    vsnEntryPoint.LoadScript();
  }
}
