using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAController : MonoBehaviour {
  public static IAController instance;

  public Coroutine coroutine;

  [Header("- IA States -")]
  public List<IAState> possibleStates;
  public IAState chosenState;


  private CharacterToken CurrentCharacter {
    get { return GameController.instance.CurrentCharacter; }
  }

  public float Frametime {
    get { return 16f; }
  }


  public void Awake() {
    instance = this;
    possibleStates = new List<IAState>();
  }

  public void Update() {
    if(coroutine != null || GameController.instance.InputOrigin != InputOrigin.IA) {
      return;
    }

    switch(GameController.instance.gameState) {
      case GameState.chooseCharacter:
        Debug.LogWarning("Starting to decide Character");
        coroutine = StartCoroutine(DecideCharacter());
        break;
      case GameState.chooseMovement:
        coroutine = StartCoroutine(DecideMovement());
        break;
      case GameState.actionsMenu:
        coroutine = StartCoroutine(DecideAction());
        break;
      case GameState.chooseEngagement:
        coroutine = StartCoroutine(DecideEngagement());
        break;
    }
  }


  public IEnumerator DecideCharacter() {
    yield return new WaitForSeconds(0.01f);
    foreach(CharacterToken ct in GameController.instance.allCharacters) {
      Debug.LogWarning("Deciding Characters");

      if(ct.combatTeam == CombatTeam.pc && !ct.usedTurn) {
        GameController.instance.InitializeCharacterTurn(GameController.instance.GetCharacterPos(ct));
        yield return null;
        coroutine = null;
        yield break;
      }
    }
    yield return null;
    coroutine = null;
  }

  public IEnumerator DecideAction() {
    if(CanEngageNow()) {
      yield return new WaitForSeconds(0.5f);
      GameController.instance.SetGameState(GameState.chooseEngagement);
    } else if(CanMoveYet()) {
      yield return new WaitForSeconds(0.5f);
      GameController.instance.SetGameState(GameState.chooseMovement);
    } else {
      yield return new WaitForSeconds(0.5f);
      GameController.instance.EndTurn();
    }
    coroutine = null;
  }

  public IEnumerator DecideMovement() {
    List<Vector2Int> walkableTiles = BoardController.instance.CalculateWalkableTiles(CurrentCharacter);
    possibleStates = new List<IAState>();
    IAState newState;
    System.DateTime realStart = System.DateTime.Now;
    System.DateTime start = realStart;
    string s = "";

    Debug.LogWarning("Frametime: " + Frametime + ". Target framerate: " + Application.targetFrameRate);
    foreach(Vector2Int tileOption in walkableTiles) {
      newState = new IAState(tileOption);
      newState.CharacterMoves(CurrentCharacter, tileOption);
      newState.CalculateHeuristic();
      possibleStates.Add(newState);

      s += (System.DateTime.Now - start).Milliseconds + "\n";
      if((System.DateTime.Now - start).Milliseconds >= Frametime) {
        yield return null;
        start = System.DateTime.Now;
        s += "-wait frame-\n";
      }
    }
    float timePassed = 0.001f * (System.DateTime.Now - realStart).Milliseconds;
    float timeToPass = 0.5f - timePassed;
    Debug.LogWarning("TIME PASSED: " + s);
    Debug.LogWarning("TOTAL TIME PASSED DECIDING MOVEMENT: " + timePassed);

    chosenState = possibleStates.FindBestState();
    if(timeToPass > 0f) {
      yield return new WaitForSeconds(timeToPass);
    }

    SfxManager.StaticPlayConfirmSfx();
    GameController.instance.StartMovement(chosenState.tileInput);
    coroutine = null;
  }

  public IEnumerator DecideEngagement() {
    List<Vector2Int> engageInputs = BoardController.instance.CalculateEngagementTargets(CurrentCharacter);
    possibleStates = new List<IAState>();
    IAState newState;
    System.DateTime realStart = System.DateTime.Now;
    System.DateTime start = realStart;

    //Debug.LogWarning("Frametime: " + Frametime + ". Target framerate: " + Application.targetFrameRate);
    foreach(Vector2Int tileOption in engageInputs) {
      newState = new IAState(tileOption);
      newState.EngageWith(BoardController.instance.CharacterInPosition(tileOption));
      newState.CalculateHeuristic();
      possibleStates.Add(newState);

      if((System.DateTime.Now - start).Milliseconds >= Frametime) {
        yield return null;
        start = System.DateTime.Now;
      }
    }
    float timePassed = 0.001f * (System.DateTime.Now - realStart).Milliseconds;
    float timeToPass = 0.5f - timePassed;
    //Debug.LogWarning("TOTAL TIME PASSED DECIDING MOVEMENT: " + timePassed);

    chosenState = possibleStates.FindBestState();
    if(timeToPass > 0f) {
      yield return new WaitForSeconds(timeToPass);
    }

    SfxManager.StaticPlayConfirmSfx();
    GameController.instance.StartEngagement(chosenState.tileInput);
    coroutine = null;
  }


  //public IEnumerator DecideAction() {
  //  List<Action> possibleActions = PossibleActions();
  //  possibleStates = new List<IAState>();
  //  IAState newState = null;
  //  System.DateTime realStart = System.DateTime.Now;
  //  System.DateTime start = realStart;
  //  string s = "";

  //  foreach(Action action in possibleActions) {
  //    if(action == Action.defend ||
  //       action == Action.wait) {
  //      newState = new IAState(action);
  //      newState.CalculateHeuristic();
  //      possibleStates.Add(newState);
  //    } else {
  //      SkillData skillUsed = SkillInActionSlot(action);
  //      List<Vector2Int> skillRange = BoardController.instance.CalculateSkillRange(
  //        CurrentCharacter,
  //        skillUsed);

  //      foreach(Vector2Int castPosition in skillRange) {
  //        List<Vector2Int> areaAffected = BoardController.instance.CalculateSkillAreaOfEffect(
  //          CurrentCharacter,
  //          skillUsed,
  //          castPosition);

  //        newState = new IAState(action, castPosition);
  //        newState.AreaAffectedBySkill(skillUsed, areaAffected);
  //        newState.CalculateHeuristic();
  //        possibleStates.Add(newState);

  //        s += (System.DateTime.Now - start).Milliseconds + "\n";
  //        if((System.DateTime.Now - start).Milliseconds >= Frametime) {
  //          yield return null;
  //          start = System.DateTime.Now;
  //          s += "-wait frame-\n";
  //        }
  //      }
  //    }

  //    s += (System.DateTime.Now - start).Milliseconds + "\n";
  //    if((System.DateTime.Now - start).Milliseconds >= Frametime) {
  //      yield return null;
  //      start = System.DateTime.Now;
  //      s += "-wait frame-\n";
  //    }
  //  }
  //  float timePassed = 0.001f * (System.DateTime.Now - realStart).Milliseconds;
  //  float timeToPass = 0.5f - timePassed;
  //  Debug.LogWarning("TIME PASSED: " + s);
  //  Debug.LogWarning("TOTAL TIME PASSED DECIDING ACTION: " + timePassed);

  //  chosenState = possibleStates.FindBestState();
  //  if(timeToPass > 0f) {
  //    yield return new WaitForSeconds(timeToPass);
  //  }

  //  /// execute chosen action
  //  SfxManager.StaticPlayConfirmSfx();
  //  ActionButton.ExecuteAction(chosenState.actionInput);
  //  coroutine = null;
  //}

  //public IEnumerator ContemplateSkillRange() {
  //  yield return new WaitForSeconds(0.5f);

  //  SfxManager.StaticPlayConfirmSfx();
  //  GameController.instance.skillCastPosition = chosenState.tileInput;
  //  GameController.instance.SetGameState(GameState.confirmSkillUse);
  //  coroutine = null;
  //}

  //public IEnumerator ConfirmSkillUse() {
  //  yield return new WaitForSeconds(0.5f);

  //  SfxManager.StaticPlayConfirmSfx();
  //  GameController.instance.ClickedConfirmSkillButton();
  //  coroutine = null;
  //}


  public bool CanEngageNow() {
    List<Vector2Int> targets = BoardController.instance.CalculateEngagementTargets(CurrentCharacter);
    return targets.Count > 0;
  }

  public bool CanMoveYet() {
    return CurrentCharacter.canMove && CurrentCharacter.battler.GetAttributeValue(Attributes.movementRange)>0;
  }
}
