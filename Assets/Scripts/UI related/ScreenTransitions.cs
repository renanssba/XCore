﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenTransitions : MonoBehaviour {

  public CanvasGroup canvasGroup;
  public RectTransform myRect;
  public CanvasGroup shadeCanvasGroup;

  public ScreenContext context;

  public static float fadeTime = 0.3f;
  //public static float fadeTime = 1f;


  public void ShowPanel() {
    if(IsInteractable()) {
      return;
    }
    DOTween.Kill(canvasGroup);
    canvasGroup.alpha = 0f;
    gameObject.SetActive(true);
    canvasGroup.interactable = false;
    canvasGroup.DOFade(1f, fadeTime).OnComplete(() => {
      canvasGroup.interactable = true;
    });
    //canvasGroup.interactable = true;
  }

  public void HidePanel() {
    if(!IsOpen() && !DOTween.IsTweening(canvasGroup)) {
      return;
    }
    DOTween.Kill(canvasGroup);
    canvasGroup.interactable = false;
    canvasGroup.DOFade(0f, fadeTime).OnComplete(() => {
      gameObject.SetActive(false);
    });
  }

  public void OpenMenuScreen(){
    if(IsInteractable()) {
      return;
    }
    DOTween.Kill(canvasGroup);
    canvasGroup.alpha = 0f;
    gameObject.SetActive(true);
    canvasGroup.interactable = false;
    myRect.anchoredPosition = new Vector2(0f, -200f);
    FadeInShade(fadeTime);
    myRect.DOAnchorPos(new Vector2(0f, 0), fadeTime);
    canvasGroup.DOFade(1f, fadeTime).OnComplete(() => {
      canvasGroup.interactable = true;
    });
  }

  public bool IsInteractable() {
    return IsOpen() && !DOTween.IsTweening(canvasGroup);
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
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
  }


  public void FadeInShade(float fadeTime) {
    if(shadeCanvasGroup == null) {
      return;
    }
    shadeCanvasGroup.gameObject.SetActive(true);
    shadeCanvasGroup.DOFade(1f, fadeTime);
  }

  public void FadeOutShade(float fadeTime) {
    if(shadeCanvasGroup == null) {
      return;
    }
    shadeCanvasGroup.DOFade(0f, fadeTime).OnComplete(() => {
      shadeCanvasGroup.gameObject.SetActive(false);
    });
  }

  public bool IsOpen(){
    return gameObject.activeSelf && canvasGroup.alpha > 0f;
  }
}
