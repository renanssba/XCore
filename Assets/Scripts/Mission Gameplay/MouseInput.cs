using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MouseInput : MonoBehaviour {
  public static MouseInput instance;

  public Tilemap world;
  public Vector2Int lastWalkableTileSelected;


  public CharacterToken SelectedCharacter {
    get {
      Vector2Int selectedPos = SelectedGridPosition();

      foreach(CharacterToken c in GameController.instance.allCharacters) {
        if(c.BoardGridPosition() == selectedPos) {
          return c;
        }
      }
      return null;
    }
  }


  public void Awake() {
    instance = this;
  }

  void Update() {
    Grid grid = BoardController.instance.GetComponent<Grid>();
    Vector2Int selectedPos = SelectedGridPosition();
    Vector3 v = grid.CellToWorld(selectedPos);

    transform.position = new Vector3(v.x, v.y, 0f);
    if(HighlightedTilesLayer.instance.IsTileWalkable(selectedPos)) {
      lastWalkableTileSelected = selectedPos;
    }

    if(SelectedCharacter != null) {
      TacticalUIController.instance.SelectCharacterByCursor(SelectedCharacter.battler);
    } else {
      TacticalUIController.instance.SelectCharacterByCursor(null);
    }
  }


  public Vector2Int SelectedGridPosition() {
    Grid grid = BoardController.instance.floorBoard.layoutGrid;
    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    return (Vector2Int)grid.WorldToCell(pos);
  }

}
