using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;


// height: 0.87
public class BoardController : MonoBehaviour {

  public static BoardController instance;

  [Header("- Tilemaps -")]
  public Tilemap floorBoard;

  [Header("- Tiles -")]
  public TileBase wallTile;



  public void Awake() {
    instance = this;
  }


  ///  RANGE CALCULATION
  public List<Vector2Int> WalkableTilesFromPosition(Vector2Int pos, int movesLeft, CombatTeam combatTeam) {
    List<Vector2Int> walkableTiles = new List<Vector2Int> { pos };
    List<Vector2Int> neighbors = new List<Vector2Int>();

    if(movesLeft == 0) {
      return walkableTiles;
    }

    neighbors = GetWalkableAdjacentTiles(pos, combatTeam);
    foreach(Vector2Int tile in neighbors) {
      walkableTiles.AddRange(WalkableTilesFromPosition(tile, movesLeft - 1, combatTeam));
    }
    walkableTiles = walkableTiles.Distinct().ToList();
    return walkableTiles;
  }


  public void HighlightWalkableTiles(CharacterToken character) {
    List<Vector2Int> selectedTiles = WalkableTilesFromPosition(character.BoardGridPosition(),
                                                               character.battler.AttributeValue(Attributes.movementRange),
                                                               character.combatTeam);
    foreach(Vector2Int pos in selectedTiles) {
      HighlightedTilesLayer.instance.SetTile(pos, TileHighlightType.walkable);
    }
  }



  public void HighlightAdjacentEnemies(CharacterToken character) {
    List<Vector2Int> selectedTiles = GetAdjacentTiles(character.BoardGridPosition());
    CombatTeam opponentTeam = character.OpponentCombatTeam();

    selectedTiles = selectedTiles.FilterByCombatTeam(opponentTeam);

    foreach(Vector2Int pos in selectedTiles) {
      HighlightedTilesLayer.instance.SetTile(pos, TileHighlightType.offensiveSkill);
    }
    //selectionBoard.GetComponent<TilemapRenderer>().sortingOrder = 5;
  }



  public CombatTeam TeamInTile(Vector2Int pos) {
    foreach(CharacterToken c in GameController.instance.allCharacters) {
      if(c.BoardGridPosition() == pos) {
        return c.combatTeam;
      }
    }
    return CombatTeam.none;
  }



  public List<Vector2Int> GetWalkableAdjacentTiles(Vector2Int pos, CombatTeam playerTeam) {
    List<Vector2Int> adjacent = GetAdjacentTiles(pos);
    List<Vector2Int> empties = adjacent.FilterByCombatTeam(CombatTeam.none);
    List<Vector2Int> companions = adjacent.FilterByCombatTeam(playerTeam);
    empties.AddRange(companions);

    return empties;
  }


  public List<Vector2Int> GetAdjacentTiles(Vector2Int pos) {
    List<Vector2Int> neighbors = new List<Vector2Int>();

    for(int i = 0; i < 20; i++) {
      for(int j = 0; j < 20; j++) {
        TileBase tileB = floorBoard.GetTile(new Vector2Int(i, j));
        if(tileB != null) {
          if(IsCloseEnough(floorBoard.layoutGrid.CellToWorld(pos),
                           floorBoard.layoutGrid.CellToWorld(new Vector2Int(i, j)), 1) &&
             floorBoard.GetTile(new Vector2Int(i, j)) != wallTile) {
            neighbors.Add(new Vector2Int(i, j));
          }
        }
      }
    }
    return neighbors;
  }

  public List<Vector2Int> GetWalkableNeighborTiles(Vector2Int pos, CombatTeam playerTeam) {
    List<Vector2Int> neighbors = new List<Vector2Int>();

    for(int i=0; i<20; i++) {
      for(int j=0; j<20; j++) {
        TileBase tileB = floorBoard.GetTile(new Vector2Int(i, j));
        if(tileB != null) {
          if(IsCloseEnough(floorBoard.layoutGrid.CellToWorld(pos),
                           floorBoard.layoutGrid.CellToWorld(new Vector2Int(i, j)), 1) &&
             floorBoard.GetTile(new Vector2Int(i, j)) != wallTile &&
             (playerTeam == CombatTeam.any || TeamInTile(pos) == playerTeam || TeamInTile(pos) == CombatTeam.none) ) {
            neighbors.Add(new Vector2Int(i, j));
          }
        }
      }
    }
    return neighbors;
  }

  public bool IsCloseEnough(Vector3 posA, Vector3 posB, int distance) {
    if(Vector3.Distance(posA, posB) < 1.003f * distance + 0.05f) {
      return true;
    }
    return false;
  }

}
