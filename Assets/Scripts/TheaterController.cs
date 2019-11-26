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


public class TheaterController : MonoBehaviour {

  public static TheaterController instance;

  public Vector3 mainPosition;
  public Vector3 supportPosition;
  public Vector3 angelPosition;
  public Vector3 encounterPosition;
  public Vector3 challengePosition;

  public Actor2D mainActor;
  public Actor2D supportActor;
  public Actor2D angelActor;
  public Actor2D challengeActor;

  public GameObject[] bgObjects;

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
        //mainActor.transform.localPosition = mainPosition;

        supportActor.gameObject.SetActive(true);
        supportActor.SetCharacterGraphics(GlobalData.instance.observedPeople[1]);
        //supportActor.transform.localPosition = supportPosition;

        if(currentEvent == TheaterEvent.date) {
          challengeActor.ShowWeaknessCard(false);

          ChallengeLeavesScene();
        } else {
          DateEvent dateEvent = BattleController.instance.GetCurrentDateEvent();
          BattleController.instance.difficultyText.text = "<size=68>NV </size>" + BattleController.instance.GetCurrentDateEvent().difficulty;

          challengeActor.SetChallengeGraphics(dateEvent);
          if(!string.IsNullOrEmpty(dateEvent.spriteName)) {
            ChallengeEntersScene();
          } else {
            InitializeChallengeLevelAndHp();
          }
          UIController.instance.datingPeoplePanel.ShowPanel();
        }        
        break;
    }
  }

  public void PositionActorsCloseup() {
    mainActor.transform.localPosition = mainPosition;

    supportActor.gameObject.SetActive(true);
    supportActor.transform.localPosition = new Vector3(mainPosition.x, supportPosition.y, supportPosition.z);
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

  public Actor2D GetActorByIdInParty(int actorId) {
    switch(actorId) {
      case 0:
        return mainActor;
      case 1:
        return supportActor;
      case 2:
        return angelActor;
    }
    return null;
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
}
