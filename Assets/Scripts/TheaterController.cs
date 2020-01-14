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
  public Actor2D enemyActor;

  public GameObject[] bgObjects;

  public float intensity;


  public void Awake() {
    instance = this;
  }

  public void SetEvent(TheaterEvent currentEvent) {
    Debug.LogWarning("SET THEATER EVENT: " + currentEvent);

    //mainActor.SetCharacterGraphics(GlobalData.instance.observedPeople[0]);
    //supportActor.SetCharacterGraphics(GlobalData.instance.observedPeople[1]);

    //mainActor.transform.localPosition = mainPosition;
    //supportActor.gameObject.SetActive(true);
    //supportActor.transform.localPosition = supportPosition;

    //if(currentEvent == TheaterEvent.date) {
    //  enemyActor.ShowWeaknessCard(false);

    //  EnemyLeavesScene();
    //} else {
    //  SetupEnemySprites();
    //}
  }

  public void SetupEnemySprites() {
    DateEvent dateEvent = BattleController.instance.GetCurrentDateEvent();
    BattleController.instance.difficultyText.text = "<size=68>NV </size>" + BattleController.instance.GetCurrentDateEvent().level;

    enemyActor.SetEnemyGraphics(dateEvent);
    //if(!string.IsNullOrEmpty(dateEvent.spriteName)) {
    //  EnemyEntersScene();
    //} else {
    //  InitializeChallengeLevelAndHp();
    //}
  }

  public void PositionActorsCloseup() {
    mainActor.transform.localPosition = mainPosition;
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
    enemyActor.EnemyAttackAnim();
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

  public Actor2D GetActorByPerson(Person p) {
    if(mainActor.person == p) {
      return mainActor;
    }
    if(supportActor.person == p) {
      return supportActor;
    }
    if(angelActor.person == p) {
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

  public void SetupDate() {
    Vector3 distance = new Vector3(3f, 0f, 0f);
    mainActor.SetCharacterGraphics(GlobalData.instance.observedPeople[0]);
    supportActor.SetCharacterGraphics(GlobalData.instance.observedPeople[1]);

    mainActor.transform.localPosition = mainPosition - distance;
    supportActor.transform.localPosition = supportPosition - distance;
    angelActor.transform.localPosition = angelPosition - distance;

    enemyActor.transform.localPosition = challengePosition + distance;
    enemyActor.ShowWeaknessCard(false);
  }

  public void PartyEntersScene() {
    // TODO: implement
    mainActor.transform.DOLocalMoveX(3f, 0.5f).SetRelative(true);
    supportActor.transform.DOLocalMoveX(3f, 0.5f).SetRelative(true);
    angelActor.transform.DOLocalMoveX(3f, 0.5f).SetRelative(true);
  }

  public void EnemyEntersScene() {
    DateEvent currentChallenge = BattleController.instance.GetCurrentDateEvent();

    SetupEnemySprites();

    currentChallenge.hp = currentChallenge.maxHp;

    VsnAudioManager.instance.PlaySfx("challenge_default");
    enemyActor.gameObject.SetActive(true);
    enemyActor.transform.localPosition = challengePosition + new Vector3(2.5f, 0f, 0f);
    enemyActor.transform.DOLocalMoveX(challengePosition.x, 0.5f).OnComplete(() => {
      InitializeChallengeLevelAndHp();
    });
  }

  public void InitializeChallengeLevelAndHp() {
    BattleController.instance.enemyHpSlider.maxValue = BattleController.instance.GetCurrentDateEvent().maxHp;
    BattleController.instance.enemyHpSlider.value = BattleController.instance.enemyHpSlider.maxValue;
    BattleController.instance.enemyHpSlider.gameObject.SetActive(true);
  }

  public void EnemyLeavesScene() {
    BattleController.instance.enemyHpSlider.gameObject.SetActive(false);

    enemyActor.ShowWeaknessCard(false);

    enemyActor.transform.DOLocalMoveX(2.5f, 0.5f).SetRelative().OnComplete(() => {
      enemyActor.gameObject.SetActive(false);
    });
  }


  public void SetLocation(string place) {

    foreach(GameObject c in bgObjects) {
      c.SetActive(false);
    }

    switch(place) {
      case "park":
        bgObjects[0].SetActive(true);
        break;
      case "shopping":
        bgObjects[1].SetActive(true);
        break;
      case "street":
        bgObjects[2].SetActive(true);
        break;
    }
  }
}
