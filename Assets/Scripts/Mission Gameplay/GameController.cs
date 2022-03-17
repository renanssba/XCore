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


  public List<CharacterToken> OrderByAgility() {
    return characters.OrderByDescending(q => q.battler.AttributeValue(Attributes.agility)).ToList();
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


  public static bool CharacterIsEngagedInCombat(CharacterToken a) {
    foreach(Combat c in GameController.instance.activeCombats) {
      if(c.characters.Contains(a)) {
        return true;
      }
    }
    return false; 
  }
}

public enum GameState {
  setupPhase,
  chooseCharacter,
  actionsMenu,
  chooseMovement,
  chooseEngagement,
  confirmEngagement,
  battlePhase,
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
  public GameState gameState = GameState.noInput;


  [Header("- Characters -")]
  public List<CharacterToken> allCharacters;
  public int currentCharacterPos;


  [Header("- Combats -")]
  public List<Combat> activeCombats;
  public GameObject engageIconPrefab;

  public Combat currentCombat;


  [Header("- Camera -")]
  public CameraController cameraController;


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
      if(CurrentCharacter == null) return InputOrigin.none;
      if(CurrentCharacter.combatTeam == CombatTeam.pc) return InputOrigin.IA;
      if(CurrentCharacter.combatTeam == CombatTeam.player) return (InputOrigin)VsnSaveSystem.GetIntVariable("player1_input", 0);
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
    yield return null;
    AdvanceTurn();
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
      case GameState.actionsMenu:
        TacticalUIController.instance.ShowCurrentCharacterInfo(CurrentCharacter.battler);
        TacticalUIController.instance.ShowActionsMenu();
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
      case GameState.battlePhase:
        TacticalUIController.instance.ShowCurrentCharacterInfo(null);
        TacticalUIController.instance.HideActionsMenu();
        TacticalUIController.instance.HideSkillConfirmPanel();
        StartCoroutine(FightsPhase());
        break;
      case GameState.confirmEngagement:
        //TacticalUIController.instance.ShowSkillConfirmPanel();
        //TacticalBoardController.instance.HighlightSkillAreaOfEffect(CurrentCharacter, CurrentSkill, skillCastPosition);
        break;
      case GameState.noInput:
        TacticalUIController.instance.HideActionsMenu();
        break;
      default:
        // do nothing
        break;
    }

    bool needsCancelButton = false;
    if(gameState == GameState.chooseEngagement ||
       gameState == GameState.confirmEngagement ||
       gameState == GameState.chooseMovement) {
      needsCancelButton = true;
    }
    TacticalUIController.instance.cancelButton.SetActive(needsCancelButton);
  }


  public void ClickedMap() {
    Vector2Int clickedGridPos = MouseInput.instance.SelectedGridPosition();

    if(gameState == GameState.noInput || gameState == GameState.battlePhase ||
       Input.GetMouseButtonDown(0) == false) {
      return;
    }    

    if(!HighlightedTilesLayer.instance.IsTileHighlighted(clickedGridPos)) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    switch(gameState) {
      case GameState.chooseMovement:
        // clicked during choose movement phase
        StartMovement(clickedGridPos);
        break;
      case GameState.chooseEngagement:
        // clicked during choose engagement phase
        StartEngagement();
        break;
    }
  }

  public void ClickedCancelButton() {
    if(gameState == GameState.noInput ||
       gameState == GameState.actionsMenu) {
      return;
    }

    SfxManager.StaticPlayCancelSfx();
    switch(gameState) {
      case GameState.chooseMovement:
        SetGameState(GameState.actionsMenu);
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

  public void AdvanceTurn() {
    if(CurrentCharacter != null) {
      CurrentCharacter.EndTurn();
    }
    TacticalUIController.instance.HideActionsMenu();
    CleanHighlightedTiles();

    currentCharacterPos++;

    /// enter battler phase
    if(currentCharacterPos >= allCharacters.Count) {
      SelectNoCharacter();
      SetGameState(GameState.battlePhase);
      return;
    }

    /// show player and enemy phase title cards
    if(currentCharacterPos == 0) {
      StartCoroutine(StartPhaseAnim());
      return;
    } else if(currentCharacterPos == FirstEnemy()) {
      StartCoroutine(StartPhaseAnim());
      return;
    }

    InitializeCharacterTurn();
  }

  public void InitializeCharacterTurn() {
    if(CurrentCharacter != null) {
      CurrentCharacter.InitializeTurn();
    }
    //SetInputModuleId();
    UpdateCurrentCharacter();

    if(Combat.CharacterIsEngagedInCombat(CurrentCharacter)) {
      AdvanceTurn();
      return;
    }

    SetGameState(GameState.actionsMenu);

    //if(CurrentCharacter == null) return;
    //MouseInput.instance.SelectGridPosition(CurrentCharacter.BoardGridPosition());
    //CameraController.instance.FocusOnCharacterThenChangeGamestate(CurrentCharacter, GameState.chooseMovement);
  }

  public void SelectNoCharacter() {
    currentCharacterPos = -1;
    UpdateCurrentCharacter();
  }

  public void UpdateCurrentCharacter() {
    foreach(CharacterToken c in allCharacters) {
      c.BecomeCurrentCharacter(c == CurrentCharacter);
    }
  }


  public int FirstEnemy() {
    for(int i=0; i<allCharacters.Count; i++) {
      if(allCharacters[i].combatTeam == CombatTeam.pc) {
        return i;
      }
    }
    return -1;
  }



  public IEnumerator StartPhaseAnim() {
    gameState = GameState.noInput;

    if(currentCharacterPos == 0) {
      VsnController.instance.StartVSN("player_phase");
    } else {
      VsnController.instance.StartVSN("enemy_phase");
    }
    yield return WaitForVsnExecution();
    InitializeCharacterTurn();
  }

  public IEnumerator WaitForVsnExecution() {
    while(VsnController.instance.state != ExecutionState.STOPPED) {
      yield return null;
    }
  }

  public IEnumerator FightsPhase() {
    foreach(Combat c in activeCombats) {
      yield return FightBattle(c);
    }
    activeCombats.Clear();

    yield return cameraController.GoToDefaultPosition();
    yield return new WaitForSeconds(0.3f);

    /// RESTART PLAYER PHASE
    if(!CheckIfMatchIsOver()) {
      AdvanceTurn();
    }
  }

  public IEnumerator FightBattle(Combat combat) {
    yield return cameraController.FocusOnCombat(combat);

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
    CurrentCharacter.Walked();
    CurrentCharacter.transform.DOMove(new Vector3(pos.x, pos.y, 0f), dur).OnComplete(() => {
      SetGameState(GameState.actionsMenu);
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

  

  public void StartEngagement() {
    CharacterToken clicked = MouseInput.instance.SelectedCharacter;
    bool addedToEngagement = false;

    foreach(Combat c in activeCombats) {
      if(c.characters.Contains(clicked)) {
        c.AddConflict(clicked, CurrentCharacter);
        addedToEngagement = true;
        break;
      }
    }
    if(!addedToEngagement) {
      activeCombats.Add(new Combat(clicked, CurrentCharacter));
    }
    AdvanceTurn();
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
      SelectNoCharacter();
      gameState = GameState.noInput;
      return true;
    }
    if(DefeatConditionMet()) {
      VsnController.instance.StartVSN("defeat");
      SelectNoCharacter();
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
