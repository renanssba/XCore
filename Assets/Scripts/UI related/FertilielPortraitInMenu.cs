using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public enum FertilielPortraitExpression {
  intrigued,
  happy,
  sad  
}

public class FertilielPortraitInMenu : MonoBehaviour {
  public Sprite[] sprites;
  public Image baseImage;
  public Image animImage;
  public float crossFadeTime = 0.1f;

  public void BecomeIntrigued() {
    AnimateToSprite(baseImage.sprite, sprites[(int)FertilielPortraitExpression.intrigued]);
  }

  public void BecomeHappy() {
    AnimateToSprite(baseImage.sprite, sprites[(int)FertilielPortraitExpression.happy]);
  }

  public void BecomeSad() {
    AnimateToSprite(baseImage.sprite, sprites[(int)FertilielPortraitExpression.sad]);
  }

  public void AnimateToSprite(Sprite old, Sprite final) {
    baseImage.sprite = final;
    animImage.sprite = old;

    DOTween.Kill(baseImage);
    DOTween.Kill(animImage);

    baseImage.color = new Color(1f, 1f, 1f, 0f);
    animImage.color = Color.white;

    baseImage.DOFade(1f, crossFadeTime);
    animImage.DOFade(0f, crossFadeTime);
  }
}
