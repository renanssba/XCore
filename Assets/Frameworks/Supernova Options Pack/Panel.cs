using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Panel : MonoBehaviour {

  public CanvasGroup canvasGroup;
  public RectTransform myRect;
  public CanvasGroup shadeCanvasGroup;

  public static float fadeTime = 0.3f;


  public void ShowPanel() {
    if(IsInteractable()) {
      return;
    }
    PreShowPanel();
    DOTween.Kill(canvasGroup);
    canvasGroup.alpha = 0f;
    gameObject.SetActive(true);
    canvasGroup.interactable = false;
    canvasGroup.DOFade(1f, fadeTime).SetUpdate(true).OnComplete(() => {
      canvasGroup.interactable = true;
      PosShowPanel();
    });
    //canvasGroup.interactable = true;
  }


  public virtual void PreShowPanel() { }
  public virtual void PosShowPanel() { }


  public void HidePanel() {
    PreHidePanel();
    if(!IsOpen() && !DOTween.IsTweening(canvasGroup)) {
      return;
    }
    DOTween.Kill(canvasGroup);
    canvasGroup.interactable = false;
    canvasGroup.DOFade(0f, fadeTime).SetUpdate(true).OnComplete(() => {
      gameObject.SetActive(false);
      PosHidePanel();
    });
  }


  public virtual void PreHidePanel() { }
  public virtual void PosHidePanel() { }

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
    myRect.DOAnchorPos(new Vector2(0f, 0), fadeTime).SetUpdate(true);
    canvasGroup.DOFade(1f, fadeTime).SetUpdate(true).OnComplete(() => {
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
    canvasGroup.DOFade(0f, fadeTime).SetUpdate(true);
    myRect.DOAnchorPos(new Vector2(0f, -200f), fadeTime).SetRelative(true).SetUpdate(true).OnComplete( ()=>{
      myRect.DOAnchorPos(new Vector2(0f, 0f), 0f).SetRelative(true).SetUpdate(true);
      gameObject.SetActive(false);
    } );
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
  }


  public void FadeInShade(float fadeTime) {
    if(shadeCanvasGroup == null) {
      return;
    }
    shadeCanvasGroup.gameObject.SetActive(true);
    shadeCanvasGroup.DOFade(1f, fadeTime).SetUpdate(true);
  }

  public void FadeOutShade(float fadeTime) {
    if(shadeCanvasGroup == null) {
      return;
    }
    shadeCanvasGroup.DOFade(0f, fadeTime).SetUpdate(true).OnComplete(() => {
      shadeCanvasGroup.gameObject.SetActive(false);
    });
  }

  public bool IsOpen(){
    return gameObject.activeSelf && canvasGroup.alpha > 0f;
  }
}
