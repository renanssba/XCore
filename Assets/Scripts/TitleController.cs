using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

  public GameObject[] buttons;

  private void Awake() {
    Utils.SelectUiElement(buttons[0]);
    //if(VsnSaveSystem.GetStringVariable("language")=="") {
      VsnSaveSystem.SetVariable("language", "pt_br");
    //}
  }

  void Start(){
    VsnAudioManager.instance.PlayMusic("observacao_intro", "observacao_loop");
  }

  public void ClickNewGame(){
    //GlobalData.GetInstance().InitializeGameData();
    VsnController.instance.StartVSN("start_game");
  }

  public void ClickContinue(){
    //    SceneManager.LoadScene(StageName.CityMap.ToString());
    VsnAudioManager.instance.PlaySfx("ui_confirm");
    /// TODO: Implement save/load feature
  }

  public void ClickPortugueseButton(){
    VsnAudioManager.instance.PlaySfx("ui_confirm");
    Lean.Localization.LeanLocalization.CurrentLanguage = "Portuguese";
  }

  public void ClickEnglishButton() {
    VsnAudioManager.instance.PlaySfx("ui_confirm");
    Lean.Localization.LeanLocalization.CurrentLanguage = "English";
  }

  public void ClickExit(){
    VsnAudioManager.instance.PlaySfx("ui_confirm");
    Application.Quit();
  }
}
