﻿using System.Collections;
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

  public Vector3[] focusPositionForTwo;
  public Vector3 focusPositionForOne;
  public SpriteRenderer focusShade;
  public Actor2D[] focusedCharacters;

  public Actor2D mainActor;
  public Actor2D supportActor;
  public Actor2D angelActor;
  public Actor2D enemyActor;

  public SpriteRenderer bgRenderer;

  public const float enterAnimationDuration = 1.5f;
  public float focusAnimationDuration = 0.2f;


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
      enemyActor.SetEnemy(dateEvent);
      enemyActor.SetEnemyGraphics();
    } else {
      spawnedEnemy = Instantiate(prefabToSpawn, challengePosition, Quaternion.identity, transform);
      enemyActor = spawnedEnemy.GetComponent<Actor2D>();
      enemyActor.SetEnemy(dateEvent);
    }
  }

  public void DestroyEnemyActor() {
    Destroy(enemyActor.gameObject);
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
      case 3:
        return enemyActor;
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


  public void SetupGirlInteraction() {
    Vector3 distance = new Vector3(3f, 0f, 0f);

    ClearBattle();

    mainActor.transform.localPosition = supportPosition -distance;
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


  public void FocusActors(Actor2D[] actorsToFocus) {
    if(actorsToFocus[0] == actorsToFocus[1]) {
      FocusActor(actorsToFocus[0]);
      return;
    }
    focusedCharacters = actorsToFocus;

    for(int i=0; i<2; i++) {
      actorsToFocus[i].SetFocusedSortingLayer(true);
    }
    focusShade.DOFade(0.5f, focusAnimationDuration);
    PositionActorsInFocus();
  }

  public void FocusActor(Actor2D actorToFocus) {
    focusedCharacters = new Actor2D[] { actorToFocus};
    PositionActorsInFocus();
  }

  public void PositionActorsInFocus() {
    if(focusedCharacters.Length == 2) {
      focusedCharacters[0].transform.DOLocalMove(focusPositionForTwo[0], focusAnimationDuration);
      focusedCharacters[1].transform.DOLocalMove(focusPositionForTwo[1], focusAnimationDuration);
    } else {
      focusedCharacters[0].transform.DOLocalMove(focusPositionForOne, focusAnimationDuration);
    }
  }

  public void UnfocusActors() {
    focusShade.DOFade(0f, focusAnimationDuration).OnComplete( ()=> {
      for(int i = 0; i < focusedCharacters.Length; i++) {
        focusedCharacters[i].SetFocusedSortingLayer(false);
      }
    } );
    PositionActorsBack();
  }

  public void PositionActorsBack() {
    mainActor.transform.DOLocalMove(mainPosition, focusAnimationDuration);
    supportActor.transform.DOLocalMove(supportPosition, focusAnimationDuration);
    angelActor.transform.DOLocalMove(angelPosition, focusAnimationDuration);
    enemyActor.transform.DOLocalMove(challengePosition, focusAnimationDuration);
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
    bgRenderer.sprite = Resources.Load<Sprite>("Bg/"+place);
  }

  public void MoveCamera(Vector3 newPosition, float time) {
    mainCamera.transform.DOMove(newPosition, time);
  }
}
