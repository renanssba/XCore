using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTranslator : MonoBehaviour {

  public string englishTranslatedText;

  void Update () {
    if(VsnSaveSystem.GetStringVariable("language") == "eng"){
      GetComponent<TextMeshProUGUI>().text = englishTranslatedText;
    }
	}
}
