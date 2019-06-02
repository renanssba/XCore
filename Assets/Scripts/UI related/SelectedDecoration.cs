using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedDecoration : MonoBehaviour {

  public GameObject selectedObject;
  CanvasGroup group;

	void Start () {
    group = GetComponent<CanvasGroup>();
	}
	
	void Update () {
		if(EventSystem.current.currentSelectedGameObject == selectedObject) {
      group.alpha = 1f;
    }else{
      group.alpha = 0f;
    }
	}
}
