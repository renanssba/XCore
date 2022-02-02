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

  public Vector3 mainPosition;
  public Vector3 supportPosition;
  public Vector3 angelPosition;
  public Vector3 enemyPosition;

  public Vector3 outPositionRight;
  public Vector3 outPositionLeft;

  public Vector3[] focusPositionForTwo;
  public Vector3 focusPositionForOne;
  public SpriteRenderer focusShade;
  public Actor2D[] focusedCharacters;

  public Actor2D[] partyActors;

  //public Actor2D mainActor;
  //public Actor2D supportActor;
  //public Actor2D angelActor;
  public Actor2D enemyActor;
  public List<Actor2D> extraActors;

  public GameObject personPrefab;
  public GameObject[] bgEffectPrefabs;
  public GameObject overlayPrefab;

  public SpriteRenderer bgRenderer;
  public SpriteRenderer bgRendererFront;
  public GameObject bgEffect;

  public const float enterAnimationDuration = 1.5f;
  public float focusAnimationDuration = 0.2f;


  public void Awake() {
    instance = this;
    extraActors = new List<Actor2D>();
  }


  public void ClearTheater() {
    for(int i = 0; i < partyActors.Length; i++) {
      partyActors[i].MoveToPosition(outPositionLeft, 0f);
    }
    for(int i = 0; i < extraActors.Count; i++) {
      Destroy(extraActors[i].gameObject);
    }
    extraActors = new List<Actor2D>();
    DestroyEnemyActor();
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
      spawnedActor = Instantiate(prefabToSpawn, enemyPosition, Quaternion.identity, transform);
      spawnedActor.GetComponent<Actor2D>().SetActorGraphics(actorPrefabName);
    } else {
      spawnedActor = Instantiate(prefabToSpawn, enemyPosition, Quaternion.identity, transform);
    }
    return spawnedActor;
  }

  public Actor2D GetActorByString(string actorReference) {
    Actor2D actor = null;

    switch(actorReference) {
      case "main":
        actor = partyActors[0];
        break;
      case "support":
        actor = partyActors[1];
        break;
      case "angel":
        actor = partyActors[2];
        break;
      case "enemy":
        actor = enemyActor;
        break;
      default:
        foreach(Actor2D aux in extraActors) {
          if(aux.actorReference == actorReference) {
            actor = aux;
            break;
          }
        }
        break;
    }
    return actor;
  }

  public Vector3 GetPositionByString(string positionName) {
    Vector3 position = Vector3.zero;

    switch(positionName.Replace("_back", "").Replace("_front", "")) {
      case "angel":
        position = angelPosition;
        break;
      case "out_left":
      case "grid_0":
        position = outPositionLeft;
        break;
      case "grid_1":
        position = supportPosition + new Vector3(-1f, 0f, 0f);
        break;
      case "support":
      case "grid_2":
        position = supportPosition;
        break;
      case "main":
      case "grid_3":
        position = mainPosition;
        break;
      case "enemy_0":
      case "grid_4":
        position = enemyPosition + new Vector3(-1f, 0f, 0f);
        break;
      case "enemy":
      case "enemy_1":
      case "grid_5":
        position = enemyPosition;
        break;
      case "enemy_2":
      case "grid_6":
        position = enemyPosition + new Vector3(1f, 0f, 0f);
        break;
      case "out_right":
      case "grid_7":
        position = outPositionRight;
        break;
    }
    if(positionName.Contains("_back")) {
      position = position + new Vector3(0f, 0f, 1f);
    }
    if(positionName.Contains("_front")) {
      position = position + new Vector3(0f, 0f, -1f);
    }

    return position;
  }

  public void ChangeActor(string actorReference, string newActorPrefabName) {
    Actor2D targetActor = GetActorByString(actorReference);
    GameObject newObj;

    if(actorReference == "enemy") {
      if(targetActor != null) {
        Destroy(targetActor.gameObject);
      }
      newObj = SpawnActor(newActorPrefabName);
      enemyActor = newObj.GetComponent<Actor2D>();
      enemyActor.enemy = BattleController.instance.GetCurrentEnemy();
      enemyActor.battler = BattleController.instance.GetCurrentEnemy();
      return;
    }

    Person person = null;
    for(int i=0; i< GlobalData.instance.people.Count; i++) {
      if(GlobalData.instance.people[i].nameKey == Utils.GetStringArgument(newActorPrefabName)) {
        person = GlobalData.instance.people[i];
        break;
      }
    }
    switch(actorReference) {
      case "main":
        partyActors[0].SetCharacter(person);
        partyActors[0].FaceRight();
        partyActors[0].gameObject.SetActive(true);
        break;
      case "support":
        partyActors[1].SetCharacter(person);
        partyActors[1].FaceRight();
        partyActors[1].gameObject.SetActive(true);
        break;
      case "angel":
        partyActors[2].SetCharacter(person);
        partyActors[2].FaceRight();
        partyActors[2].gameObject.SetActive(true);
        break;
      default:
        targetActor = null;
        foreach(Actor2D currentActor in extraActors) {
          if(currentActor.actorReference == actorReference) {
            targetActor = currentActor;
            extraActors.Remove(targetActor);
            Destroy(targetActor.gameObject);
            break;
          }
        }
        newObj = SpawnActor(newActorPrefabName);
        extraActors.Add(newObj.GetComponent<Actor2D>());
        newObj.GetComponent<Actor2D>().actorReference = actorReference;
        break;
    }
  }

  public void DestroyEnemyActor() {
    if(enemyActor != null) {
      Destroy(enemyActor.gameObject);
    }
  }


  public Actor2D GetActorByIdInParty(SkillTarget actorId) {
    switch(actorId) {
      case SkillTarget.partyMember1:
        return partyActors[0];
      case SkillTarget.partyMember2:
        return partyActors[1];
      case SkillTarget.partyMember3:
        return partyActors[2];
      case SkillTarget.enemy1:
        return enemyActor;
    }
    return null;
  }

  public Actor2D[] GetAllHeroesActors() {
    return partyActors;
    //return new Actor2D[] { mainActor, supportActor };
  }

  public Actor2D[] GetAllEnemiesActors() {
    return new Actor2D[] { enemyActor };
  }

  public Actor2D GetActorByBattlingCharacter(Battler character) {
    for(int i=0; i<partyActors.Length; i++) {
      if(partyActors[i].battler == character) {
        return partyActors[i];
      }
    }

    //if(mainActor.battler == character) {
    //  return mainActor;
    //}
    //if(supportActor.battler == character) {
    //  return supportActor;
    //}
    //if(angelActor.battler == character) {
    //  return angelActor;
    //}
    if(enemyActor != null && enemyActor.battler == character) {
      return enemyActor;
    }
    return null;
  }


  public void SetupGirlWaitingForBoy() {
    Vector3 distance = new Vector3(3f, 0f, 0f);

    ClearTheater();
    ClearBattle();

    partyActors[0].transform.localPosition = supportPosition - distance;
    partyActors[1].transform.localPosition = mainPosition;
    partyActors[2].transform.localPosition = angelPosition - distance;

    if(GlobalData.instance.CurrentBoy() != null) {
      partyActors[0].SetCharacter(GlobalData.instance.CurrentBoy());
      partyActors[0].FaceRight();
    }
    if(GlobalData.instance.CurrentGirl() != null) {
      partyActors[1].SetCharacter(GlobalData.instance.CurrentGirl());
      partyActors[1].FaceLeft();
      partyActors[1].gameObject.SetActive(true);
    } else {
      partyActors[1].gameObject.SetActive(false);
    }

    //angelActor.SetCharacter(GlobalData.instance.people[3]);
    partyActors[2].FaceRight();
  }

  public void SetupBoyWaitingForGirl() {
    Vector3 distance = new Vector3(3f, 0f, 0f);

    ClearTheater();
    ClearBattle();

    partyActors[0].transform.localPosition = mainPosition;
    partyActors[1].transform.localPosition = supportPosition - distance;
    partyActors[2].transform.localPosition = angelPosition - distance;

    if(GlobalData.instance.CurrentBoy() != null) {
      partyActors[0].SetCharacter(GlobalData.instance.CurrentBoy());
      partyActors[0].FaceLeft();
    }
    if(GlobalData.instance.CurrentGirl() != null) {
      partyActors[1].SetCharacter(GlobalData.instance.CurrentGirl());
      partyActors[1].FaceRight();
      partyActors[1].gameObject.SetActive(true);
    } else {
      partyActors[1].gameObject.SetActive(false);
    }

    //partyActors[2].SetCharacter(GlobalData.instance.people[3]);
    partyActors[2].FaceRight();
  }



  public void SetupDate() {
    Vector3 distance = new Vector3(3f, 0f, 0f);

    ClearTheater();
    ClearBattle();

    BattleController.instance.SetupDateLocation();


    for(int i=0; i< partyActors.Length; i++) {
      if(i < BattleController.instance.partyMembers.Length) {
        partyActors[i].SetCharacter(BattleController.instance.partyMembers[0]);
        partyActors[i].transform.localPosition = mainPosition - distance;
        partyActors[i].gameObject.SetActive(true);
        partyActors[i].FaceRight();
      } else {
        partyActors[i].gameObject.SetActive(false);
      }
    }

    //mainActor.SetCharacter(BattleController.instance.partyMembers[0]);
    //supportActor.SetCharacter(BattleController.instance.partyMembers[1]);
    //angelActor.SetCharacter(BattleController.instance.partyMembers[2]);

    //mainActor.transform.localPosition = mainPosition - distance;
    //supportActor.transform.localPosition = supportPosition - distance;
    //angelActor.transform.localPosition = angelPosition - distance;

    //mainActor.gameObject.SetActive(true);
    //supportActor.gameObject.SetActive(true);
    //angelActor.gameObject.SetActive(true);

    //mainActor.FaceRight();
    //supportActor.FaceRight();
    //angelActor.FaceRight();
  }

  public void ClearBattle() {
    for(int i = 0; i < partyActors.Length; i++) {
      partyActors[i].SetBattleMode(false);
    }
    //mainActor.SetBattleMode(false);
    //supportActor.SetBattleMode(false);
    //angelActor.SetBattleMode(false);

    DestroyEnemyActor();
  }

  public void PartyEntersScene() {
    for(int i = 0; i < partyActors.Length; i++) {
      partyActors[i].transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true).SetEase(Ease.Linear);
    }
  }

  public void MainActorEntersScene() {
    partyActors[0].transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true);
  }

  public void SupportActorEntersScene() {
    partyActors[1].transform.DOLocalMoveX(3f, enterAnimationDuration).SetRelative(true);
  }

  public void EnemyEntersScene() {
    Enemy currentEnemy = BattleController.instance.GetCurrentEnemy();
    ChangeActor("enemy", currentEnemy.spriteName);
    enemyActor.SetEnemy(currentEnemy);
    
    BattleController.instance.partyMembers[0].ClearSkillUsesInBattle();
    BattleController.instance.partyMembers[1].ClearSkillUsesInBattle();
    BattleController.instance.FullHealEnemies();

    enemyActor.gameObject.SetActive(true);
    enemyActor.transform.localPosition = enemyPosition + new Vector3(2.5f, 0f, 0f);
    enemyActor.transform.DOLocalMoveX(enemyPosition.x, 0.5f).OnComplete(() => {
      VsnAudioManager.instance.PlaySfx(currentEnemy.appearSfxName);
      InitializeEnemyLevelAndHp();
      PartyEntersBattleMode();
    });
  }

  public void ApplyBgEffect(BgEffect type, int intensity) {
    if(bgEffect != null && intensity <= 0) {
      StartCoroutine(WaitThenDeleteBgEffect());
    }

    if(intensity > 0) {
      if(bgEffect == null) {
        bgEffect = Instantiate(bgEffectPrefabs[(int)type], gameObject.transform);
      }
      bgEffect.GetComponent<Animator>().SetInteger("Intensity", intensity);
    }
  }

  public IEnumerator WaitThenDeleteBgEffect() {
    bgEffect.GetComponent<Animator>().SetInteger("Intensity", 0);
    yield return new WaitForSeconds(2f);
    Destroy(bgEffect);
  }

  public void SetCharacterChoosingAction(int characterId) {
    for(int i = 0; i < 3; i++) {
      GetActorByIdInParty((SkillTarget)i).SetChooseActionMode(false);
    }
    if(characterId >= 0 && characterId <= 2) {
      GetActorByIdInParty((SkillTarget)characterId).SetChooseActionMode(true);
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
        focusedCharacters[i].SetAttackMode(true);
      }
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
    foreach(Actor2D actorToPositionBack in focusedCharacters) {
      if(actorToPositionBack == partyActors[0]) {
        partyActors[0].transform.DOLocalMove(mainPosition, focusAnimationDuration);
        continue;
      }
      if(actorToPositionBack == partyActors[1]) {
        partyActors[1].transform.DOLocalMove(supportPosition, focusAnimationDuration);
        continue;
      }
      if(actorToPositionBack == partyActors[2]) {
        partyActors[2].transform.DOLocalMove(angelPosition, focusAnimationDuration);
        continue;
      }
      if(actorToPositionBack == enemyActor) {
        enemyActor.transform.DOLocalMove(enemyPosition, focusAnimationDuration);
        continue;
      }
    }
  }




  public void SetActorAnimatorParameter(string actorReference, string parameterName, bool value) {
    Actor2D actor = GetActorByString(actorReference);

    if(actor == null) {
      return;
    }
    actor.SetAnimationParameter(parameterName, value);
  }

  public void PartyEntersBattleMode() {
    for(int i = 0; i < partyActors.Length; i++) {
      partyActors[i].SetBattleMode(true);
    }
  }

  public void PartyLeavesBattleMode() {
    for(int i = 0; i < partyActors.Length; i++) {
      partyActors[i].SetBattleMode(false);
    }
  }


  public void InitializeEnemyLevelAndHp() {
    Debug.LogWarning("Called InitializeChallengeLevelAndHp");
    Enemy enemy = BattleController.instance.GetCurrentEnemy();
    UIController.instance.enemyHpSlider.SetMaxValue(enemy.maxHp);
    UIController.instance.enemyHpSlider.SetSliderValueWithoutAnimation(enemy.maxHp);
    UIController.instance.AnimateEnemyHpChange(enemy.maxHp, enemy.maxHp);
    UIController.instance.enemyLevelText.text = "<size=60>"+Lean.Localization.LeanLocalization.GetTranslationText("attribute/lvl") +" </size>" + enemy.level;
  }

  public void EnemyLeavesScene() {
    //UIController.instance.enemyHpSlider.gameObject.SetActive(false);  
    enemyActor.ShowWeaknessCard(false);

    enemyActor.transform.DOLocalMoveX(2.5f, 0.5f).SetRelative().OnComplete(() => {
      //enemyActor.gameObject.SetActive(false);
      PartyLeavesBattleMode();
      DestroyEnemyActor();
    });
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
