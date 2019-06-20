using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedDecoration : MonoBehaviour {

  public GameObject selectedObject;
  CanvasGroup group;

	void Start () {
    group = GetComponent<CanvasGroup>();
    StartCoroutine(Check());
  }

  public IEnumerator Check() {
    while(true) {
      yield return new WaitForSeconds(0.1f);
      if(EventSystem.current.currentSelectedGameObject == selectedObject) {
        group.alpha = 1f;
      } else {
        group.alpha = 0f;
      }
    }
  }
}
