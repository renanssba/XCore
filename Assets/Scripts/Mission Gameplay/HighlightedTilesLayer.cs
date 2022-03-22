﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightedTilesLayer : MonoBehaviour {

  public static HighlightedTilesLayer instance;

  [Header("- References -")]
  public GameObject highlightedTilePrefab;
  public Transform highlightedContent;
  public List<Vector2Int> highlightedPositions;


  public void Awake() {
    instance = this;
    highlightedPositions = new List<Vector2Int>();
  }


  public void SetTile(Vector2Int pos, TileHighlightType highlightType) {
    if(highlightedPositions.Contains(pos)) {
      return;
    }
    highlightedPositions.Add(pos);

    Grid grid = BoardController.instance.floorBoard.layoutGrid;
    Vector3 spawnPos = grid.CellToWorld(pos);

    GameObject newObj = Instantiate(highlightedTilePrefab, spawnPos, Quaternion.identity, highlightedContent);
    newObj.GetComponent<TileHighlight>().Initialize(highlightType);
  }

  public bool IsTileHighlighted(Vector2Int pos) {
    return highlightedPositions.Contains(pos);
  }


  public void StopHighlightWalkableTiles() {
    TileHighlight[] tiles = GetComponentsInChildren<TileHighlight>();
    foreach(TileHighlight t in tiles) {
      t.BeforeDestroy();
    }

    highlightedContent.ClearChildren();
    highlightedPositions.Clear();
  }
}
