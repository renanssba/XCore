using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

  //public GameObject[] buttons;
  public ScreenTransitions loadPanel;


  void Start(){
    VsnAudioManager.instance.PlayMusic("observacao_intro", "observacao_loop");
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

  public void ClickPortugueseButton(){
    VsnAudioManager.instance.PlaySfx("ui_confirm");
    Lean.Localization.LeanLocalization.CurrentLanguage = "Portuguese";
    JoystickController.instance.SelectStartingObject();
  }

  public void ClickEnglishButton() {
    VsnAudioManager.instance.PlaySfx("ui_confirm");
    Lean.Localization.LeanLocalization.CurrentLanguage = "English";
    JoystickController.instance.SelectStartingObject();
  }

  public void ClickExit(){
    VsnAudioManager.instance.PlaySfx("ui_confirm");
    Application.Quit();
  }
}
