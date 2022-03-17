using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

public enum CharacterId {
  marcus = 0,
  agnes = 1,
  maya = 2,

  fly = 30,
  brute = 31,
  boss = 32,
}

public enum CombatTeam {
  player,
  pc,
  npc,
  any,
  none
}


public class CharacterToken : MonoBehaviour {
  [SerializeField]
  private CharacterId idToLoad;

  [Header("- Team -")]
  public CombatTeam combatTeam = CombatTeam.pc;

  [Header("- My Battler -")]
  public Battler battler;

  [Header("- Visuals -")]
  public SpriteRenderer renderer;
  public GameObject hpSlider;
  public GameObject currentCharacterIcon;

  [Header("- Effect Particles -")]
  public GameObject damageParticlePrefab;
  public GameObject missParticlePrefab;

  [SerializeField]
  private Vector3 turnInitialPosition;
  public bool canRevertMovement;
  public bool canWalk;

  public int Id {
    get {
      return battler.id;
    }
  }



  public void Start() {
    /// Initialize Battler
    if(idToLoad <= CharacterId.maya) {
      battler = GlobalData.instance.pilots[(int)idToLoad];
    } else {
      Enemy newEnemy = new Enemy(BattleController.instance.GetEnemyById((int)idToLoad));
      battler = newEnemy;
      GlobalData.instance.instancedEnemies.Add(newEnemy);
    }

    Debug.LogWarning("character " + name + " registering");
    GameController.instance.RegisterCharacter(this);
    //renderer.sprite = ResourcesManager.instance
    UpdateUI();
    SnapToTile();
  }

  public void SnapToTile() {
    Grid grid = BoardController.instance.floorBoard.layoutGrid;
    transform.position = grid.CellToWorld(BoardGridPosition());
  }


  public Vector2Int BoardGridPosition() {
    Grid grid = BoardController.instance.floorBoard.layoutGrid;
    return (Vector2Int)grid.WorldToCell(transform.position);
  }

  public List<Vector2Int> GetAdjacentTiles() {
    return BoardController.instance.GetWalkableNeighborTiles(BoardGridPosition(), CombatTeam.any);
  }


  public CombatTeam OpponentCombatTeam() {
    if(combatTeam == CombatTeam.pc) {
      return CombatTeam.player;
    }
    return CombatTeam.pc;
  }


  public List<Vector2Int> WalkableTilesFromPosition(Vector2Int pos, int movesLeft) {
    List<Vector2Int> walkableTiles = new List<Vector2Int>();
    List<Vector2Int> neighbors = new List<Vector2Int>();

    if(movesLeft == 0) {
      return walkableTiles;
    }

    neighbors = BoardController.instance.GetWalkableNeighborTiles(pos, combatTeam);
    walkableTiles.AddRange(neighbors);
    foreach(Vector2Int tile in neighbors) {
      walkableTiles.AddRange(WalkableTilesFromPosition(tile, movesLeft-1));
    }
    return walkableTiles;
  }



  public void InitializeTurn() {
    canRevertMovement = false;
    canWalk = true;
    //EndDefense();
    turnInitialPosition = transform.position;
  }

  public void EndTurn() {
    //Debug.LogWarning("ENDING " + attributes.id + " TURN!");

    /// pass status conditions turn
    //for(int i = statusConditions.Count - 1; i >= 0; i--) {
    //  if(statusConditions[i].duration > 0) {
    //    statusConditions[i].duration--;
    //  }
    //  if(statusConditions[i].duration == 0) {
    //    statusConditions.RemoveAt(i);
    //  }
    //}
  }


  public void Walked() {
    canRevertMovement = true;
    canWalk = false;
  }


  public void TackleAnim(Vector3 posToAttack) {
    Vector3 targetPos = (posToAttack + transform.position)/2f;
    transform.DOMove(targetPos, 0.15f).SetLoops(2, LoopType.Yoyo);
  }

  public void UpdateUI() {
    renderer.sprite = ResourcesManager.instance.GetCharacterSprite(Id, CharacterSpritePart.mapIcon);
    hpSlider.transform.localScale = new Vector3((float)battler.hp / battler.AttributeValue(Attributes.maxHp), 1f, 1f);
  }

  public bool CheckToDie() {
    if(battler.hp <= 0) {
      Die();
      return true;
    }
    return false;
  }

  public void MissParticle() {
    GameObject obj = Instantiate(missParticlePrefab, transform);
    obj.transform.SetParent(transform.parent);
  }


  public void Die() {
    Debug.LogWarning("Character " + Id + " has died!");
    GameController.instance.CharacterDies(this);
    Destroy(gameObject);
  }


  public void RevertMovement() {
    transform.position = turnInitialPosition;
    canRevertMovement = false;
  }

  public void BecomeCurrentCharacter(bool isCurrent) {
    currentCharacterIcon.SetActive(isCurrent);
  }





  [Button]
  public void PrintBattler() {
    if(battler == null) {
      Debug.LogError("Battler is NULL!");
    } else {
      Debug.LogWarning("Battler is not null. Name: "+battler.nameKey+", id: "+battler.id);
    }    
  }
}
