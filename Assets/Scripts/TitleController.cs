using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

  //public GameObject[] buttons;
  public ScreenTransitions loadPanel;
  public ScreenTransitions languageSelectPanel;


  void Start(){
    VsnSaveSystem.CleanAllData();
    VsnAudioManager.instance.PlayMusic("observacao_intro", "observacao_loop");
    //if(PlayerPrefs.GetInt("initialized_language", 0) == 1) {
    //  // then language is already initialized
    //  languageSelectPanel.gameObject.SetActive(false);
    //} else {
    //  // then language is not initialized, show language select panel
    //  languageSelectPanel.gameObject.SetActive(true);
    //}
    languageSelectPanel.gameObject.SetActive(true); //DEBUG, while there's no Options screen to select language
  }

  public void CloseLanguageSelectPanel() {
    languageSelectPanel.HidePanel();
  }


  public void ClickNewGame(){
    GlobalData.instance.saveToLoad = -1;
    VsnController.instance.StartVSN("start_game");
  }

  public void ClickContinue(){
    SfxManager.StaticPlayConfirmSfx();
    loadPanel.ShowPanel();
  }

  public void ClickCloseContinueScreen() {
    SfxManager.StaticPlayCancelSfx();
    loadPanel.HidePanel();
  }

  public void ClickToggleLanguageButton() {
    if(Lean.Localization.LeanLocalization.CurrentLanguage == "Portuguese") {
      ClickEnglishButton();
    } else {
      ClickPortugueseButton();
    }
  }

  public void ClickPortugueseButton() {
    SfxManager.StaticPlayBigConfirmSfx();
    Lean.Localization.LeanLocalization.CurrentLanguage = "Portuguese";
    JoystickController.instance.SelectStartingObject();
    PlayerPrefs.SetInt("initialized_language", 1);
    CloseLanguageSelectPanel();
  }

  public void ClickEnglishButton() {
    SfxManager.StaticPlayBigConfirmSfx();
    Lean.Localization.LeanLocalization.CurrentLanguage = "English";
    JoystickController.instance.SelectStartingObject();
    PlayerPrefs.SetInt("initialized_language", 1);
    CloseLanguageSelectPanel();
  }

  public void ClickLinkButton(GameObject newObj) {
    string urlToLoad = newObj.GetComponent<Link>().url;
    if(Application.platform == RuntimePlatform.WebGLPlayer) {
      return;
    }
    Application.OpenURL(urlToLoad);
  }

  public void ClickExit(){
    VsnAudioManager.instance.PlaySfx("ui_confirm");
    Application.Quit();
  }
}
