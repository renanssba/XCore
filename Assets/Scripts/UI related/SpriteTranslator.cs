using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpriteTranslator : MonoBehaviour {

  public Sprite englishTranslatedSprite;

  void Update() {
    if (VsnSaveSystem.GetStringVariable("language") == "eng") {
      Image img = GetComponent<Image>();
      Text t2 = GetComponent<Text>();
      if (img != null) {
        img.sprite = englishTranslatedSprite;
      }
    }
  }
}
