using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

  private void Awake() {
    VsnSaveSystem.SetVariable("language", "eng");
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

  public void ClickExit(){
    Application.Quit();
  }
}
