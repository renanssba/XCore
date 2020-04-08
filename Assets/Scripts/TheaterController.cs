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


public enum BgEffect {
  pulsingEffect,
  distortingEffect,
  count
}


public class TheaterController : MonoBehaviour {

  public static TheaterController instance;

  public Camera mainCamera;

  public Vector3 mainPosition;
  public Vector3 supportPosition;
  public Vector3 angelPosition;
  public Vector3 challengePosition;

  public Vector3[] focusPositionForTwo;
  public Vector3 focusPositionForOne;
  public SpriteRenderer focusShade;
  public Actor2D[] focusedCharacters;

  public Actor2D mainActor;
  public Actor2D supportActor;
  public Actor2D angelActor;
  public Actor2D enemyActor;

  public GameObject[] bgEffectPrefabs;

  public SpriteRenderer bgRenderer;
  public GameObject bgEffect;

  public const float enterAnimationDuration = 1.5f;
  public float focusAnimationDuration = 0.2f;


  public void Awake() {
    instance = this;
  }


  public GameObject SpawnEnemyActor(string enemyPrefabName) {
    Enemy dateEvent = BattleController.instance.GetCurrentEnemy();
    UIController.instance.difficultyText.text = "<size=68>NV </size>" + dateEvent.level;

    GameObject prefabToSpawn = Resources.Load<GameObject>("Enemy Prefabs/" + enemyPrefabName);
    GameObject spawnedEnemy;

    if(prefabToSpawn == null) {
      Debug.LogWarning("No prefab to load for enemy: " + enemyPrefabName + ". Using default prefab");
      spawnedEnemy = Instantiate(BattleController.instance.defaultEnemyPrefab, challengePosition, Quaternion.identity, transform);
      enemyActor = spawnedEnemy.GetComponent<Actor2D>();
      enemyActor.SetEnemy(dateEvent);
      enemyActor.SetEnemyGraphics();
    } else {
      spawnedEnemy = Instantiate(prefabToSpawn, challengePosition, Quaternion.identity, transform);
      enemyActor = spawnedEnemy.GetComponent<Actor2D>();
      enemyActor.SetEnemy(dateEvent);
    }
    return spawnedEnemy;
  }

  public void ChangeActor(string actorName, string newActorPrefabName) {
    Actor2D targetActor = null;

    switch(actorName) {
      case "main":
        targetActor = mainActor;
        break;
      case "support":
        targetActor = supportActor;
        break;
      case "angel":
        targetActor = angelActor;
        break;
      case "enemy":
        targetActor = enemyActor;
        break;
    }

    if(targetActor != null) {
      Destroy(targetActor.gameObject);
    }

    GameObject newObj = SpawnEnemyActor(newActorPrefabName);
    switch(actorName) {
      case "main":
        mainActor = newObj.GetComponent<Actor2D>();
        break;
      case "support":
        supportActor = newObj.GetComponent<Actor2D>();
        break;
      case "angel":
        angelActor = newObj.GetComponent<Actor2D>();
        break;
      case "enemy":
        enemyActor = newObj.GetComponent<Actor2D>();
        break;
    }
  }

  public void DestroyEnemyActor() {
    Destroy(enemyActor.gameObject);
  }


  public Actor2D GetActorByIdInParty(int actorId) {
    switch(actorId) {
      case 0:
        return mainActor;
      case 1:
        return supportActor;
      case 2:
        return angelActor;
      case 3:
        return enemyActor;
    }
    return null;
  }

  public Actor2D GetActorByBattlingCharacter(Battler character) {
    if(mainActor.battler == character) {
      return mainActor;
    }
    if(supportActor.battler == character) {
      return supportActor;
    }
    if(angelActor.battler == character) {
      return angelActor;
    }
    if(enemyActor.battler == character) {
      return enemyActor;
    }
    return null;
  }


