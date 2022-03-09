using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;


public enum GameState {
  choosingMovement,
  choosingEngage,
  battlePhase,
  noInput
}


// height: 0.87
public class BoardController : MonoBehaviour {

  public static BoardController instance;

  [Header("- Tilemaps -")]
  public Tilemap floorBoard;
  public Tilemap selectionBoard;


  [Header("- Tiles -")]
  public TileBase wallTile;
  public TileBase highlightedTile;


  public Dictionary<Vector3, WorldTile> tilesData;

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


  // currently not being used
  // reference: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
  public void InitializeWorldTiles() {
    tilesData = new Dictionary<Vector3, WorldTile>();
    foreach(Vector3Int pos in floorBoard.cellBounds.allPositionsWithin) {
      var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

      if(!floorBoard.HasTile(localPlace)) continue;
      var tile = new WorldTile {
        LocalPlace = localPlace,
        WorldLocation = floorBoard.CellToWorld(localPlace),
        TileBase = floorBoard.GetTile(localPlace),
        TilemapMember = floorBoard,
        Name = localPlace.x + "," + localPlace.y,
        Cost = 1 // TODO: Change this with the proper cost from ruletile
      };

      tilesData.Add(tile.WorldLocation, tile);
    }
  }


  



  public void HighlightWalkableTiles(Character character) {
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



  public void HighlightAdjacentEnemies(Character character) {
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
    foreach(Character c in GameController.instance.allCharacters) {
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
