using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

  [Header("- Panels -")]
  public Panel loadPanel;
  public Panel languageSelectPanel;
  public Panel buttonsPanel;
  public Panel optionsPanel;

  [Header("- Elements -")]
  public GameObject[] languageButtons;
  public Transform logoImage;


  void Start(){
    VsnSaveSystem.CleanAllData();
    VsnAudioManager.instance.PlayMusic("", "XCore1 Opening");
    if(PlayerPrefs.GetInt("initialized_language", 0) == 1) {
      languageSelectPanel.gameObject.SetActive(false);
      Time.timeScale = 1f;
      StartCoroutine(LogoAnimation());
    } else {
      // if language is not initialized, show language select panel
      languageSelectPanel.gameObject.SetActive(true);
    }
  }

  public IEnumerator LogoAnimation() {
    yield return new WaitForSeconds(0.1f);
    logoImage.transform.AnimTweenPopAppear();

    yield return new WaitForSeconds(1f);
    buttonsPanel.ShowPanel();
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

  public void ClickOptionsButton() {
    optionsPanel.ShowPanel();
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
