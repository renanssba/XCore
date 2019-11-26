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

  public Actor2D mainActor;
  public Actor2D supportActor;
  public Actor2D angelActor;
  public Actor2D challengeActor;

  public GameObject[] bgObjects;

  public GameObject weaknessCard;
  public TextMeshProUGUI weaknessCardText;

  public Image[] faceImages;

  public float intensity;


  public void Awake() {
    instance = this;
  }

  public void SetEvent(TheaterEvent currentEvent) {
    Debug.LogWarning("SET THEATER EVENT: " + currentEvent);

    mainActor.SetCharacterGraphics(GlobalData.instance.observedPeople[0]);
    faceImages[0].sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.CurrentBoy().faceId);
    faceImages[1].sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.CurrentGirl().faceId);


    switch(currentEvent) {
      case TheaterEvent.date:
      case TheaterEvent.dateChallenge:
        PositionCamera(CameraPosition.mainCamera);

        //mainActor.transform.localPosition = mainPosition;
        //mainActor.transform.localEulerAngles = rotationFacingRight;

        supportActor.gameObject.SetActive(true);
        supportActor.SetCharacterGraphics(GlobalData.instance.observedPeople[1]);
        //supportActor.transform.localPosition = supportPosition;
        //supportActor.transform.localEulerAngles = rotationFacingRight;

        if(currentEvent == TheaterEvent.date) {
          //if(VsnSaveSystem.GetIntVariable("currentDateEvent") >= 7) {
          //  PositionCamera(CameraPosition.closeupCamera);
          //  PositionActorsCloseup();
          //}
          ShowWeaknessCard(false);

          ChallengeLeavesScene();
        } else {
          DateEvent dateEvent = BattleController.instance.GetCurrentDateEvent();
          BattleController.instance.difficultyText.text = "<size=68>NV </size>" + BattleController.instance.GetCurrentDateEvent().difficulty;

          challengeActor.SetChallengeGraphics(dateEvent);
          if(!string.IsNullOrEmpty(dateEvent.spriteName)) {
            ChallengeEntersScene();
          } else {
            //PositionCamera(CameraPosition.closeupCamera);
            //PositionActorsCloseup();
            InitializeChallengeLevelAndHp();
          }
          UIController.instance.datingPeoplePanel.ShowPanel();
        }        
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

  public void CharacterAttackAnimation(int actorId, int animId) {
    switch(actorId) {
      case 0:
        mainActor.CharacterAttackAnim();
        break;
      case 1:
        supportActor.CharacterAttackAnim();
        break;
      case 2:
        angelActor.CharacterAttackAnim();
        break;
    }
  }

  public void EnemyAttackAnimation() {
    challengeActor.EnemyAttackAnim();
  }

  public void ShineCharacter(int actorId) {
    switch(actorId) {
      case 0:
        mainActor.Shine();
        break;
      case 1:
        supportActor.Shine();
        break;
      case 2:
        angelActor.Shine();
        break;
    }
  }


  public void FlashRenderer(Transform obj, float minFlash, float maxFlash, float flashTime) {
    SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
    DOTween.Kill(spriteRenderer.material);
    spriteRenderer.material.SetFloat("_FlashAmount", minFlash);
    spriteRenderer.material.DOFloat(maxFlash, "_FlashAmount", flashTime).SetLoops(2, LoopType.Yoyo);
  }

  public void ChallengeEntersScene() {
    DateEvent currentChallenge = BattleController.instance.GetCurrentDateEvent();

    currentChallenge.hp = currentChallenge.maxHp;

    if(challengeActor.gameObject.activeSelf == false) {
      VsnAudioManager.instance.PlaySfx("challenge_default");
      challengeActor.gameObject.SetActive(true);
      challengeActor.transform.localPosition = challengePosition + new Vector3(2.5f, 0f, 0f);
      challengeActor.transform.DOLocalMoveX(challengePosition.x, 0.5f).OnComplete(()=> {
        InitializeChallengeLevelAndHp();
      });
    }
  }

  public void InitializeChallengeLevelAndHp() {
    BattleController.instance.enemyHpSlider.maxValue = BattleController.instance.GetCurrentDateEvent().maxHp;
    BattleController.instance.enemyHpSlider.value = BattleController.instance.enemyHpSlider.maxValue;
    BattleController.instance.enemyHpSlider.gameObject.SetActive(true);
  }

  public void ChallengeLeavesScene() {
    BattleController.instance.enemyHpSlider.gameObject.SetActive(false);
    if(challengeActor.gameObject.activeSelf == true) {
      challengeActor.transform.DOLocalMoveX(2.5f, 0.5f).SetRelative().OnComplete(() => {
        challengeActor.gameObject.SetActive(false);
      });
    }
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
  }

  public void ShowWeaknessCard(bool activate) {
    weaknessCard.SetActive(activate);
    if(activate) {
      VsnAudioManager.instance.PlaySfx("relationship_up");
      SetWeaknessCardText();
    }
  }

  public void SetWeaknessCardText() {
    string text = "";
    DateEvent dateChallenge = BattleController.instance.GetCurrentDateEvent();
    Attributes[] weak = dateChallenge.GetWeaknesses();
    Attributes[] resistant = dateChallenge.GetResistances();

    if(weak.Length>0) {
      text += "Fraqueza:\n";
      for(int i=0; i<weak.Length; i++) {
        text += Lean.Localization.LeanLocalization.GetTranslationText("attribute/" + weak[i].ToString()) + "\n";
      }
    }
    if(resistant.Length > 0) {
      if(!string.IsNullOrEmpty(text)) {
        text += "\n";
      }
      text += "Resistência:\n";
      for(int i = 0; i < resistant.Length; i++) {
        text += Lean.Localization.LeanLocalization.GetTranslationText("attribute/" + resistant[i].ToString()) + "\n";
      }
    }

    weaknessCardText.text = text;
  }
}
