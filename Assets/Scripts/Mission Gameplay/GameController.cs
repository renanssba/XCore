using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Combat {
  public List<CharacterToken> characters;
  public List<GameObject> engageIcons;

  public const int maxCombatPartySize = 3;


  public Combat(CharacterToken a, CharacterToken b) {
    characters = new List<CharacterToken>();
    engageIcons = new List<GameObject>();
    
    AddConflict(a, b);
  }

  public void AddConflict(CharacterToken a, CharacterToken b) {
    if(!characters.Contains(a)) {
      characters.Add(a);
    }
    if(!characters.Contains(b)) {
      characters.Add(b);
    }

    Vector3 position = (a.transform.position + b.transform.position) / 2f;
    GameObject newObj = MonoBehaviour.Instantiate(GameController.instance.engageIconPrefab,
      position, Quaternion.identity, GameController.instance.transform);
    engageIcons.Add(newObj);

    characters = OrderByAgility();
  }

  public bool Contains(CharacterToken character) {
    return characters.Contains(character);
  }

  public int CountCharactersByTeam(CombatTeam team) {
    int count = 0;
    foreach(CharacterToken ct in characters) {
      if(ct.combatTeam == team) {
        count++;
      }
    }
    return count;
  }


  public List<CharacterToken> OrderByAgility() {
    return characters.OrderByDescending(q => q.battler.GetAttributeValue(Attributes.agility)).ToList();
  }

  public CharacterToken RandomCharacterInTeam(CombatTeam team) {
    List<CharacterToken> chars = new List<CharacterToken>();
    foreach(CharacterToken c in characters) {
      if(c.combatTeam == team) {
        chars.Add(c);
      }      
    }
    if(chars.Count == 0) {
      return null;
    }
    return chars[Random.Range(0, chars.Count)];
  }


  public Vector3 CenterPosition() {
    Vector3 pos = Vector3.zero;
    foreach(CharacterToken c in characters) {
      pos += c.transform.position;
    }
    return pos / (float)characters.Count;
  }

  public void DestroyIcons() {
    foreach(GameObject icon in engageIcons) {
      MonoBehaviour.Destroy(icon);
    }
    engageIcons.Clear();
  }


  public static bool CharacterIsEngagedInCombat(CharacterToken character) {
    foreach(Combat c in GameController.instance.activeCombats) {
      if(c.characters.Contains(character)) {
        return true;
      }
    }
    return false; 
  }

}


public enum GamePhase {
  playerPhase,
  enemyPhase,
  battlePhase
}

public enum GameState {
  setupPhase,
  chooseCharacter,
  actionsMenu,
  chooseMovement,
  chooseEngagement,
  confirmEngagement,
  noInput
}

public enum InputOrigin {
  IA = -1,
  keyboardAndMouse,
  joystick_1,
  joystick_2,
  joystick_3,
  joystick_4,
  none
}


public class GameController : MonoBehaviour {
  public static GameController instance;

  [Header("- Game State -")]
  public GamePhase gamePhase = GamePhase.playerPhase;
  public GameState gameState = GameState.noInput;


  [Header("- Characters -")]
  public List<CharacterToken> allCharacters;
  public int currentCharacterPos;


  [Header("- Combats -")]
  public List<Combat> activeCombats;
  public GameObject engageIconPrefab;

  public Combat currentCombat;


  public CharacterToken CurrentCharacter {
    get {
      if(currentCharacterPos >= allCharacters.Count ||
        currentCharacterPos < 0) {
        return null;
      }
      return allCharacters[currentCharacterPos];
    }
  }

  public InputOrigin InputOrigin {
    get {
      //if(gameState == GameState.setupPhase) return (InputOrigin)VsnSaveSystem.GetIntVariable("player1_input", 0);
      //return InputOrigin.IA; /// DEBUG for testing IA vs IA
      //if(CurrentCharacter == null) return InputOrigin.none;

      if(gamePhase == GamePhase.enemyPhase) return InputOrigin.IA;
      if(gamePhase == GamePhase.playerPhase) return (InputOrigin)VsnSaveSystem.GetIntVariable("player1_input", 0);
      return InputOrigin.none;
    }
  }



  public void Awake() {
    instance = this;

    allCharacters = new List<CharacterToken>();
    activeCombats = new List<Combat>();
  }

