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
  flamingEffect,
  distortingEffect,
  count
}


public class TheaterController : MonoBehaviour {

  public static TheaterController instance;

  public Camera mainCamera;

  [Header("- Actor Positions -")]
  public Vector3 firstPosition;
  public Vector3 secondPosition;
  public Vector3 thirdPosition;
  public Vector3 outPositionLeft;

  public Vector3 enemyPosition1 {
    get { return new Vector3(-firstPosition.x, firstPosition.y, firstPosition.z); }
  }
  public Vector3 enemyPosition2 {
    get { return new Vector3(-secondPosition.x, secondPosition.y, secondPosition.z); }
  }
  public Vector3 enemyPosition3 {
    get { return new Vector3(-thirdPosition.x, thirdPosition.y, thirdPosition.z); }
  }


  [Header("- Focus Animation -")]
  public Vector3[] focusPositionForTwo;
  public Vector3 focusPositionForOne;
  public SpriteRenderer focusShade;
  public Actor2D[] focusedCharacters;

  [Header("- Actors -")]
  public Actor2D[] partyActors;
  public Actor2D[] enemyActors;

  public GameObject personPrefab;

  [Header("- Background -")]
  public SpriteRenderer bgRenderer;
  public SpriteRenderer bgRendererFront;
  public GameObject bgEffect;

  public const float enterAnimationDuration = 1.5f;
  public float focusAnimationDuration = 0.2f;


  public void Awake() {
    instance = this;
  }


  public GameObject SpawnActor(string actorPrefabName) {
    GameObject prefabToSpawn = Resources.Load<GameObject>("Enemy Prefabs/" + actorPrefabName);
    GameObject spawnedActor;

    if(prefabToSpawn == null) {
      prefabToSpawn = BattleController.instance.defaultEnemyPrefab;
      if(actorPrefabName.StartsWith("person")) {
        prefabToSpawn = personPrefab;
      }

      Debug.LogWarning("No prefab to load for enemy: " + actorPrefabName + ". Using default prefab");
    }
    spawnedActor = Instantiate(prefabToSpawn, transform);
    return spawnedActor;
  }

  public Actor2D GetActorByString(string actorReference) {
    Actor2D actor = null;

    switch(actorReference) {
      case "hero_1":
        actor = partyActors[0];
        break;
      case "hero_2":
        actor = partyActors[1];
        break;
      case "hero_3":
        actor = partyActors[2];
        break;
      case "enemy_1":
        actor = enemyActors[0];
        break;
      case "enemy_2":
        actor = enemyActors[1];
        break;
      case "enemy_3":
        actor = enemyActors[2];
        break;
      default:
        break;
    }
    return actor;
  }

  
  public void ChangeActor(string actorReference, string newActorPrefabName) {
    Actor2D targetActor = GetActorByString(actorReference);
    GameObject newObj;
    Pilot person = null;
    int actorId = 0;

    if(actorReference.Contains("enemy")) {
      switch(actorReference) {
        case "enemy_1":
          actorId = 0;
          break;
        case "enemy_2":
          actorId = 1;
          break;
        case "enemy_3":
          actorId = 2;
          break;
          
      }
      if(targetActor != null) {
        Destroy(targetActor.gameObject);
      }
      newObj = SpawnActor(newActorPrefabName);
      enemyActors[actorId] = newObj.GetComponent<Actor2D>();
      enemyActors[actorId].SetCharacter(BattleController.instance.enemyMembers[actorId]);
      //enemyActors[actorId].battler = BattleController.instance.enemyMembers[actorId];
      //enemyActors[actorId].UpdateGraphics();
      return;
    }

    for(int i=0; i< GlobalData.instance.pilots.Count; i++) {
      if(GlobalData.instance.pilots[i].nameKey == Utils.GetStringArgument(newActorPrefabName)) {
        person = GlobalData.instance.pilots[i];
        break;
      }
    }
    
    switch(actorReference) {
      case "hero_1":
        actorId = 0;
        break;
      case "hero_2":
        actorId = 1;
        break;
      case "hero_3":
        actorId = 2;
        break;
      default:
        targetActor = null;
        newObj = SpawnActor(newActorPrefabName);
        break;
    }
    partyActors[actorId].SetCharacter(person);
    partyActors[actorId].FaceRight();
    partyActors[actorId].gameObject.SetActive(true);
  }

  public void DestroyEnemyActors() {
    for(int i=0; i<enemyActors.Length; i++) {
      if(enemyActors[i] != null) {
        Destroy(enemyActors[i].gameObject);
      }
    }
  }


  public Actor2D GetActorByPartyId(SkillTarget actorId) {
    switch(actorId) {
      case SkillTarget.partyMember1:
        return partyActors[0];
      case SkillTarget.partyMember2:
        return partyActors[1];
      case SkillTarget.partyMember3:
        return partyActors[2];
      case SkillTarget.enemy1:
        return enemyActors[0];
      case SkillTarget.enemy2:
        return enemyActors[1];
      case SkillTarget.enemy3:
        return enemyActors[2];
    }
    return null;
  }

  public Actor2D[] GetAllHeroesActors() {
    return partyActors;
  }

  public Actor2D[] GetAllEnemiesActors() {
    return enemyActors;
  }

  public Actor2D GetActorByBattler(Battler character) {
    for(int i=0; i<partyActors.Length; i++) {
      if(partyActors[i].battler == character) {
        return partyActors[i];
      }
    }

    for(int i = 0; i < enemyActors.Length; i++) {
      if(enemyActors[i].battler == character) {
        return enemyActors[i];
      }
    }
    return null;
  }



