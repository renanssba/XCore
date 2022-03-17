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
  public Tilemap selectionBoard;


  [Header("- Tiles -")]
  public TileBase wallTile;
  public TileBase highlightedTile;


  [Header("- Shine -")]
  public Color playerHighlightColor;
  public Color enemyHighlightColor;
  public Color engageHighlightColor;
  public float time;


  [Header("- Engagement Icons -")]
  public TileBase engageTile;



  public void Awake() {
    instance = this;
  }



  public void HighlightWalkableTiles(CharacterToken character) {
    List<Vector2Int> neighbors = character.GetWalkableTiles();

    foreach(Vector2Int pos in neighbors) {
      if(TeamInTile(pos) == CombatTeam.none ||
         pos == character.BoardGridPosition()) {
        selectionBoard.SetTile(pos, highlightedTile);
      }      
    }

    selectionBoard.GetComponent<TilemapRenderer>().sortingOrder = 0;
    ShineWalkableTiles(character.combatTeam);
  }

  public void ShineWalkableTiles(CombatTeam combatTeam) {
    Color highlightColor;
    if(combatTeam == CombatTeam.player) {
      highlightColor = playerHighlightColor;
    } else {
      highlightColor = enemyHighlightColor;
    }
    ShineTiles(highlightColor);
  }



  public void HighlightAdjacentEnemies(CharacterToken character) {
    List<Vector2Int> neighbors = character.GetAdjacentTiles();
    CombatTeam opponentTeam = character.OpponentCombatTeam();

    foreach(Vector2Int pos in neighbors) {
      if(TeamInTile(pos) == opponentTeam) {
        selectionBoard.SetTile(pos, engageTile);
      }
    }

    selectionBoard.GetComponent<TilemapRenderer>().sortingOrder = 5;
    ShineEngagementTiles();
  }

  public void ShineEngagementTiles() {
    ShineTiles(engageHighlightColor);
  }

  public void ShineTiles(Color highlightColor) {
    DOTween.Kill(selectionBoard);
    selectionBoard.color = highlightColor;
    DOTween.To(() => selectionBoard.color, x => selectionBoard.color = x, highlightColor * 1.5f, time).
      SetLoops(-1, LoopType.Yoyo);
  }



  public CombatTeam TeamInTile(Vector2Int pos) {
    foreach(CharacterToken c in GameController.instance.allCharacters) {
      if(c.BoardGridPosition() == pos) {
        return c.combatTeam;
      }
    }
    return CombatTeam.none;
  }

  public void StopHighlightWalkableTiles() {
    DOTween.KillAll(selectionBoard);
    for(int i = 0; i < 20; i++) {
      for(int j = 0; j < 20; j++) {
        TileBase tileB = selectionBoard.GetTile(new Vector2Int(i, j));
        if(tileB != null) {
          selectionBoard.SetTile(new Vector2Int(i, j), null);
        }
      }
    }
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
