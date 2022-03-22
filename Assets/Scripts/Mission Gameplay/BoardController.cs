using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;


// Unity grid tile size for this project
// width: 0.5
// height: 0.87
public class BoardController : MonoBehaviour {

  public static BoardController instance;

  [Header("- Tilemaps -")]
  public Tilemap floorBoard;

  [Header("- Tiles -")]
  public TileBase wallTile;

  [Header("- Pathfindig -")]
  public AStarAlgorithm pathfinding;



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


  public List<Vector2Int> CalculateWalkableTiles(CharacterToken character) {
    List<Vector2Int> selectedTiles = WalkableTilesFromPosition(character.BoardGridPosition(),
                                                               character.battler.GetAttributeValue(Attributes.movementRange),
                                                               character.combatTeam);

    selectedTiles = selectedTiles.FilterByCombatTeam(CombatTeam.none);
    selectedTiles.Add(character.BoardGridPosition());
    return selectedTiles;
  }

  public void HighlightWalkableTiles(CharacterToken character) {
    List<Vector2Int> walkableTiles = CalculateWalkableTiles(character);

    foreach(Vector2Int pos in walkableTiles) {
      HighlightedTilesLayer.instance.SetTile(pos, TileHighlightType.walkableTile);
    }

    HighlightEngageableTiles(walkableTiles);
  }

  public void HighlightEngageableTiles(List<Vector2Int> walkableTiles) {
    List<Vector2Int> engageableTiles = new List<Vector2Int>();

    foreach(Vector2Int w in walkableTiles) {
      engageableTiles.AddRange(GetAdjacentTiles(w));
    }
    engageableTiles = engageableTiles.Distinct().ToList();

    foreach(Vector2Int pos in engageableTiles) {
      if(!walkableTiles.Contains(pos)) {
        HighlightedTilesLayer.instance.SetTile(pos, TileHighlightType.characterToEngage);
      }
    }
  }


  public List<Vector2Int> CalculateEngagementTargets(CharacterToken character) {
    List<Vector2Int> selectedTiles = GetAdjacentTiles(character.BoardGridPosition());
    CombatTeam opponentTeam = character.OpponentCombatTeam();

    selectedTiles = selectedTiles.FilterByCombatTeam(opponentTeam);
    selectedTiles = selectedTiles.FilterEngageableTargets(character.combatTeam);
    return selectedTiles;
  }

  public void HighlightAdjacentEnemies(CharacterToken character) {
    List<Vector2Int> selectedTiles = CalculateEngagementTargets(character);

    foreach(Vector2Int pos in selectedTiles) {
      HighlightedTilesLayer.instance.SetTile(pos, TileHighlightType.characterToEngage);
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

    for(int i = pos.x - 1; i <= pos.x + 1; i++) {
      for(int j = pos.y - 1; j <= pos.y + 1; j++) {
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

  public bool IsCloseEnough(Vector3 posA, Vector3 posB, int distance) {
    if(Vector3.Distance(posA, posB) < 1.003f * distance + 0.05f) {
      return true;
    }
    return false;
  }


  public CharacterToken CharacterInPosition(Vector2Int selectedPos) {
    foreach(CharacterToken c in GameController.instance.allCharacters) {
      if(c.BoardGridPosition() == selectedPos) {
        return c;
      }
    }
    return null;
  }

  public bool IsPositionWalkable(Vector2Int pos, CombatTeam alliesTeam) {
    if(TeamInTile(pos) != alliesTeam && TeamInTile(pos) != CombatTeam.none &&
       alliesTeam != CombatTeam.any) {
      return false;
    }
    return true;
  }

}
