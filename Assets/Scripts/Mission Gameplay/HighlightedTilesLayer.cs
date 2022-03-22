using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightedTilesLayer : MonoBehaviour {

  public static HighlightedTilesLayer instance;

  [Header("- References -")]
  public GameObject highlightedTilePrefab;
  public Transform highlightedContent;
  public List<Vector2Int> highlightedPositions;
  public List<TileHighlight> highlights;


  public void Awake() {
    instance = this;
    highlightedPositions = new List<Vector2Int>();
    highlights = new List<TileHighlight>();
  }


  public void SetTile(Vector2Int pos, TileHighlightType highlightType) {
    if(highlightedPositions.Contains(pos)) {
      return;
    }
    highlightedPositions.Add(pos);

    Grid grid = BoardController.instance.floorBoard.layoutGrid;
    Vector3 spawnPos = grid.CellToWorld(pos);

    GameObject newObj = Instantiate(highlightedTilePrefab, spawnPos, Quaternion.identity, highlightedContent);
    newObj.GetComponent<TileHighlight>().Initialize(pos, highlightType);
    highlights.Add(newObj.GetComponent<TileHighlight>());
  }

  public bool IsTileInputValid(Vector2Int pos) {
    foreach(TileHighlight tile in highlights) {
      if(tile.myPos == pos) {
        return tile.IsInputValid();
      }
    }
    return false;
  }

  public bool IsTileWalkable(Vector2Int pos) {
    foreach(TileHighlight tile in highlights) {
      if(tile.myPos == pos && tile.mytype == TileHighlightType.walkableTile) {
        return tile.IsInputValid();
      }
    }
    return false;
  }

  public TileHighlightType HighlightTypeInPos(Vector2Int pos) {
    foreach(TileHighlight tile in highlights) {
      if(tile.myPos == pos) {
        return tile.mytype;
      }
    }
    return TileHighlightType.none;
  }


  public void StopHighlightWalkableTiles() {
    TileHighlight[] tiles = GetComponentsInChildren<TileHighlight>();
    foreach(TileHighlight t in tiles) {
      t.BeforeDestroy();
    }

    highlightedContent.ClearChildren();
    highlightedPositions.Clear();
    highlights.Clear();
  }
}
