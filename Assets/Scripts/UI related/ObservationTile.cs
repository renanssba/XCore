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

  public bool wasUsed = false;

  void OnEnable() {
    UpdateUI();
  }

  public void UpdateUI() {
    titleText.text = "";
    if (wasUsed){
      tileImage.color = GameController.instance.observationTilesColors[6];
      iconImage.gameObject.SetActive(false);
      return;
    }

    iconImage.gameObject.SetActive(true);
    tileImage.color = GameController.instance.observationTilesColors[(int)evt.eventType];
    switch (evt.eventType){
      case ObservationEventType.attributeTraining:
        iconImage.sprite = ResourcesManager.instance.attributeSprites[(int)evt.challengedAttribute];
        break;
      case ObservationEventType.femaleInTrouble:
      case ObservationEventType.maleInTrouble:
        iconImage.sprite = ResourcesManager.instance.GetFaceSprite(personInEvent.faceId);
        titleText.text = personInEvent.name;
        break;
    }
  }
	
	public void ClickedTile(){
    if(wasUsed){ 
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    if (Vector3.Distance(GameController.instance.playerToken.transform.position, transform.position) > 1.6f){
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    VsnSaveSystem.SetVariable("observation_energy", VsnSaveSystem.GetIntVariable("observation_energy")-1);
    GameController.instance.UpdateUI();

    GameController.instance.observationSegments = new ObservationEvent[1];
    GlobalData.instance.encounterPerson = personInEvent;
    GameController.instance.observationSegments[0] = evt;

    Utils.SelectUiElement(null);
    VsnController.instance.StartVSNContent("wait 1", "custom");
    GameController.instance.WalkToObservationTile(this);
  }
}
