using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowIfLanguage : MonoBehaviour {

  public string language;
  Image image;

  public void Start() {
    image = GetComponent<Image>();
    StartCoroutine(Check());
  }

  public IEnumerator Check() {
    while(true) {
      yield return new WaitForSeconds(0.1f);
      if(Lean.Localization.LeanLocalization.CurrentLanguage == language) {
        image.color = Color.white;
      } else {
        image.color = Color.clear;
      }
    }
  }
  
}
