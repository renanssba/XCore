using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Tut_Blinking : MonoBehaviour {

  public float blinkingTime;
  public float minAlpha =0f;
  public float maxAlpha =1f;

  void OnEnable(){
    StartCoroutine(SmoothBlink());
  }

  IEnumerator SmoothBlink(){
    SetAlpha(minAlpha);
    while(true) {
      GetComponent<Image>().DOFade(maxAlpha, blinkingTime/2f).SetUpdate(true);
      yield return new WaitForSecondsRealtime(blinkingTime/2f);
      GetComponent<Image>().DOFade(minAlpha, blinkingTime/2f).SetUpdate(true);
      yield return new WaitForSecondsRealtime(blinkingTime/2f);
    }
  }


  void SetAlpha(float alpha){
    Color c = GetComponent<Image>().color;
    c.a = alpha;
    GetComponent<Image>().color = c;
  }
}