  public void SetupGirlInteraction() {
    Vector3 distance = new Vector3(3f, 0f, 0f);

    ClearBattle();

    mainActor.transform.localPosition = supportPosition - distance;
    supportActor.transform.localPosition = mainPosition;
    angelActor.transform.localPosition = angelPosition - distance;

    if(GlobalData.instance.CurrentBoy() != null) {
      mainActor.SetCharacter(GlobalData.instance.CurrentBoy());
      mainActor.SetClothing("uniform");
      mainActor.FaceRight();
    }
    if(GlobalData.instance.CurrentGirl() != null) {
      supportActor.SetCharacter(GlobalData.instance.CurrentGirl());
      supportActor.SetClothing("uniform");
      supportActor.FaceLeft();
      supportActor.gameObject.SetActive(true);
    } else {
      supportActor.gameObject.SetActive(false);
    }

    angelActor.SetCharacter(GlobalData.instance.people[4]);
    angelActor.FaceRight();
  }

  public void SetupDate() {
    Vector3 distance = new Vector3(3f, 0f, 0f);

    ClearBattle();

    mainActor.SetCharacter(GlobalData.instance.observedPeople[0]);
    supportActor.SetCharacter(GlobalData.instance.observedPeople[1]);
    angelActor.SetCharacter(GlobalData.instance.people[4]);

    mainActor.transform.localPosition = mainPosition - distance;
    supportActor.transform.localPosition = supportPosition - distance;
    angelActor.transform.localPosition = angelPosition - distance;

    mainActor.gameObject.SetActive(true);
    supportActor.gameObject.SetActive(true);

    mainActor.FaceRight();
    supportActor.FaceRight();
    angelActor.FaceRight();

    BattleController.instance.SetupDateLocation();
  }

  public void ClearBattle() {
    mainActor.SetBattleMode(false);
    supportActor.SetBattleMode(false);
    angelActor.SetBattleMode(false);

    if(enemyActor != null) {
      DestroyEnemyActor();
    }
  }