  public void Start() {
    //stageDesignController.LoadStageDesign();
    SelectNoCharacter();

    // Play Tactical Song
    //VsnAudioManager.instance.PlayMusic("Valkyrie Arena OST 1 INTRO", "Valkyrie Arena OST 1 LOOP");
    VsnController.instance.StartVSN("start battle");
    StartCoroutine(WaitSetupToStart());
  }


  public IEnumerator WaitSetupToStart() {
    yield return WaitForVsnExecution();
    yield return StartGamePhase(GamePhase.playerPhase);
  }


  void Update() {
    if(Input.GetKeyDown(KeyCode.F5)) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// If current input is IA, dont take input from player
    if(InputOrigin == InputOrigin.IA) {
      return;
    }

    /// Clicked Cancel button
    if(Input.GetMouseButtonDown(1)) {
      ClickedCancelButton();
    }
  }


  public IEnumerator StartGamePhase(GamePhase newPhase) {
    InitializeGamePhaseForCharacters();
    gamePhase = newPhase;
    gameState = GameState.noInput;

    switch(newPhase) {
      case GamePhase.playerPhase:
        VsnController.instance.StartVSN("player_phase");
        yield return WaitForVsnExecution();
        SetGameState(GameState.chooseCharacter);
        break;
      case GamePhase.enemyPhase:
        VsnController.instance.StartVSN("enemy_phase");
        yield return WaitForVsnExecution();
        SetGameState(GameState.chooseCharacter);
        break;
      case GamePhase.battlePhase:
        yield return new WaitForSeconds(1f);
        VsnController.instance.StartVSN("battle_phase");
        yield return WaitForVsnExecution();
        StartCoroutine(BattlePhase());
        break;
    }
  }


  public void SetGameState(GameState newState) {
    CleanHighlightedTiles();

    gameState = GameState.noInput;
    StartCoroutine(UpdateGameState(newState));
  }

  public IEnumerator UpdateGameState(GameState newState) {
    yield return null;
    gameState = newState;

    //Debug.LogWarning("SET GAMESTATE TO " + newState + "!!!");

    CleanHighlightedTiles();
    //if(CurrentCharacter == null) {
    //  yield break;
    //}
    switch(gameState) {
      case GameState.setupPhase:
        /// TODO: Implement Setup Phase
        break;
      case GameState.chooseCharacter:
        SelectNoCharacter();
        TacticalUIController.instance.ShowCurrentCharacterInfo(null);
        TacticalUIController.instance.HideActionsMenu();
        TacticalUIController.instance.HideSkillConfirmPanel();
        /// check if should advance phase
        if((gamePhase == GamePhase.playerPhase && !TeamCanStillAct(CombatTeam.player)) ||
           (gamePhase == GamePhase.enemyPhase && !TeamCanStillAct(CombatTeam.pc))) {
          AdvancePhase();
        }
        break;
      case GameState.actionsMenu:
        TacticalUIController.instance.ShowCurrentCharacterInfo(CurrentCharacter.battler);
        if(InputOrigin != InputOrigin.IA) {
          TacticalUIController.instance.ShowActionsMenu();
        } else {
          TacticalUIController.instance.HideActionsMenu();
        }
        TacticalUIController.instance.HideSkillConfirmPanel();
        break;
      case GameState.chooseMovement:
        TacticalUIController.instance.ShowCurrentCharacterInfo(CurrentCharacter.battler);
        TacticalUIController.instance.HideActionsMenu();
        TacticalUIController.instance.HideSkillConfirmPanel();
        BoardController.instance.HighlightWalkableTiles(CurrentCharacter);
        break;
      case GameState.chooseEngagement:
        TacticalUIController.instance.HideActionsMenu();
        TacticalUIController.instance.HideSkillConfirmPanel();
        BoardController.instance.HighlightAdjacentEnemies(CurrentCharacter);
        break;
      case GameState.confirmEngagement:
        //TacticalUIController.instance.ShowSkillConfirmPanel();
        //TacticalBoardController.instance.HighlightSkillAreaOfEffect(CurrentCharacter, CurrentSkill, skillCastPosition);
        break;
      case GameState.noInput:
        TacticalUIController.instance.HideActionsMenu();
        TacticalUIController.instance.HideSkillConfirmPanel();
        break;
      default:
        // do nothing
        break;
    }

    bool needsCancelButton = false;
    if((gameState == GameState.chooseEngagement ||
        gameState == GameState.confirmEngagement ||
        gameState == GameState.chooseMovement) &&
        InputOrigin != InputOrigin.IA) {
      needsCancelButton = true;
    }
    TacticalUIController.instance.cancelButton.SetActive(needsCancelButton);
  }


