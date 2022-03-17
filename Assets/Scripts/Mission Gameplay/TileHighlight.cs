using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum TileHighlightType {
  walkable,
  offensiveSkill,
  supportSkill
}


public class TileHighlight : MonoBehaviour {
  [Header("- Renderer -")]
  public SpriteRenderer tileRenderer;

  [Header("- Colors -")]
  public Color movementHighlightColor;
  public Color attackHighlightColor;
  public Color supportHighlightColor;

  public float shineAnimTime = 0.8f;

  TileHighlightType mytype;


  public void Initialize(TileHighlightType type) {
    mytype = type;
    switch(type) {
      case TileHighlightType.walkable:
        ShineTiles(movementHighlightColor);
        break;
      case TileHighlightType.offensiveSkill:
        ShineTiles(attackHighlightColor);
        break;
      case TileHighlightType.supportSkill:
        ShineTiles(supportHighlightColor);
        break;
    }
  }

  public void ShineTiles(Color highlightColor) {
    tileRenderer.color = highlightColor;
    tileRenderer.DOColor(highlightColor * 1.5f, shineAnimTime).SetLoops(-1, LoopType.Yoyo);
  }

  public void BeforeDestroy() {
    DOTween.Kill(tileRenderer);
  }
}
