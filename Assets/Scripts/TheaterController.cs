using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public enum TheaterEvent {
  observation,
  date,
  dateChallenge
}

public enum CameraPosition {
  mainCamera,
  closeupCamera
}


public class TheaterController : MonoBehaviour {

  public static TheaterController instance;

  public Vector3 cameraMainPosition;
  public Vector3 cameraCloseupPosition;

  public Vector3 mainPosition;
  public Vector3 supportPosition;
  public Vector3 encounterPosition;
  public Vector3 challengePosition;

  public Vector3 rotationFacingRight;
  public Vector3 rotationFacingLeft;
  public Vector3 rotationFacingFront;

  public Vector2 mainActiveCardPosition;
  public Vector2 supportActiveCardPosition;

  public Actor3D mainActor;
  public Actor3D supportActor;
  public SpriteRenderer challengeActor;

  public GameObject damageParticlePrefab;

  public Image attributeIcon;
  public Slider hpSlider;

  public GameObject[] bgObjects;

  public Color greenColor;
  public Color redColor;

  public float intensity;

  const float animTime = 0.15f;


  public void Awake() {
    instance = this;
  }

  public void SetEvent(TheaterEvent currentEvent) {
    Debug.LogWarning("SET THEATER EVENT: " + currentEvent);

    mainActor.SetGraphics(GlobalData.instance.observedPeople[0]);

    switch(currentEvent) {
      case TheaterEvent.observation:
        ObservationEventType evtType = GameController.instance.GetCurrentObservationEvent().eventType;
        PositionCamera(CameraPosition.mainCamera);

        mainActor.transform.localPosition = mainPosition;
        mainActor.transform.localEulerAngles = rotationFacingRight;
        //mainActor.sprite = GlobalData.instance.ObservedPerson().isMale ? peopleSprites[0] : peopleSprites[1];
        challengeActor.gameObject.SetActive(false);

        switch(evtType) {
          case ObservationEventType.attributeTraining:
            mainActor.transform.localPosition = new Vector3(0f, mainPosition.y, mainPosition.z);
            supportActor.gameObject.SetActive(false);
            break;
          case ObservationEventType.femaleInTrouble:
          case ObservationEventType.maleInTrouble:
            supportActor.SetGraphics(GlobalData.instance.observedPeople[1]);
            supportActor.gameObject.SetActive(true);
            supportActor.transform.localPosition = encounterPosition;
            supportActor.transform.localEulerAngles = rotationFacingLeft;
            break;
        }        
        break;
      case TheaterEvent.date:
      case TheaterEvent.dateChallenge:
        PositionCamera(CameraPosition.mainCamera);

        mainActor.transform.localPosition = mainPosition;
        mainActor.transform.localEulerAngles = rotationFacingRight;

        supportActor.gameObject.SetActive(true);
        supportActor.SetGraphics(GlobalData.instance.observedPeople[1]);
        supportActor.transform.localPosition = supportPosition;
        supportActor.transform.localEulerAngles = rotationFacingRight;

        if(currentEvent == TheaterEvent.date) {
          if(VsnSaveSystem.GetIntVariable("currentDateEvent") >= 7) {
            PositionCamera(CameraPosition.closeupCamera);
            PositionActorsCloseup();
          }
          GameController.instance.actionPersonCard.HidePanel();
          ChallengeLeavesScene();
        } else {
          string spriteName = GameController.instance.GetCurrentDateEvent().spriteName;

          if(!string.IsNullOrEmpty(spriteName)) {
            ChallengeEntersScene(LoadSprite("Challenges/" + spriteName));
          } else {
            PositionCamera(CameraPosition.closeupCamera);
            PositionActorsCloseup();
          }

          switch(GameController.instance.GetCurrentDateEvent().interactionType) {
            case DateEventInteractionType.male:
              mainActor.SetBrightness(1f);
              supportActor.SetBrightness(intensity);
              GameController.instance.actionPersonCard.GetComponent<RectTransform>().anchoredPosition = mainActiveCardPosition;
              GameController.instance.actionPersonCard.GetComponent<PersonCard>().Initialize(GlobalData.instance.CurrentBoy());
              break;
            case DateEventInteractionType.female:
              mainActor.SetBrightness(intensity);
              supportActor.SetBrightness(1f);
              GameController.instance.actionPersonCard.GetComponent<RectTransform>().anchoredPosition = supportActiveCardPosition;
              GameController.instance.actionPersonCard.GetComponent<PersonCard>().Initialize(GlobalData.instance.CurrentGirl());
              break;
            case DateEventInteractionType.couple:
              mainActor.SetBrightness(1f);
              supportActor.SetBrightness(1f);
              break;
          }

          if(string.IsNullOrEmpty(spriteName)) {
            mainActor.SetBrightness(1f);
            supportActor.SetBrightness(1f);
          }

          GameController.instance.actionPersonCard.ShowPanel();
        }        
        break;
    }
  }

