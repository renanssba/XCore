using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedDecoration : MonoBehaviour {

  public GameObject selectedObject;
  public bool invert = false;
  CanvasGroup group;

	void OnEnable() {
    group = GetComponent<CanvasGroup>();
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
      group.alpha = !invert ? 1f : 0f;
    } else {
      group.alpha = !invert ? 0f : 1f;
    }
  }
}
