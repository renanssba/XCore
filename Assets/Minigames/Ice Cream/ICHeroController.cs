using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICHeroController : MonoBehaviour {

  public Rigidbody body;
  public float walkingSpeed;
  public float runningSpeed;
  float baseWalkingSpeed;

  void Awake() {
    baseWalkingSpeed = walkingSpeed;
  }


  void Update () {
    SetWalkingSpeed();

    /// If is moving, change facing
    if(body.velocity.sqrMagnitude > 0f) {
      ChangeFacingDirection();
    }

    if (Input.GetButtonDown("Jump")) {
      ICGameController.instance.ClickInteraction();     
    }
    if (Input.GetAxis("Fire1") > 0f){
      //SfxManager.StaticPlaySelectSfx();
      //Debug.Log("Running...");
      walkingSpeed = runningSpeed;
    }else{
      //Debug.Log("Walking...");
      walkingSpeed = baseWalkingSpeed;
    }
  }

  public void SetWalkingSpeed(){
    float hor = Input.GetAxis("Horizontal");
    float ver = Input.GetAxis("Vertical");

    //Debug.Log("HOR: " + hor + ",VER: " + ver);

    if (hor > 0f) {
      body.velocity = new Vector3(walkingSpeed, 0f, body.velocity.z);
    } else if (hor < 0f) {
      body.velocity = new Vector3(-walkingSpeed, 0f, body.velocity.z);
    } else {
      body.velocity = new Vector3(0f, 0f, body.velocity.z);
    }

    if (ver > 0f) {
      body.velocity = new Vector3(body.velocity.x, 0f, walkingSpeed);
    } else if (ver < 0f) {
      body.velocity = new Vector3(body.velocity.x, 0f, -walkingSpeed);
    } else {
      body.velocity = new Vector3(body.velocity.x, 0f, 0f);
    }
  }

  public void ChangeFacingDirection(){
    float angle = Vector3.Angle(Vector3.back, body.velocity);
    if(body.velocity.x > 0f) {
      angle = -angle;
    }

    transform.eulerAngles = new Vector3(0f, angle, 0f);
  }
}
