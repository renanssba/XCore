using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTranslator : MonoBehaviour {

  public string englishTranslatedText;

  void Update () {
    if(VsnSaveSystem.GetStringVariable("language") == "eng"){
      TextMeshProUGUI t1 = GetComponent<TextMeshProUGUI>();
      Text t2 = GetComponent<Text>();
      if(t1 != null) {
        t1.text = englishTranslatedText;
      }
      if(t2 != null) {
        t2.text = englishTranslatedText;
      }
    }
	}
}
