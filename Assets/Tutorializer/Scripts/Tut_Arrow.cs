using UnityEngine;
using System.Collections;

public class Tut_Arrow : MonoBehaviour {

  public RectTransform finalRect;
  public float animDuration;
  public float waitTime;

  private bool hasStarted = false;


  private Vector2 initialPos;
  private Vector2 finalPos;

  public void OnEnable() {
    finalRect.gameObject.SetActive(false);
    initialPos = GetComponent<RectTransform>().anchoredPosition;
    finalPos = finalRect.anchoredPosition;

    Initialize();
	}

  public void Update(){
    if(!hasStarted){
      hasStarted = true;
      Initialize();
    }
  }

  public void Initialize(){
    StartCoroutine(GoToEnd());
  }

  IEnumerator GoToEnd(){
    yield return StartCoroutine(SmoothMove(initialPos, finalPos, animDuration));

    if(waitTime!=0f){
      yield return StartCoroutine(Wait());
    }

    StartCoroutine(GoBackToStart());
  }


  IEnumerator SmoothMove(Vector2 startpos, Vector2 endpos, float seconds) {
    float t = 0f;
    while (t <= 1f) {
      t += Time.deltaTime/seconds;
      GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
      yield return null;
    }
    hasStarted = true;
  }


  IEnumerator Wait(){
    yield return new WaitForSeconds(waitTime);
  }

  IEnumerator GoBackToStart(){
    yield return StartCoroutine(SmoothMove(finalPos, initialPos, animDuration));

    if(waitTime!=0f){
      yield return StartCoroutine(Wait());
    }

    StartCoroutine(GoToEnd());
  }
}
