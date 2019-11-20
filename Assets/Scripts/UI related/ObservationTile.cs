using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObservationTile : MonoBehaviour {

	public ObservationEvent evt;
  public Image tileImage;
  public Image iconImage;
  public TextMeshProUGUI titleText;
  public Person personInEvent;

  public GameObject clickableHighlight;

  public bool wasUsed = false;

  public const float tileDistanceTolerance = 230f;

 // void OnEnable() {
 //   UpdateUI();
 // }

 // public void UpdateUI() {
 //   titleText.text = "";
 //   if (wasUsed){
 //     tileImage.color = GameController.instance.observationTilesColors[6];
 //     iconImage.gameObject.SetActive(false);
 //     clickableHighlight.SetActive(false);
 //     return;
 //   }

 //   if(IsCloseToToken() && !wasUsed) {
 //     clickableHighlight.SetActive(true);
 //   } else {
 //     clickableHighlight.SetActive(false);
 //   }

 //   iconImage.gameObject.SetActive(true);
 //   tileImage.color = GameController.instance.observationTilesColors[(int)evt.eventType];
 //   switch (evt.eventType){
 //     case ObservationEventType.attributeTraining:
 //       iconImage.sprite = ResourcesManager.instance.attributeSprites[(int)evt.challengedAttribute];
 //       break;
 //     case ObservationEventType.femaleInTrouble:
 //     case ObservationEventType.maleInTrouble:
 //       if(personInEvent.state == PersonState.unrevealed) {
 //         iconImage.sprite = ResourcesManager.instance.unknownSprite;
 //         titleText.text = "???";
 //       } else {
 //         iconImage.sprite = ResourcesManager.instance.GetFaceSprite(personInEvent.faceId);
 //         titleText.text = personInEvent.name;
 //       }        
 //       break;
 //   }
 // }
	
	//public void ClickedTile(){
 //   if(wasUsed){ 
 //     SfxManager.StaticPlayForbbidenSfx();
 //     VsnController.instance.StartVSN("observation_clicked_used");
 //     return;
 //   }

 //   if(!IsCloseToToken()){
 //     SfxManager.StaticPlayForbbidenSfx();
 //     VsnController.instance.StartVSN("observation_clicked_far");
 //     return;
 //   }

 //   VsnSaveSystem.SetVariable("observation_energy", VsnSaveSystem.GetIntVariable("observation_energy")-1);
 //   UIController.instance.UpdateUI();

 //   GameController.instance.observationSegments = new ObservationEvent[1];
 //   GlobalData.instance.observedPeople[1] = personInEvent;
 //   GameController.instance.observationSegments[0] = evt;

 //   Utils.SelectUiElement(null);
 //   JoystickController.instance.CurrentContext().lastSelectedObject = gameObject;
 //   VsnController.instance.StartVSNContent("wait 1", "custom");
 //   GameController.instance.WalkToObservationTile(this);
 // }

 // public bool IsCloseToToken() {
 //   Debug.LogWarning("player token: "+ GameController.instance.playerToken.transform.localPosition + ", this tile: "+ transform.localPosition);
 //   Debug.LogWarning("TILE Distance: "+ Vector3.Distance(GameController.instance.playerToken.transform.localPosition, transform.localPosition));

 //   //return true;

 //   return Vector3.Distance(GameController.instance.playerToken.transform.localPosition, transform.localPosition) <= tileDistanceTolerance;
 // }
}
