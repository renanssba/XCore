using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


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

  public Slider hpSlider;

  public Sprite[] peopleSprites;

  public Material[] floorMaterials;

  public Color inactiveColor;

  const float animTime = 0.15f;


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

        supportActor.gameObject.SetActive(true);
        supportActor.transform.localPosition = supportPosition;
        supportActor.transform.localEulerAngles = Vector3.zero;

        if(currentEvent == TheaterEvent.date) {
          GameController.instance.actionPersonCard.HidePanel();
          mainActor.color = Color.white;
          supportActor.color = Color.white;
          ChallengeLeavesScene();
        } else {
          ChallengeEntersScene(LoadSprite("Challenges/" + GameController.instance.GetCurrentDateEvent().spriteName));
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

  public void ActorAnimation() {
    DateEventInteractionType interactionType = GameController.instance.GetCurrentDateEvent().interactionType;
    switch(interactionType) {
      case DateEventInteractionType.male:
        AnimTransform(mainActor.transform);
        break;
      case DateEventInteractionType.female:
        AnimTransform(supportActor.transform);
        break;
      case DateEventInteractionType.couple:
        AnimTransform(mainActor.transform);
        AnimTransform(supportActor.transform);
        break;
    }
    StartCoroutine(WaitAndShine(animTime));
  }

  public void AnimTransform(Transform obj) {
    obj.DOMoveX(1.5f, animTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public IEnumerator WaitAndShine(float time) {
    yield return new WaitForSeconds(time);
    FlashRenderer(challengeActor.transform, 0.1f, 0.8f, 0.2f);

    yield return new WaitForSeconds(0.4f);

    int selected = VsnSaveSystem.GetIntVariable("selected_attribute");

    hpSlider.value = 0;
    hpSlider.maxValue = GameController.instance.GetCurrentDateEvent().difficultyForAttribute[selected];
    hpSlider.gameObject.SetActive(true);
    hpSlider.DOValue(GlobalData.instance.EventSolvingAttributeLevel(selected), 1f);
  }

  public void FlashRenderer(Transform obj, float minFlash, float maxFlash, float flashTime) {
    SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
    DOTween.Kill(spriteRenderer.material);
    spriteRenderer.material.SetFloat("_FlashAmount", minFlash);
    spriteRenderer.material.DOFloat(maxFlash, "_FlashAmount", flashTime).SetLoops(2, LoopType.Yoyo);
  }

  public void ChallengeEntersScene(Sprite sp) {
    if(challengeActor.gameObject.activeSelf == false) {
      challengeActor.sprite = sp;
      challengeActor.gameObject.SetActive(true);
      challengeActor.transform.localPosition = challengePosition+new Vector3(7f, 0f, 0f);
      challengeActor.transform.DOLocalMoveX(challengePosition.x, 0.5f);
      }
  }

  public void ChallengeLeavesScene() {
    hpSlider.gameObject.SetActive(false);
    if(challengeActor.gameObject.activeSelf == true) {
      challengeActor.transform.DOLocalMoveX(7f, 0.5f).SetRelative().OnComplete(() => {
        challengeActor.gameObject.SetActive(false);
      });
    }
  }

  public Sprite LoadSprite(string sprite) {
    Sprite backgroundSprite = Resources.Load<Sprite>(sprite);
    if(backgroundSprite == null) {
      Debug.LogError("Error loading " + sprite + " sprite. Please check its path");
    }
    return backgroundSprite;
  }


  public void SetLocation(string place) {
    SetBgSprite(LoadSprite("Bg/"+place));
  }

  public void SetBgSprite(Sprite s) {
    if(s == null) {
      Debug.LogWarning("Trying to set bg to null");
      return;
    }
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
