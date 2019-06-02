using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedObjectSfx : MonoBehaviour {

  [HideInInspector]
  public GameObject previousSelected;
  public bool preventNoElementSelected = false;

	void Start () {
    previousSelected = EventSystem.current.currentSelectedGameObject;
  }

	
	void Update () {
    // prevent from having no UI element selected
    if(preventNoElementSelected && EventSystem.current.currentSelectedGameObject == null) {
      Utils.SelectUiElement(previousSelected);
    }

		if(previousSelected != EventSystem.current.currentSelectedGameObject &&
       EventSystem.current.currentSelectedGameObject != null) {
      SfxManager.StaticPlaySelectSfx();
      previousSelected = EventSystem.current.currentSelectedGameObject;
    }
	}
}
