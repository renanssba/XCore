using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

  private void Awake() {
    if(VsnSaveSystem.GetStringVariable("language")=="") {
      VsnSaveSystem.SetVariable("language", "pt_br");
    }
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
    VsnSaveSystem.SetVariable("language", "pt_br");
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void ClickEnglishButton() {
    VsnSaveSystem.SetVariable("language", "eng");
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void ClickExit(){
    VsnAudioManager.instance.PlaySfx("ui_confirm");
    Application.Quit();
  }
}
