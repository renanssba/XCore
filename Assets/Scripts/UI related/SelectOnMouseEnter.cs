using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectOnMouseEnter : MonoBehaviour {

  public void Select(){
    Utils.SelectUiElement(gameObject);
  }
}
