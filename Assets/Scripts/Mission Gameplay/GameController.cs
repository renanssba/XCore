using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Combat {
  public List<Character> characters;
  public List<GameObject> engageIcons;

  public Combat(Character a, Character b) {
    characters = new List<Character>();
    engageIcons = new List<GameObject>();
    
    AddConflict(a, b);
  }

  public void AddConflict(Character a, Character b) {
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
  }


  public List<Character> TurnsOrder() {
    return characters.OrderByDescending(q => q.agility).ToList();
  }

  public Character RandomCharacterInTeam(CombatTeam team) {
    List<Character> chars = new List<Character>();
    foreach(Character c in characters) {
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
    foreach(Character c in characters) {
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


  public static bool CharacterIsEngagedInCombat(Character a) {
    foreach(Combat c in GameController.instance.currentCombats) {
      if(c.characters.Contains(a)) {
        return true;
      }
    }
    return false; 
  }
}


public class GameController : MonoBehaviour {
  public static GameController instance;

  [Header("- Game State -")]
  public GameState gameState = GameState.choosingMovement;


  [Header("- Characters -")]
  public List<Character> allCharacters;
  public int currentCharacterId;


  [Header("- Combats -")]
  public List<Combat> currentCombats;
  public GameObject engageIconPrefab;

  public Combat combatHappening;


  [Header("- Camera -")]
  public CameraController cameraController;

  [Header("- Battle Setup -")]
  public Camera[] cameras;
  public BattleController battleController;


  public Character CurrentCharacter {
    get { return allCharacters[currentCharacterId]; }
  }



  public void Awake() {
    instance = this;

    allCharacters = new List<Character>();
    currentCombats = new List<Combat>();
  }

  public void Start() {
    StartCoroutine(WaitSetupToStart());
  }

  public IEnumerator WaitSetupToStart() {
    yield return null;
    Initialize();
  }

  public void Initialize() {
    BoardController.instance.InitializeWorldTiles();
    StartMovementStep(0);
  }

  void Update() {
    if(Input.GetKeyDown(KeyCode.Space)) {
      AdvanceStep();
    }

    if(Input.GetKeyDown(KeyCode.F5)) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }


  public void ClickedMap() {
    if(gameState == GameState.noInput || gameState == GameState.battlePhase) {
      return;
    }
    Vector2Int clickedGridPos = MouseInput.instance.SelectedGridPosition();
    Vector3 pos = BoardController.instance.floorBoard.layoutGrid.CellToWorld(clickedGridPos);

    ClickDamageCharacter();

    if(BoardController.instance.selectionBoard.GetTile(new Vector2Int(clickedGridPos.x, clickedGridPos.y)) == null) {
      Debug.LogWarning("Error SFX");
      return;
    }

    switch(gameState) {
      case GameState.choosingMovement:
        // clicked during choose movement phase
        StartMovement(clickedGridPos);
        break;
      case GameState.choosingEngage:
        // clicked during choose engagement phase
        StartEngagement();
        break;
    }    
  }


  public void ClickDamageCharacter() {
    if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
      if(MouseInput.instance.SelectedCharacter != null) {
        Debug.LogWarning(MouseInput.instance.SelectedCharacter.name + " clicked to receive damage!");
        MouseInput.instance.SelectedCharacter.TakeDamage(1);
      }
    }
  }


  public void CleanHighlightedTiles() {
    BoardController.instance.StopHighlightWalkableTiles();
    gameState = GameState.noInput;
  }

  public void AdvanceStep() {
    if(gameState == GameState.choosingMovement) {
      StartEngagementStep();
    } else if(gameState == GameState.choosingEngage) {
      EndCurrentCharacterTurn();
    }
  }

  public void EndCurrentCharacterTurn() {
    CleanHighlightedTiles();
    currentCharacterId++;
    if(currentCharacterId >= allCharacters.Count) {
      StartCoroutine(CombatsAnimation());
      return;
    }

    if(Combat.CharacterIsEngagedInCombat(CurrentCharacter)) {
      EndCurrentCharacterTurn();
      return;
    }

    StartMovementStep(currentCharacterId);
    gameState = GameState.choosingMovement;
  }



  public IEnumerator CombatsAnimation() {
    foreach(Combat c in currentCombats) {
      yield return FightBattle(c);
      c.DestroyIcons();
    }
    currentCombats.Clear();

    yield return cameraController.GoToDefaultPosition();
    yield return new WaitForSeconds(0.3f);

    /// START PLAYER PHASE
    if(VictoryConditionMet()) {
      VsnController.instance.StartVSN("victory");
    } else if(DefeatConditionMet()) {
      VsnController.instance.StartVSN("defeat");
    } else {
      StartMovementStep(0);
      gameState = GameState.choosingMovement;
    }    
  }

  public IEnumerator FightBattle(Combat combat) {
    combatHappening = combat;
    VsnController.instance.StartVSN("battle");
    yield break;


    List<Character> turns = combat.TurnsOrder();

    yield return cameraController.FocusOnCombat(combat);

    foreach(Character currentChar in turns) {
      if(currentChar == null) {
        continue;
      }

      Debug.LogWarning("Character "+currentChar+" will attack!");

      yield return new WaitForSeconds(0.1f);

      // attack and take damage
      Character targetChar = combat.RandomCharacterInTeam(currentChar.OpponentCombatTeam());
      if(targetChar == null) {
        continue;
      }
      currentChar.TackleAnim(targetChar.transform.position);
      yield return new WaitForSeconds(0.3f);

      if(Random.Range(0, 100) < targetChar.dodgeRate) {
        targetChar.MissParticle();
      } else {
        targetChar.TakeDamage(currentChar.attack);
      }
      yield return new WaitForSeconds(1.5f);
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
      StartEngagementStep();
    });
  }


  public void StartMovementStep(int id) {
    currentCharacterId = id;
    BoardController.instance.HighlightWalkableTiles(CurrentCharacter);
  }




  public void StartEngagementStep() {
    CleanHighlightedTiles();
    gameState = GameState.choosingEngage;
    BoardController.instance.HighlightAdjacentEnemies(CurrentCharacter);
  }

  

  public void StartEngagement() {
    Character clicked = MouseInput.instance.SelectedCharacter;
    foreach(Combat c in currentCombats) {
      if(c.characters.Contains(clicked)) {
        c.AddConflict(clicked, CurrentCharacter);
        EndCurrentCharacterTurn();
        return;
      }
    }

    currentCombats.Add(new Combat(clicked, CurrentCharacter));
    EndCurrentCharacterTurn();
  }


  public void RegisterCharacter(Character newCharacter) {
    allCharacters.Add(newCharacter);
    allCharacters = allCharacters.OrderBy(o => o.id).ToList();
  }

  public void CharacterDies(Character dyingCharacter) {
    int indexInList = allCharacters.IndexOf(dyingCharacter);
    allCharacters.Remove(dyingCharacter);
  }


  public bool VictoryConditionMet() {
    foreach(Character c in allCharacters) {
      if(c != null && c.combatTeam == CombatTeam.enemy) {
        return false;
      }
    }
    return true;
  }

  public bool DefeatConditionMet() {
    foreach(Character c in allCharacters) {
      if(c != null && c.combatTeam == CombatTeam.player) {
        return false;
      }
    }
    return true;
  }


  public void StartBattle() {
    cameras[0].gameObject.SetActive(false);
    cameras[1].gameObject.SetActive(true);
    gameState = GameState.battlePhase;
    TacticalUIController.instance.Select(null);
    TacticalUIController.instance.EnterBattlePhase();
    BoardController.instance.gameObject.SetActive(false);
  }

  public void EndBattle() {
    cameras[0].gameObject.SetActive(true);
    cameras[1].gameObject.SetActive(false);
    TacticalUIController.instance.EndBattlePhase();
    BoardController.instance.gameObject.SetActive(true);
  }
}