  public void PartyEntersScene() {
    mainActor.transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true).SetEase(Ease.Linear);
    supportActor.transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true).SetEase(Ease.Linear);
    angelActor.transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true).SetEase(Ease.Linear);
  }

  public void MainActorEntersScene() {
    mainActor.transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true);
    //supportActor.transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true).SetEase(Ease.Linear);
    //angelActor.transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true).SetEase(Ease.Linear);
  }

  public void EnemyEntersScene() {
    Enemy currentEnemy = BattleController.instance.GetCurrentEnemy();

    SpawnEnemyActor(currentEnemy.scriptName);

    currentEnemy.hp = currentEnemy.maxHp;
    currentEnemy.RemoveAllStatusConditions();

    enemyActor.gameObject.SetActive(true);
    enemyActor.transform.localPosition = challengePosition + new Vector3(2.5f, 0f, 0f);
    enemyActor.transform.DOLocalMoveX(challengePosition.x, 0.5f).OnComplete(() => {
      VsnAudioManager.instance.PlaySfx(currentEnemy.appearSfxName);
      InitializeChallengeLevelAndHp();
      PartyEntersBattleMode();
    });
  }

  public void ApplyBgEffect(BgEffect type, int intensity) {
    if(bgEffect != null) {
      Destroy(bgEffect);
    }

    if(intensity > 0) {
      bgEffect = Instantiate(bgEffectPrefabs[(int)type], gameObject.transform);
      bgEffect.GetComponent<Animator>().SetInteger("Intensity", intensity);
    }
  }

  public void SetCharacterChoosingAction(int characterId) {
    for(int i = 0; i < 3; i++) {
      GetActorByIdInParty(i).SetChooseActionMode(false);
    }
    if(characterId >= 0 && characterId <= 2) {
      GetActorByIdInParty(characterId).SetChooseActionMode(true);
    }
  }


  public void FocusActors(Actor2D[] actorsToFocus) {
    if(actorsToFocus[0] == actorsToFocus[1]) {
      FocusActor(actorsToFocus[0]);
      return;
    }
    focusedCharacters = actorsToFocus;

    for(int i = 0; i < 2; i++) {
      actorsToFocus[i].SetFocusedSortingLayer(true);
    }
    PositionActorsInFocus();
    focusShade.DOFade(0.5f, focusAnimationDuration);
  }

  public void FocusActor(Actor2D actorToFocus) {
    focusedCharacters = new Actor2D[] { actorToFocus };
    actorToFocus.SetFocusedSortingLayer(true);
    PositionActorsInFocus();
    focusShade.DOFade(0.5f, focusAnimationDuration);
  }

  public void PositionActorsInFocus() {
    if(focusedCharacters[0].transform.position.x > focusedCharacters[1].transform.position.x) {
      Actor2D aux = focusedCharacters[0];
      focusedCharacters[0] = focusedCharacters[1];
      focusedCharacters[1] = aux;
    }

    if(focusedCharacters.Length == 2) {
      focusedCharacters[0].transform.DOLocalMove(focusPositionForTwo[0], focusAnimationDuration);
      focusedCharacters[1].transform.DOLocalMove(focusPositionForTwo[1], focusAnimationDuration);
      focusedCharacters[0].SetAttackMode(true);
      focusedCharacters[1].SetAttackMode(true);
    } else {
      focusedCharacters[0].transform.DOLocalMove(focusPositionForOne, focusAnimationDuration);
      focusedCharacters[0].SetAttackMode(true);
    }
  }

  public void UnfocusActors() {
    focusShade.DOFade(0f, focusAnimationDuration).OnComplete(() => {
      for(int i = 0; i < focusedCharacters.Length; i++) {
        focusedCharacters[i].SetFocusedSortingLayer(false);
        focusedCharacters[i].SetAttackMode(false);
      }
    });
    PositionActorsBack();
  }

  public void PositionActorsBack() {
    mainActor.transform.DOLocalMove(mainPosition, focusAnimationDuration);
    supportActor.transform.DOLocalMove(supportPosition, focusAnimationDuration);
    angelActor.transform.DOLocalMove(angelPosition, focusAnimationDuration);
    enemyActor.transform.DOLocalMove(challengePosition, focusAnimationDuration);
  }




  public void SetActorAnimatorParameter(string actorName, string parameterName, bool value) {
    Actor2D actor = null;

    switch(actorName) {
      case "main":
        actor = mainActor;
        break;
      case "support":
        actor = supportActor;
        break;
      case "angel":
        actor = angelActor;
        break;
    }
    actor.SetParameter(parameterName, value);
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
    UIController.instance.enemyHpSlider.maxValue = BattleController.instance.GetCurrentEnemy().maxHp;
    UIController.instance.enemyHpSlider.value = UIController.instance.enemyHpSlider.maxValue;
    UIController.instance.enemyHpSlider.gameObject.SetActive(true);
  }

  public void EnemyLeavesScene() {
    UIController.instance.enemyHpSlider.gameObject.SetActive(false);  

    enemyActor.ShowWeaknessCard(false);

    enemyActor.transform.DOLocalMoveX(2.5f, 0.5f).SetRelative().OnComplete(() => {
      //enemyActor.gameObject.SetActive(false);
      PartyLeavesBattleMode();
      DestroyEnemyActor();
    });
  }

  public void HumansDetectAngel() {
    StartCoroutine(DetectAngelAnimation());
  }

  public IEnumerator DetectAngelAnimation() {
    yield return new WaitForSeconds(0.5f);

    mainActor.FaceLeft();
    supportActor.FaceLeft();
    yield return new WaitForSeconds(0.5f);

    mainActor.DetectAnimation();
    supportActor.DetectAnimation();
    yield return new WaitForSeconds(0.5f);

    //VsnAudioManager.instance.PlaySfx("ui_detect_fertiliel");
    //angelActor.DetectAnimation();
    //yield return new WaitForSeconds(0.5f);
    
    StatusCondition status = BattleController.instance.GetStatusConditionByName("spotted");
    status.duration = -1;
    angelActor.battler.ReceiveStatusCondition(status);
    yield return new WaitForSeconds(1f);

    angelActor.SetChooseActionMode(false);
    angelActor.SetAttackMode(false);

    yield return new WaitForSeconds(1f);
    mainActor.FaceRight();
    supportActor.FaceRight();

    yield return new WaitForSeconds(0.5f);
  }


  public void SetLocation(string place) {
    bgRenderer.sprite = Resources.Load<Sprite>("Bg/" + place);
  }

  public void MoveCamera(Vector3 newPosition, float time) {
    mainCamera.transform.DOMove(newPosition, time);
  }

  public void Screenshake() {
    DOTween.Kill(transform);
    transform.DOShakeRotation(0.3f, 1f).OnComplete(() => {
      transform.position = Vector3.zero;
    });
  }
}
