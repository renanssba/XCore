using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

  private void Awake() {
    if(VsnSaveSystem.GetStringVariable("language")=="") {
      VsnSaveSystem.SetVariable("language", "eng");
    }
  }

  void Start(){
    VsnAudioManager.instance.PlayMusic("observacao_intro", "observacao_loop");
  }

  public void ClickNewGame(){
    //GlobalData.GetInstance().InitializeGameData();
    SceneManager.LoadScene(StageName.Gameplay.ToString());
  }

  public void ClickContinue(){
//    SceneManager.LoadScene(StageName.CityMap.ToString());
    //SoundManager.GetInstance().PlayCancelSound();
    VsnController.instance.StartVSN("title_screen");
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
    Application.Quit();
  }
}
