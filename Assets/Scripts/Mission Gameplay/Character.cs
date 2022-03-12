using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

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
  enemy,
  npc,
  any,
  none
}


public class Character : MonoBehaviour {
  public CharacterId id;

  [Header("- Attributes -")]
  public int movementRange = 2;
  public int maxHp;
  public int attack;
  public int agility;
  public int dodgeRate = 0;

  [Header("- Team -")]
  public CombatTeam combatTeam = CombatTeam.enemy;

  [HideInInspector]public int hp;

  [Header("- HP Slider -")]
  public GameObject hpSlider;

  [Header("- Effect Particles -")]
  public GameObject damageParticlePrefab;
  public GameObject missParticlePrefab;


  public void Start() {
    hp = maxHp;
    Debug.LogWarning("character "+name+" registering");
    GameController.instance.RegisterCharacter(this);
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
    return WalkableTilesFromPosition(BoardGridPosition(), movementRange);
  }

  public List<Vector2Int> GetAdjacentTiles() {
    return BoardController.instance.GetWalkableNeighborTiles(BoardGridPosition(), CombatTeam.any);
  }


  public CombatTeam OpponentCombatTeam() {
    if(combatTeam == CombatTeam.enemy) {
      return CombatTeam.player;
    }
    return CombatTeam.enemy;
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
    hpSlider.transform.localScale = new Vector3((float)hp / maxHp, 1f, 1f);
  }

  public void TakeDamage(int damage) {
    GameObject obj = Instantiate(damageParticlePrefab, transform);
    obj.transform.SetParent(transform.parent);
    obj.GetComponentInChildren<TextMeshPro>().text = damage.ToString();

    hp -= damage;
    UpdateHPSlider();
    if(hp <= 0) {
      Die();
    }
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
}