  public void SetupBattle() {
    ClearBattle();

    GameController.instance.StartBattle();
    BattleController.instance.SetupDateLocation();


    for(int i=0; i< 3; i++) {
      if(i < BattleController.instance.partyMembers.Length) {
        partyActors[i].SetCharacter(BattleController.instance.partyMembers[i]);
        partyActors[i].gameObject.SetActive(true);
        partyActors[i].FaceRight();
      } else {
        partyActors[i].gameObject.SetActive(false);
      }

      if(i < BattleController.instance.enemyMembers.Length) {
        Enemy currentEnemy = BattleController.instance.enemyMembers[i];
        ChangeActor("enemy_"+(i+1).ToString(), currentEnemy.spriteName);
        enemyActors[i].gameObject.SetActive(true);
      }
    }

    /// Position Heroes Party
    partyActors[0].transform.localPosition = firstPosition;
    partyActors[1].transform.localPosition = secondPosition;
    partyActors[2].transform.localPosition = thirdPosition;

    /// Position Enemies Party
    enemyActors[0].transform.localPosition = enemyPosition1;
    if(enemyActors[1] != null) {
      enemyActors[1].transform.localPosition = enemyPosition2;
    }
    if(enemyActors[2] != null) {
      enemyActors[2].transform.localPosition = enemyPosition3;
    }


    /// Clear Skill Uses
    foreach(Pilot partyMember in BattleController.instance.partyMembers) {
      partyMember.ClearSkillUsesInBattle();
    }
    foreach(Enemy enemyMember in BattleController.instance.enemyMembers) {
      enemyMember.ClearSkillUsesInBattle();
    }
  }


  public void ClearBattle() {
    for(int i = 0; i < partyActors.Length; i++) {
      partyActors[i].MoveToPosition(outPositionLeft, 0f);
    }
    DestroyEnemyActors();
  }


  public IEnumerator WaitThenDeleteBgEffect() {
    bgEffect.GetComponent<Animator>().SetInteger("Intensity", 0);
    yield return new WaitForSeconds(2f);
    Destroy(bgEffect);
  }

  public void SetCharacterChoosingAction(int characterId) {
    for(int i = 0; i < 3; i++) {
      GetActorByPartyId((SkillTarget)i).SetChooseActionMode(false);
    }
    if(characterId >= 0 && characterId <= 2) {
      GetActorByPartyId((SkillTarget)characterId).SetChooseActionMode(true);
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
    if(focusedCharacters.Length > 1 && focusedCharacters[0].transform.position.x > focusedCharacters[1].transform.position.x) {
      Actor2D aux = focusedCharacters[0];
      focusedCharacters[0] = focusedCharacters[1];
      focusedCharacters[1] = aux;
    }

    if(focusedCharacters.Length == 2) {
      for(int i=0; i<2; i++) {
        float heightDifference = 1f - focusedCharacters[i].transform.localScale.y;
        focusedCharacters[i].transform.DOLocalMove(focusPositionForTwo[i] + new Vector3(0f, heightDifference*1.25f, 0f), focusAnimationDuration);
      }
    } else {
      focusedCharacters[0].transform.DOLocalMove(focusPositionForOne, focusAnimationDuration);
    }
  }

  public void UnfocusActors() {
    focusShade.DOFade(0f, focusAnimationDuration).OnComplete(() => {
      for(int i = 0; i < focusedCharacters.Length; i++) {
        focusedCharacters[i].SetFocusedSortingLayer(false);
      }
    });
    PositionActorsBack();
  }

  public void PositionActorsBack() {
    foreach(Actor2D actorToPositionBack in focusedCharacters) {
      if(actorToPositionBack == partyActors[0]) {
        partyActors[0].transform.DOLocalMove(firstPosition, focusAnimationDuration);
        continue;
      }
      if(actorToPositionBack == partyActors[1]) {
        partyActors[1].transform.DOLocalMove(secondPosition, focusAnimationDuration);
        continue;
      }
      if(actorToPositionBack == partyActors[2]) {
        partyActors[2].transform.DOLocalMove(thirdPosition, focusAnimationDuration);
        continue;
      }

      if(actorToPositionBack == enemyActors[0]) {
        enemyActors[0].transform.DOLocalMove(enemyPosition1, focusAnimationDuration);
        continue;
      }
      if(actorToPositionBack == enemyActors[1]) {
        enemyActors[1].transform.DOLocalMove(enemyPosition2, focusAnimationDuration);
        continue;
      }
      if(actorToPositionBack == enemyActors[2]) {
        enemyActors[2].transform.DOLocalMove(enemyPosition3, focusAnimationDuration);
        continue;
      }
    }
  }




  public void SetLocation(string place) {
    bgRenderer.sprite = Resources.Load<Sprite>("Bg/" + place);
    Sprite front = Resources.Load<Sprite>("Bg/" + place+"_front");
    bgRendererFront.sprite = front;
  }

  public void MoveCamera(Vector3 newPosition, float time) {
    mainCamera.transform.DOMove(newPosition, time);
  }

  public void Screenshake(float effectivity, float time = 0.3f) {
    DOTween.Kill(transform);
    transform.DOShakeRotation(time, effectivity).OnComplete(() => {
      transform.position = Vector3.zero;
    });
  }
}
