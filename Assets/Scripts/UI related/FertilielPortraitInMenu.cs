using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public enum FertilielPortraitExpression {
  intrigued,
  happy,
  sad,
  none
}


public class FertilielPortraitInMenu : MonoBehaviour {
  public Sprite[] sprites;
  public Image baseImage;
  public Image animImage;
  public float crossFadeTime = 0.1f;
  public float minExpressionTime = 1f;
  public float changeExpressionDelay;

  public GameObject sighPrefab;

  public FertilielPortraitExpression currentExpression = FertilielPortraitExpression.intrigued;
  public FertilielPortraitExpression nextExpression;


  public void BecomeIntrigued() {
    nextExpression = FertilielPortraitExpression.intrigued;
    changeExpressionDelay = minExpressionTime;
  }

  public void BecomeHappy() {
    nextExpression = FertilielPortraitExpression.happy;
    changeExpressionDelay = 0f;
  }

  public void BecomeSad() {
    nextExpression = FertilielPortraitExpression.sad;
    changeExpressionDelay = 0f;
  }

  public void Update() {
    if(changeExpressionDelay > 0f) {
      changeExpressionDelay -= Time.unscaledDeltaTime;
      
    }

    if(changeExpressionDelay <= 0f && currentExpression != nextExpression) {
      AnimateToSprite(nextExpression);
      changeExpressionDelay = minExpressionTime;
    }
  }

  public void AnimateToSprite(FertilielPortraitExpression newExpression) {
    Sprite old = baseImage.sprite;
    Sprite final = sprites[(int)newExpression];
    baseImage.sprite = final;
    animImage.sprite = old;

    DOTween.Kill(baseImage);
    DOTween.Kill(animImage);

    baseImage.color = new Color(1f, 1f, 1f, 0f);
    animImage.color = Color.white;

    baseImage.DOFade(1f, crossFadeTime);
    animImage.DOFade(0f, crossFadeTime);

    if(newExpression == FertilielPortraitExpression.sad) {
      Instantiate(sighPrefab, transform);
    }

    currentExpression = newExpression;
  }
}
