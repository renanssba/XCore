using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenTransitions : MonoBehaviour {

  public CanvasGroup canvasGroup;
  public RectTransform myRect;
  public CanvasGroup shadeCanvasGroup;

  public static float fadeTime = 0.3f;


  public void ShowPanel() {
    if(IsOpen() && !DOTween.IsTweening(canvasGroup)) {
      return;
    }
    DOTween.Kill(canvasGroup);
    canvasGroup.alpha = 0f;
    gameObject.SetActive(true);
    canvasGroup.DOFade(1f, fadeTime);
  }

  public void HidePanel() {
    if(!IsOpen() && !DOTween.IsTweening(canvasGroup)) {
      return;
    }
    DOTween.Kill(canvasGroup);
    canvasGroup.DOFade(0f, fadeTime).OnComplete(() => {
      gameObject.SetActive(false);
    });
  }

  public void OpenMenuScreen(){
    if(IsOpen() && !DOTween.IsTweening(canvasGroup)) {
      return;
    }
    DOTween.Kill(canvasGroup);
    myRect.DOAnchorPos(new Vector2(0f, -200f), 0f).OnComplete( ()=>{
      canvasGroup.alpha = 0f;
      FadeInShade(fadeTime);
      canvasGroup.DOFade(1f, fadeTime);
      myRect.DOAnchorPos(new Vector2(0f, 0), fadeTime);
      gameObject.SetActive(true);
    } );
  }

  public void CloseMenuScreen(){
    if(!IsOpen() && !DOTween.IsTweening(canvasGroup)) {
      return;
    }
    DOTween.Kill(canvasGroup);
    FadeOutShade(fadeTime);
    canvasGroup.DOFade(0f, fadeTime);
    myRect.DOAnchorPos(new Vector2(0f, -200f), fadeTime).SetRelative(true).OnComplete( ()=>{
      myRect.DOAnchorPos(new Vector2(0f, 0f), 0f).SetRelative(true);
      gameObject.SetActive(false);
    } );
  }


  public void FadeInShade(float fadeTime) {
    shadeCanvasGroup.gameObject.SetActive(true);
    shadeCanvasGroup.DOFade(1f, fadeTime);
  }

  public void FadeOutShade(float fadeTime) {
    shadeCanvasGroup.DOFade(0f, fadeTime).OnComplete(() => {
      shadeCanvasGroup.gameObject.SetActive(false);
    });
  }

  public bool IsOpen(){
    return gameObject.activeSelf && canvasGroup.alpha > 0f;
  }
}
