using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TheaterEvent {
  observation,
  date,
  dateChallenge
}


public class TheaterController : MonoBehaviour {

  public static TheaterController instance;

  public SpriteRenderer bgRenderer;

  public Vector3 mainPosition;
  public Vector3 supportPosition;
  public Vector3 encounterPosition;
  public Vector3 challengePosition;

  public Vector2 mainActiveCardPosition;
  public Vector2 supportActiveCardPosition;

  public MeshRenderer floor;
  public SpriteRenderer mainActor;
  public SpriteRenderer supportActor;
  public SpriteRenderer challengeActor;

  public Sprite[] peopleSprites;

  public Material[] floorMaterials;

  public Color inactiveColor;


  public void Awake() {
    instance = this;
  }

  public void SetEvent(TheaterEvent currentEvent) {
    Debug.LogWarning("SET THEATER EVENT: " + currentEvent);

    switch(currentEvent) {
      case TheaterEvent.observation:
        ObservationEventType evtType = GameController.instance.GetCurrentObservationEvent().eventType;
        mainActor.transform.localPosition = mainPosition;
        mainActor.sprite = GlobalData.instance.ObservedPerson().isMale ? peopleSprites[0] : peopleSprites[1];
        challengeActor.gameObject.SetActive(false);

        switch(evtType) {
          case ObservationEventType.attributeTraining:
            mainActor.transform.Translate(-mainActor.transform.localPosition.x, 0f, 0f);
            supportActor.gameObject.SetActive(false);
            break;
          case ObservationEventType.femaleInTrouble:
          case ObservationEventType.maleInTrouble:
            supportActor.gameObject.SetActive(true);
            supportActor.transform.localPosition = encounterPosition;
            supportActor.transform.localEulerAngles = 180f*Vector3.up;
            if(evtType == ObservationEventType.maleInTrouble) {
              supportActor.sprite = peopleSprites[0];
            } else {
              supportActor.sprite = peopleSprites[1];
            }
            break;
        }        
        break;
      case TheaterEvent.date:
      case TheaterEvent.dateChallenge:
        mainActor.transform.localPosition = mainPosition;
        mainActor.sprite = GlobalData.instance.CurrentBoy().isMale ? peopleSprites[0] : peopleSprites[1];
        supportActor.sprite = GlobalData.instance.CurrentGirl().isMale ? peopleSprites[0] : peopleSprites[1];
        challengeActor.gameObject.SetActive(true);

        supportActor.gameObject.SetActive(true);
        supportActor.transform.localPosition = supportPosition;
        supportActor.transform.localEulerAngles = Vector3.zero;

        if(currentEvent == TheaterEvent.date) {
          GameController.instance.actionPersonCard.HidePanel();
          mainActor.color = Color.white;
          supportActor.color = Color.white;
        } else {
          switch(GameController.instance.GetCurrentDateEvent().interactionType) {
            case DateEventInteractionType.male:
              mainActor.color = Color.white;
              supportActor.color = inactiveColor;
              GameController.instance.actionPersonCard.GetComponent<RectTransform>().anchoredPosition = mainActiveCardPosition;
              GameController.instance.actionPersonCard.GetComponent<PersonCard>().Initialize(GlobalData.instance.CurrentBoy());
              break;
            case DateEventInteractionType.female:
              mainActor.color = inactiveColor;
              supportActor.color = Color.white;
              GameController.instance.actionPersonCard.GetComponent<RectTransform>().anchoredPosition = supportActiveCardPosition;
              GameController.instance.actionPersonCard.GetComponent<PersonCard>().Initialize(GlobalData.instance.CurrentGirl());
              break;
            case DateEventInteractionType.couple:
              mainActor.color = Color.white;
              supportActor.color = Color.white;
              break;
          }
          GameController.instance.actionPersonCard.ShowPanel();
        }        
        break;
    }
  }


  public void SetBgSprite(Sprite s) {
    bgRenderer.sprite = s;
    bgRenderer.gameObject.SetActive(true);

    floor.material = GetMaterial(s.name);
  }

  public Material GetMaterial(string name) {
    foreach(Material m in floorMaterials) {
      if(m.name == name) {
        return m;
      }
    }
    return null;
  }

  public void ResetBgSprite() {
    bgRenderer.gameObject.SetActive(false);
  }
}
