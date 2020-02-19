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

  public Camera mainCamera;

  public Vector3 mainPosition;
  public Vector3 supportPosition;
  public Vector3 angelPosition;
  public Vector3 challengePosition;

  public Actor2D mainActor;
  public Actor2D supportActor;
  public Actor2D angelActor;
  public Actor2D enemyActor;

  public GameObject[] bgObjects;


  public void Awake() {
    instance = this;
  }


  public void SpawnEnemyActor() {
    DateEvent dateEvent = BattleController.instance.GetCurrentDateEvent();
    BattleController.instance.difficultyText.text = "<size=68>NV </size>" + dateEvent.level;

    GameObject prefabToSpawn = Resources.Load<GameObject>("Enemy Prefabs/" + dateEvent.scriptName);
    GameObject spawnedEnemy;

    if(prefabToSpawn == null) {
      Debug.LogWarning("No prefab to load for enemy: " + dateEvent.scriptName+". Using default prefab");
      spawnedEnemy = Instantiate(BattleController.instance.defaultEnemyPrefab, challengePosition, Quaternion.identity, transform);
      enemyActor = spawnedEnemy.GetComponent<Actor2D>();
      enemyActor.SetEnemyGraphics(dateEvent);
    } else {
      spawnedEnemy = Instantiate(prefabToSpawn, challengePosition, Quaternion.identity, transform);
      enemyActor = spawnedEnemy.GetComponent<Actor2D>();
    }
  }

  public void DestroyEnemyActor() {
    Destroy(enemyActor.gameObject);
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
        mainActor.ShineRed();
        break;
      case 1:
        supportActor.ShineRed();
        break;
      case 2:
        angelActor.ShineRed();
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

    mainActor.SetBattleMode(false);
    supportActor.SetBattleMode(false);
    angelActor.SetBattleMode(false);

    if(enemyActor != null) {
      DestroyEnemyActor();
    }
  }

  public void PartyEntersScene() {
    float enterAnimationDuration = 1.5f;

    mainActor.transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true).SetEase(Ease.Linear);
    supportActor.transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true).SetEase(Ease.Linear);
    angelActor.transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true).SetEase(Ease.Linear);
  }

  public void EnemyEntersScene() {
    DateEvent currentChallenge = BattleController.instance.GetCurrentDateEvent();

    SpawnEnemyActor();

    currentChallenge.hp = currentChallenge.maxHp;
    
    enemyActor.gameObject.SetActive(true);
    enemyActor.transform.localPosition = challengePosition + new Vector3(2.5f, 0f, 0f);
    enemyActor.transform.DOLocalMoveX(challengePosition.x, 0.5f).OnComplete(() => {
      VsnAudioManager.instance.PlaySfx(currentChallenge.appearSfxName);
      InitializeChallengeLevelAndHp();
      PartyEntersBattleMode();
    });
  }


  public void PartyEntersBattleMode() {
    mainActor.SetBattleMode(true);
    supportActor.SetBattleMode(true);
    angelActor.SetBattleMode(true);
  }

  public void PartyLeavesBattleMode() {
    mainActor.SetBattleMode(false);
    supportActor.SetBattleMode(false);
    angelActor.SetBattleMode(false);
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
      //enemyActor.gameObject.SetActive(false);
      PartyLeavesBattleMode();
      DestroyEnemyActor();
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

  public void MoveCamera(Vector3 newPosition, float time) {
    mainCamera.transform.DOMove(newPosition, time);
  }
}