  public void SetupActorGraphics(TheaterEvent currentEvent) {
    switch(currentEvent) {
      case TheaterEvent.observation:
        //mainActor.sprite = GlobalData.instance.ObservedPerson().isMale ? peopleSprites[0] : peopleSprites[1];
        break;
      case TheaterEvent.date:
      case TheaterEvent.dateChallenge:
        //mainActor.sprite = GlobalData.instance.CurrentBoy().isMale ? peopleSprites[0] : peopleSprites[1];
        //supportActor.sprite = GlobalData.instance.CurrentGirl().isMale ? peopleSprites[0] : peopleSprites[1];
        break;
    }
  }

  public void PositionCamera(CameraPosition camPos) {
    switch(camPos) {
      case CameraPosition.mainCamera:
        Camera.main.transform.position = cameraMainPosition;
        Camera.main.transform.eulerAngles = Vector3.zero;
        break;
      case CameraPosition.closeupCamera:
        Camera.main.transform.position = cameraCloseupPosition;
        Camera.main.transform.eulerAngles = 90f*Vector3.down;
        break;
    }
  }

  public void PositionActorsCloseup() {
    mainActor.transform.localPosition = mainPosition;
    mainActor.transform.localEulerAngles = rotationFacingFront;

    supportActor.gameObject.SetActive(true);
    supportActor.transform.localPosition = new Vector3(mainPosition.x, supportPosition.y, supportPosition.z);
    supportActor.transform.localEulerAngles = rotationFacingFront + new Vector3(0f, 90f, 0f);
  }

  public void ShowParticleAnimation(int attribute, int attributeLevel) {
    GameObject newobj = Instantiate(damageParticlePrefab, GameController.instance.bgImage.transform.parent);
    newobj.GetComponent<TextMeshProUGUI>().text = attributeLevel.ToString();
    newobj.GetComponent<TextMeshProUGUI>().color = ResourcesManager.instance.attributeColor[attribute];
  }

  public void ShowChallengeResult(bool success) {
    GameObject newobj = Instantiate(damageParticlePrefab, GameController.instance.bgImage.transform.parent);

    newobj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

    newobj.GetComponent<JumpingParticle>().duration = 1f;
    if(success == true) {
      newobj.GetComponent<JumpingParticle>().jumpForce = 10;
    } else {
      newobj.GetComponent<JumpingParticle>().jumpForce = 0;
    }
    newobj.GetComponent<TextMeshProUGUI>().text = success ?
      Lean.Localization.LeanLocalization.GetTranslationText("date/success_message") :
      Lean.Localization.LeanLocalization.GetTranslationText("date/failure_message");
    newobj.GetComponent<TextMeshProUGUI>().color = success ? greenColor : redColor;
  }

  public void ActorAttackAnimation() {
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
    obj.DOMoveX(0.3f, animTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public IEnumerator WaitAndShine(float time) {
    int selected = VsnSaveSystem.GetIntVariable("selected_attribute");
    int selectedAttributeLevel = GlobalData.instance.EventSolvingAttributeLevel(selected);

    VsnAudioManager.instance.PlaySfx("hit_default");

    yield return new WaitForSeconds(time);

    FlashRenderer(challengeActor.transform, 0.1f, 0.8f, 0.2f);
    ShowParticleAnimation(selected, selectedAttributeLevel);

    yield return new WaitForSeconds(0.4f);

    attributeIcon.sprite = ResourcesManager.instance.attributeSprites[selected];
    attributeIcon.color = ResourcesManager.instance.attributeColor[selected];
    hpSlider.value = 0;
    hpSlider.fillRect.GetComponent<Image>().color = ResourcesManager.instance.attributeColor[selected];
    hpSlider.maxValue = GameController.instance.GetCurrentDateEvent().difficultyForAttribute[selected];
    hpSlider.gameObject.SetActive(true);
    hpSlider.DOValue(selectedAttributeLevel, 1f);
  }

  public void FlashRenderer(Transform obj, float minFlash, float maxFlash, float flashTime) {
    SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
    DOTween.Kill(spriteRenderer.material);
    spriteRenderer.material.SetFloat("_FlashAmount", minFlash);
    spriteRenderer.material.DOFloat(maxFlash, "_FlashAmount", flashTime).SetLoops(2, LoopType.Yoyo);
  }

  public void ChallengeEntersScene(Sprite sp) {
    if(challengeActor.gameObject.activeSelf == false) {
      VsnAudioManager.instance.PlaySfx("challenge_default");
      challengeActor.sprite = sp;
      challengeActor.gameObject.SetActive(true);
      challengeActor.transform.localPosition = challengePosition+new Vector3(2.5f, 0f, 0f);
      challengeActor.transform.DOLocalMoveX(challengePosition.x, 0.5f);
    }
  }

  public void ChallengeLeavesScene() {
    hpSlider.gameObject.SetActive(false);
    if(challengeActor.gameObject.activeSelf == true) {
      challengeActor.transform.DOLocalMoveX(2.5f, 0.5f).SetRelative().OnComplete(() => {
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

    foreach(GameObject c in bgObjects) {
      c.SetActive(false);
    }

    switch(place) {
      case "parque":
        bgObjects[0].SetActive(true);
        break;
      case "shopping":
        bgObjects[1].SetActive(true);
        break;
      case "rua":
        bgObjects[2].SetActive(true);
        break;
    }

    /// TODO: Teleport to correct map
    /// 
  }
}
