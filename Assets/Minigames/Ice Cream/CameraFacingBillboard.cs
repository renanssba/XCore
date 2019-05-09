using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour{
  public Camera currentCamera;

  public void Awake(){
    currentCamera = Camera.main;
  }

  void LateUpdate(){
    transform.LookAt(transform.position + currentCamera.transform.rotation * Vector3.forward,
                     currentCamera.transform.rotation * Vector3.up);
  }
}