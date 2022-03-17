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
  public CharacterId id;

  [Header("- Team -")]
  public CombatTeam combatTeam = CombatTeam.pc;

  [Header("- Visuals -")]
  public SpriteRenderer renderer;
  public GameObject hpSlider;

  [Header("- Effect Particles -")]
  public GameObject damageParticlePrefab;
  public GameObject missParticlePrefab;

  [Header("- My Battler -")]
  public Battler battler;


  public void Start() {
    /// Initialize Battler
    if(id <= CharacterId.maya) {
      battler = GlobalData.instance.pilots[(int)id];
    } else {
      Enemy newEnemy = new Enemy(BattleController.instance.GetEnemyById((int)id));
      battler = newEnemy;
      GlobalData.instance.instancedEnemies.Add(newEnemy);
    }

    Debug.LogWarning("character " + name + " registering");
    GameController.instance.RegisterCharacter(this);
    //renderer.sprite = ResourcesManager.instance
    UpdateHPSlider();
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

  public List<Vector2Int> GetWalkableTiles() {
    return WalkableTilesFromPosition(BoardGridPosition(), battler.AttributeValue(Attributes.movementRange));
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



  public void TackleAnim(Vector3 posToAttack) {
    Vector3 targetPos = (posToAttack + transform.position)/2f;
    transform.DOMove(targetPos, 0.15f).SetLoops(2, LoopType.Yoyo);
  }

  public void UpdateHPSlider() {
    hpSlider.transform.localScale = new Vector3((float)battler.hp / battler.AttributeValue(Attributes.maxHp), 1f, 1f);
  }

  //public void TakeDamage(int damage) {
  //  GameObject obj = Instantiate(damageParticlePrefab, transform);
  //  obj.transform.SetParent(transform.parent);
  //  obj.GetComponentInChildren<TextMeshPro>().text = damage.ToString();

  //  hp -= damage;
  //  UpdateHPSlider();
  //  CheckToDie();
  //}

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
    Debug.LogWarning("Character "+id+" has died!");
    GameController.instance.CharacterDies(this);
    Destroy(gameObject);
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
