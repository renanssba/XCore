using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Tut_Sliding_Finger : MonoBehaviour {

  public Sprite[] fingerSprites;
  public float moveTime;
  public float waitTime;
  public Image fingerImage;

  public Transform[] waypoints;


  public void OnEnable(){
    transform.parent.gameObject.SetActive(true);

    Debug.LogWarning("Starting tutorial sliding finger");

    StartCoroutine(Advance());
  }


  IEnumerator Advance(){
    GoToStart();
    yield return new WaitForSecondsRealtime(waitTime);

    SetFingerSprite(1);
    yield return new WaitForSecondsRealtime(waitTime/2f);

    for(int i=1; i<waypoints.Length; i++){
      transform.DOMove(waypoints[i].position, moveTime).SetUpdate(true);
      yield return new WaitForSecondsRealtime(moveTime);
    }

    SetFingerSprite(0);
    yield return new WaitForSecondsRealtime(waitTime);

    StartCoroutine(Advance());
  }

  void GoToStart(){
    transform.position = waypoints[0].position;
  }

  void SetFingerSprite(int sprite){
    fingerImage.sprite = fingerSprites[sprite];
  }
}
