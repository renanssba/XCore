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

  [Header("- Battle Setup -")]
  public BattleController battleController;


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

    // Play Tactical Song
    //VsnAudioManager.instance.PlayMusic("Valkyrie Arena OST 1 INTRO", "Valkyrie Arena OST 1 LOOP");
    VsnController.instance.StartVSN("start battle");
    StartCoroutine(WaitSetupToStart());
  }


  public IEnumerator WaitSetupToStart() {
    while(VsnController.instance.state != ExecutionState.STOPPED) {
      yield return null;
    }
    yield return null;
    SetGameState(GameState.setupPhase);
  }


  void Update() {
    if(Input.GetKeyDown(KeyCode.F5)) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// If current input is IA, dont take input from player
    if(InputOrigin == InputOrigin.IA) {
      return;
    }

    //if(Input.GetKeyDown(KeyCode.Space)) {
    //  AdvanceStep();
    //}

    /// Clicked Cancel button
    if(Input.GetMouseButtonDown(1)) {
      ClickedCancelButton();
    }
  }


  public void SetGameState(GameState newState) {
    CleanHighlightedTiles();

    gameState = GameState.noInput;
    if(newState == GameState.actionsMenu && CheckIfMatchIsOver()) {
      return;
    } else {
      StartCoroutine(UpdateGameState(newState));
    }
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
        BoardController.instance.HighlightWalkableTiles(CurrentCharacter);
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
        break;
      default:
        // do nothing
        break;
    }

    bool needsCancelButton = false;
    if(gameState == GameState.chooseEngagement ||
       gameState == GameState.confirmEngagement) {
      needsCancelButton = true;
    }
    TacticalUIController.instance.cancelButton.SetActive(needsCancelButton);
  }


  public void ClickedMap() {
    if(gameState == GameState.noInput || gameState == GameState.battlePhase ||
       Input.GetMouseButtonDown(0) == false) {
      return;
    }
    Vector2Int clickedGridPos = MouseInput.instance.SelectedGridPosition();
    Vector3 pos = BoardController.instance.floorBoard.layoutGrid.CellToWorld(clickedGridPos);

    if(BoardController.instance.selectionBoard.GetTile(new Vector2Int(clickedGridPos.x, clickedGridPos.y)) == null) {
      Debug.LogWarning("Error SFX");
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
      //!PlayerCanInput ||
      gameState == GameState.chooseMovement) {
      return;
    }

    SfxManager.StaticPlayCancelSfx();
    switch(gameState) {
      case GameState.actionsMenu:
        if(RevertMovement()) {
          SetGameState(GameState.chooseMovement);
        }
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
    BoardController.instance.StopHighlightWalkableTiles();
    gameState = GameState.noInput;
  }

  public void AdvanceStep() {
    if(gameState == GameState.chooseMovement) {
      StartEngagementStep();
    } else if(gameState == GameState.chooseEngagement) {
      EndCurrentCharacterTurn();
    }
  }

  public void EndCurrentCharacterTurn() {
    CleanHighlightedTiles();
    currentCharacterPos++;
    if(currentCharacterPos >= allCharacters.Count) {
      StartCoroutine(FightsPhase());
      return;
    }

    if(Combat.CharacterIsEngagedInCombat(CurrentCharacter)) {
      EndCurrentCharacterTurn();
      return;
    }

    StartMovementStep(currentCharacterPos);
    gameState = GameState.chooseMovement;
  }



  public IEnumerator FightsPhase() {
    foreach(Combat c in activeCombats) {
      yield return FightBattle(c);
      c.DestroyIcons();
    }
    activeCombats.Clear();

    yield return cameraController.GoToDefaultPosition();
    yield return new WaitForSeconds(0.3f);

    /// RESTART PLAYER PHASE
    if(!CheckIfMatchIsOver()) {
      StartMovementStep(0);
      gameState = GameState.chooseMovement;
    }
  }

  public IEnumerator FightBattle(Combat combat) {
    yield return cameraController.FocusOnCombat(combat);

    currentCombat = combat;
    VsnController.instance.StartVSN("battle");
    yield return new WaitForSeconds(0.5f);

    while(VsnController.instance.state != ExecutionState.STOPPED) {
      yield return null;
    }

    bool someoneDies = false;
    foreach(CharacterToken ct in combat.characters) {
      ct.UpdateHPSlider();
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
    CurrentCharacter.transform.DOMove(new Vector3(pos.x, pos.y, 0f), dur).OnComplete(() => {
      SetGameState(GameState.chooseEngagement);
    });
  }


  public bool RevertMovement() {
    /// TODO: Implement
    return false;
    //if(!CurrentCharacter.canRevertMovement) {
    //  SfxManager.StaticPlayForbbidenSfx();
    //  return false;
    //}
    //CurrentCharacter.RevertMovement();
    ////cameraController.FocusOnCharacterImmediate(CurrentCharacter);
    //return true;
  }

  public void StartMovementStep(int pos) {
    currentCharacterPos = pos;
    BoardController.instance.HighlightWalkableTiles(CurrentCharacter);
  }




  public void StartEngagementStep() {
    CleanHighlightedTiles();
    gameState = GameState.chooseEngagement;
    BoardController.instance.HighlightAdjacentEnemies(CurrentCharacter);
  }

  

  public void StartEngagement() {
    CharacterToken clicked = MouseInput.instance.SelectedCharacter;
    foreach(Combat c in activeCombats) {
      if(c.characters.Contains(clicked)) {
        c.AddConflict(clicked, CurrentCharacter);
        EndCurrentCharacterTurn();
        return;
      }
    }

    activeCombats.Add(new Combat(clicked, CurrentCharacter));
    EndCurrentCharacterTurn();
  }


  public void RegisterCharacter(CharacterToken newCharacter) {
    allCharacters.Add(newCharacter);
    allCharacters = allCharacters.OrderBy(o => o.id).ToList();
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
      //Debug.LogWarning("end battle: victory");
      VsnController.instance.StartVSN("victory");
      currentCharacterPos = -1;
      gameState = GameState.noInput;
      return true;
    }
    if(DefeatConditionMet()) {
      //Debug.LogWarning("end battle: defeat");
      VsnController.instance.StartVSN("defeat");
      currentCharacterPos = -1;
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


  public void StartBattle() {
    gameState = GameState.battlePhase;
  }

  public void EndBattle() {
    gameState = GameState.chooseMovement;
  }
}
