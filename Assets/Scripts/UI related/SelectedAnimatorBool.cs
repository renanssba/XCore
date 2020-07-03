using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedAnimatorBool : MonoBehaviour {

  public GameObject selectedObject;
  public bool boolValue = true;
  public Animator animator;
  public string variableName;


  void OnEnable() {
    animator = GetComponent<Animator>();
    CheckIfShouldShow();
    StartCoroutine(KeepChecking());
  }

  public IEnumerator KeepChecking() {
    while(true) {
      yield return new WaitForSeconds(0.05f);
      CheckIfShouldShow();
    }
  }

  public void CheckIfShouldShow() {
    if(EventSystem.current.currentSelectedGameObject == selectedObject) {
      animator.SetBool(variableName, boolValue);
    } else {
      animator.SetBool(variableName, !boolValue);
    }
  }
}
