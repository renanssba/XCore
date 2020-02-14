using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Tut_Blinking : MonoBehaviour {

  public float blinkingTime;
  public float minAlpha =0f;
  public float maxAlpha =1f;

  private Image image;
  private SpriteRenderer renderer;

  void OnEnable(){
    image = GetComponent<Image>();
    renderer = GetComponent<SpriteRenderer>();
    StartCoroutine(SmoothBlink());
  }

  IEnumerator SmoothBlink(){
    SetAlpha(minAlpha);
    while(true) {
      if(image != null) {
        image.DOFade(maxAlpha, blinkingTime / 2f).SetUpdate(true);
      }
      if(renderer != null) {
        renderer.DOFade(maxAlpha, blinkingTime / 2f).SetUpdate(true);
      }

      yield return new WaitForSecondsRealtime(blinkingTime / 2f);

      if(image != null) {
        image.DOFade(minAlpha, blinkingTime / 2f).SetUpdate(true);
      }
      if(renderer != null) {
        renderer.DOFade(minAlpha, blinkingTime / 2f).SetUpdate(true);
      }

      yield return new WaitForSecondsRealtime(blinkingTime / 2f);
    }
  }


  void SetAlpha(float alpha){
    Color c;
    if(image != null) {
      c = image.color;
      c.a = alpha;
      image.color = c;
    }
    if(renderer != null) {
      c = renderer.color;
      c.a = alpha;
      renderer.color = c;
    }
  }
}
