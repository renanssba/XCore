using UnityEngine;
using System.Collections;
using TMPro;

public class VsnConsoleSimulator : MonoBehaviour {
  public TMP_Text consoleText;

  int totalCharacters;
  public bool autopass = false;

  public voidFunctionDelegate callAfterShowCharacters;
  public delegate void voidFunctionDelegate();

  public float elapsedTime;
  int numberOfCharsToShow;
  float lastPlayedSfx;

  Coroutine showLettersCoroutine = null;



  public void SetAutoPassText(){
    autopass = true;
  }

  public void StartShowingCharacters(){
    showLettersCoroutine = StartCoroutine(ShowCharactersFromTheStart());
  }


  public void FinishShowingCharacters(){
    consoleText.maxVisibleCharacters = totalCharacters;
    consoleText.ForceMeshUpdate();
    if(showLettersCoroutine != null){
      StopCoroutine(showLettersCoroutine);
    }
    showLettersCoroutine = null;
    Debug.LogWarning("Finished Showing Characters. Autopass is "+autopass);

    if(callAfterShowCharacters != null) {
      callAfterShowCharacters();
    }
  }

  public void UpdateText() {
    TMP_TextInfo textInfo = consoleText.textInfo;

    consoleText.ForceMeshUpdate();
    totalCharacters = textInfo.characterCount;
  }

  public IEnumerator ShowCharactersFromTheStart() {
    VsnUIManager.instance.isTextAppearing = true;
    elapsedTime = 0;
    numberOfCharsToShow = 0;
    lastPlayedSfx = 0f;

    UpdateText();
    while(numberOfCharsToShow < totalCharacters) {
      consoleText.maxVisibleCharacters = numberOfCharsToShow;

      elapsedTime += Time.unscaledDeltaTime;
      numberOfCharsToShow = (int)(elapsedTime * VsnUIManager.instance.charsToShowPerSecond);
      if(elapsedTime - lastPlayedSfx > VsnAudioManager.instance.dialogSfxTime){
        lastPlayedSfx = elapsedTime;
        VsnAudioManager.instance.PlayDialogSfx();
      }
      yield return null;
    }
    FinishShowingCharacters();
  }

  public IEnumerator ShowCharactersFromPoint(int minCharsToShow) {
    VsnUIManager.instance.isTextAppearing = true;
    elapsedTime = 0;
    numberOfCharsToShow = minCharsToShow;
    lastPlayedSfx = 0f;

    UpdateText();
    Debug.LogWarning("number of chars to show: " + numberOfCharsToShow + ", new length (totalCharacters): " + totalCharacters);
    while(numberOfCharsToShow < totalCharacters) {
      Debug.LogWarning("INSIDE LOOP. Showing "+numberOfCharsToShow+" chars. String: "+consoleText.text);
      consoleText.maxVisibleCharacters = numberOfCharsToShow;

      elapsedTime += Time.unscaledDeltaTime;
      numberOfCharsToShow = minCharsToShow + (int)(elapsedTime * VsnUIManager.instance.charsToShowPerSecond);
      if(elapsedTime - lastPlayedSfx > VsnAudioManager.instance.dialogSfxTime){
        lastPlayedSfx = elapsedTime;
        VsnAudioManager.instance.PlayDialogSfx();
      }
      yield return null;
    }
    FinishShowingCharacters();
  }
}
