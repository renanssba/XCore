using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Particle : MonoBehaviour {

  public Vector2 initialSpeed;
  public RectTransform position;
  public float rotateSpeed;
  public Vector2 gravity;

  private Vector2 speed;


  public void Start(){
    speed = initialSpeed;
  }

  void Update(){
    speed = speed - gravity * Time.deltaTime;

    position.anchoredPosition = position.anchoredPosition + speed;
    position.Rotate(new Vector3(0f, 0f, rotateSpeed) * Time.deltaTime);
  }

  public void Rotate(float angle){
    float speedMagnitude = initialSpeed.magnitude;

    initialSpeed = initialSpeed + new Vector2(Random.Range(-speedMagnitude*0.6f, speedMagnitude*0.6f), 0f);
    initialSpeed.Normalize();
    initialSpeed = initialSpeed * speedMagnitude;
  }
}
