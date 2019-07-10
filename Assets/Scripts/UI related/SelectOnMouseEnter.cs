using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnMouseEnter : MonoBehaviour {

  public void Select(){
    if(EventSystem.current.currentSelectedGameObject != gameObject) {
      SfxManager.StaticPlaySelectSfx();
      Utils.SelectUiElement(gameObject);
    }
  }
}