  public void ClickedMap() {
    Vector2Int clickedGridPos = MouseInput.instance.SelectedGridPosition();

    if(gameState == GameState.noInput ||
       Input.GetMouseButtonDown(0) == false) {
      return;
    }

    if((gameState == GameState.chooseMovement || gameState == GameState.chooseEngagement) &&
       !HighlightedTilesLayer.instance.IsTileHighlighted(clickedGridPos)) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    switch(gameState) {
      case GameState.chooseCharacter:
        CharacterToken ct = BoardController.instance.CharacterInPosition(clickedGridPos);
        if(ct != null && ct.combatTeam == CombatTeam.player) {
          InitializeCharacterTurn(GetCharacterPos(ct));
        } else {
          SfxManager.StaticPlayForbbidenSfx();
          return;
        }
        break;
      case GameState.chooseMovement:
        // clicked during choose movement phase
        StartMovement(clickedGridPos);
        break;
      case GameState.chooseEngagement:
        // clicked during choose engagement phase
        StartEngagement(clickedGridPos);
        break;
    }
  }

  public void ClickedCancelButton() {
    if(gameState == GameState.noInput ||
      gameState == GameState.chooseCharacter) {
      return;
    }

    SfxManager.StaticPlayCancelSfx();
    switch(gameState) {
      case GameState.chooseMovement:
        SetGameState(GameState.chooseCharacter);
        break;
      case GameState.actionsMenu:
        RevertMovement();
        SetGameState(GameState.chooseMovement);
        break;
      case GameState.chooseEngagement:
        SetGameState(GameState.actionsMenu);
        break;
      case GameState.confirmEngagement:
        SetGameState(GameState.chooseEngagement);
        break;
    }
  }



  public void CleanHighlightedTiles() {
    HighlightedTilesLayer.instance.StopHighlightWalkableTiles();
  }


  public void AdvancePhase() {
    switch(gamePhase) {
      case GamePhase.playerPhase:
        StartCoroutine(StartGamePhase(GamePhase.enemyPhase));
        break;
      case GamePhase.enemyPhase:
        StartCoroutine(StartGamePhase(GamePhase.battlePhase));
        break;
      case GamePhase.battlePhase:
        StartCoroutine(StartGamePhase(GamePhase.playerPhase));
        break;
    }
  }


  public void InitializeCharacterTurn(int newTurn) {
    Debug.LogWarning("New character selected: " + allCharacters[newTurn].battler.nameKey);

    if(allCharacters[newTurn].usedTurn) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    currentCharacterPos = newTurn;
    allCharacters[currentCharacterPos].BecomeCurrentCharacter(true);

    SetGameState(GameState.chooseMovement);

    //if(CurrentCharacter == null) return;
    //MouseInput.instance.SelectGridPosition(CurrentCharacter.BoardGridPosition());
    //CameraController.instance.FocusOnCharacterThenChangeGamestate(CurrentCharacter, GameState.chooseMovement);
  }

  public void EndTurn() {
    TacticalUIController.instance.HideActionsMenu();
    TacticalUIController.instance.ShowCurrentCharacterInfo(null);
    CleanHighlightedTiles();

    if(CurrentCharacter != null) {
      CurrentCharacter.EndTurn();
    }
    SetGameState(GameState.chooseCharacter);
  }


  public void SelectNoCharacter() {
    if(CurrentCharacter != null) {
      CurrentCharacter.BecomeCurrentCharacter(false);
    }
    currentCharacterPos = -1;
  }

  public void InitializeGamePhaseForCharacters() {
    foreach(CharacterToken ct in allCharacters) {
      ct.StartNewGamePhase();
    }
  }


  public int GetCharacterPos(CharacterToken ct) {
    for(int i = 0; i < allCharacters.Count; i++) {
      if(allCharacters[i] == ct) {
        return i;
      }
    }
    return -1;
  }

  public bool TeamCanStillAct(CombatTeam team) {
    foreach(CharacterToken ct in allCharacters) {
      if(ct.combatTeam == team && !ct.usedTurn) {
        return true;
      }
    }
    return false;
  }


  public int FirstEnemy() {
    for(int i=0; i<allCharacters.Count; i++) {
      if(allCharacters[i].combatTeam == CombatTeam.pc) {
        return i;
      }
    }
    return -1;
  }



