using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Particle : MonoBehaviour {

  public Image image;
  public Vector3 initialSpeed;
  public RectTransform position;
  public float rotateSpeed;
  public Vector3 gravity;

  public float timeToStartFade = -1;
  public float fadeTime;

  private Vector3 speed;


  public void Start(){
    speed = initialSpeed;
    if(timeToStartFade > 0f) {
      StartCoroutine(WaitToFade());
    }
    position.localPosition = new Vector3(position.localPosition.x, position.localPosition.y, 0f);
  }

  void Update(){
    speed = speed - gravity * Time.deltaTime;

    position.anchoredPosition = position.anchoredPosition + (Vector2)speed;
    position.Rotate(new Vector3(0f, 0f, rotateSpeed) * Time.deltaTime);
  }

  public void Rotate(float angle){
    float speedMagnitude = initialSpeed.magnitude;

    initialSpeed = initialSpeed + new Vector3(Random.Range(-speedMagnitude*0.6f, speedMagnitude*0.6f), 0f, 0f);
    initialSpeed.Normalize();
    initialSpeed = initialSpeed * speedMagnitude;
  }

  public IEnumerator WaitToFade() {
    yield return new WaitForSeconds(timeToStartFade);
    image.DOFade(0f, fadeTime);
  }
}
