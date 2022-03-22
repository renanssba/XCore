using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum TileHighlightType {
  walkableTile,
  characterToEngage,
  supportSkill,
  none
}


public class TileHighlight : MonoBehaviour {

  [Header("- Data -")]
  public TileHighlightType mytype;
  public Vector2Int myPos;

  [Header("- Renderer -")]
  public SpriteRenderer tileRenderer;

  [Header("- Colors -")]
  public Color movementHighlightColor;
  public Color attackHighlightColor;
  public Color supportHighlightColor;

  public float shineAnimTime = 0.8f;


  public void Initialize(Vector2Int pos, TileHighlightType type) {
    myPos = pos;
    mytype = type;
    switch(type) {
      case TileHighlightType.walkableTile:
        ShineTile(movementHighlightColor);
        break;
      case TileHighlightType.characterToEngage:
        ShineTile(attackHighlightColor);
        break;
      case TileHighlightType.supportSkill:
        ShineTile(supportHighlightColor);
        break;
    }
  }

  public void ShineTile(Color highlightColor) {
    tileRenderer.color = highlightColor;
    Color endColor = highlightColor * 1.5f;
    endColor.a = highlightColor.a;
    tileRenderer.DOColor(endColor, shineAnimTime).SetLoops(-1, LoopType.Yoyo);
  }

  public bool IsInputValid() {
    if(mytype == TileHighlightType.walkableTile) {
      if(BoardController.instance.CharacterInPosition(myPos) != null &&
         BoardController.instance.CharacterInPosition(myPos) != GameController.instance.CurrentCharacter) {
        return false;
      }
    }
    if(mytype == TileHighlightType.characterToEngage) {
      if(BoardController.instance.CharacterInPosition(myPos) == null ||
         BoardController.instance.CharacterInPosition(myPos) == GameController.instance.CurrentCharacter) {
        return false;
      }
    }
    return true;
  }

  public void BeforeDestroy() {
    DOTween.Kill(tileRenderer);
  }
}