  public IEnumerator WaitForVsnExecution() {
    while(VsnController.instance.state != ExecutionState.STOPPED) {
      yield return null;
    }
  }

  public IEnumerator BattlePhase() {
    foreach(Combat c in activeCombats) {
      yield return FightBattle(c);
    }
    activeCombats.Clear();

    yield return CameraController.instance.GoToDefaultPosition();
    yield return new WaitForSeconds(0.3f);

    /// RESTART PLAYER PHASE
    if(!CheckIfMatchIsOver()) {
      AdvancePhase();
    }
  }

  public IEnumerator FightBattle(Combat combat) {
    yield return CameraController.instance.FocusOnCombat(combat);

    currentCombat = combat;
    VsnController.instance.StartVSN("battle");
    yield return new WaitForSeconds(0.5f);

    yield return WaitForVsnExecution();
    combat.DestroyIcons();

    bool someoneDies = false;
    foreach(CharacterToken ct in combat.characters) {
      ct.UpdateUI();
      bool died = ct.CheckToDie();
      someoneDies = someoneDies || died;
    }
    if(someoneDies) {
      yield return new WaitForSeconds(0.5f);
    }
  }


  public void StartMovement(Vector2Int clickedGridPos) {
    Vector3 pos = BoardController.instance.floorBoard.layoutGrid.CellToWorld(clickedGridPos);
    float dur = 1f;
    if(clickedGridPos == CurrentCharacter.BoardGridPosition()) {
      dur = 0f;
    }

    CleanHighlightedTiles();
    SetGameState(GameState.noInput);
    //CurrentCharacter.BecomeCurrentCharacter(false);

    CurrentCharacter.transform.DOMove(new Vector3(pos.x, pos.y, 0f), dur).OnComplete(() => {
      SetGameState(GameState.actionsMenu);
      CurrentCharacter.RegisterMovement();
    });
  }


  public bool RevertMovement() {
    if(!CurrentCharacter.canRevertMovement) {
      SfxManager.StaticPlayForbbidenSfx();
      return false;
    }
    CurrentCharacter.RevertMovement();
    //cameraController.FocusOnCharacterImmediate(CurrentCharacter);
    return true;
  }

  

  public void StartEngagement(Vector2Int clickedGridPos) {
    CharacterToken clicked = BoardController.instance.CharacterInPosition(clickedGridPos);
    bool addedToEngagement = false;

    foreach(Combat c in activeCombats) {
      if(c.Contains(clicked)) {
        c.AddConflict(clicked, CurrentCharacter);
        addedToEngagement = true;
        break;
      }
    }
    if(!addedToEngagement) {
      activeCombats.Add(new Combat(clicked, CurrentCharacter));
    }
    EndTurn();
  }

  public int CountCharactersEngagedWithThis(CharacterToken character) {
    foreach(Combat combat in activeCombats) {
      if(combat.Contains(character)) {
        return combat.CountCharactersByTeam(character.OpponentCombatTeam());
      }
    }
    return 0;
  }


  public void RegisterCharacter(CharacterToken newCharacter) {
    allCharacters.Add(newCharacter);
    allCharacters = allCharacters.OrderBy(o => o.Id).ToList();
  }

  public void CharacterDies(CharacterToken dyingCharacter) {
    int indexInList = allCharacters.IndexOf(dyingCharacter);
    allCharacters.Remove(dyingCharacter);
  }



  public bool CheckIfMatchIsOver() {
    if(gameState == GameState.setupPhase) {
      return false;
    }

    if(VictoryConditionMet()) {
      VsnController.instance.StartVSN("victory");
      gameState = GameState.noInput;
      return true;
    }
    if(DefeatConditionMet()) {
      VsnController.instance.StartVSN("defeat");
      gameState = GameState.noInput;
      return true;
    }
    return false;
  }

  public bool VictoryConditionMet() {
    foreach(CharacterToken c in allCharacters) {
      if(c != null && c.combatTeam == CombatTeam.pc) {
        return false;
      }
    }
    return true;
  }

  public bool DefeatConditionMet() {
    foreach(CharacterToken c in allCharacters) {
      if(c != null && c.combatTeam == CombatTeam.player) {
        return false;
      }
    }
    return true;
  }


  public void EndBattle() {
    //SetGameState(GameState.chooseMovement);
  }
}
