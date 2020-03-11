using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameController : MonoBehaviour {

  public static GameController instance;

  public ItemSelectorScreen itemSelectorScreen;

  public ParticleGenerator babiesParticleGenerator;
  public Image[] engagementScreenImages;
  public GameObject engagementScreen;



  public void Awake() {
    instance = this;
  }

  public void Start() {
    //VsnAudioManager.instance.PlayMusic("observacao_intro", "observacao_loop");

    if (VsnSaveSystem.GetIntVariable("minigame_ended") == 1) {
      //datingPeopleCards[0].Initialize(GlobalData.instance.people[0]);
      //datingPeopleCards[1].Initialize(GlobalData.instance.people[1]);

      VsnSaveSystem.SetVariable("minigame_ended", 0);
      UIController.instance.UpdateUI();
      VsnController.instance.StartVSN("back_from_minigame");
    } else {
      GlobalData.instance.InitializeChapter();
      GlobalData.instance.PassTime();

      //VsnSaveSystem.SetVariable("observation_played", 1);
      //VsnController.instance.StartVSN("show_people_screen");

      UIController.instance.UpdateUI();

      if(GlobalData.instance.hideTutorials) {
        VsnSaveSystem.SetVariable("tutorial_date", 1);
        VsnSaveSystem.SetVariable("tutorial_date2", 1);
        VsnSaveSystem.SetVariable("tutorial_shop", 1);
        VsnSaveSystem.SetVariable("tutorial_choose_date", 1);
        VsnSaveSystem.SetVariable("tutorial_observation", 1);
      }
      //VsnController.instance.StartVSN("cap0_intro");
      //VsnController.instance.StartVSN("cap1_manha");
      VsnController.instance.StartVSN("cap0_intro");
      //VsnController.instance.StartVSN("cap1_manha");
      //VsnController.instance.StartVSN("tutorial_intro");
    }
  }


  public void Update() {
    if(Input.GetKeyDown(KeyCode.F4) && Application.isEditor){
      SceneManager.LoadScene(StageName.TitleScreen.ToString());
    }
    if(Input.GetKeyDown(KeyCode.F5) && Application.isEditor) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    if(Input.GetKeyDown(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftShift) && Application.isEditor) {
      GlobalData.instance.observedPeople = new Person[] {GlobalData.instance.people[0],
                                                         GlobalData.instance.people[1]};
      VsnController.instance.StartVSNContent("add_exp 100", "custom");
    }
    if(Input.GetKeyDown(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftShift) && Application.isEditor) {
      GlobalData.instance.observedPeople = new Person[] {GlobalData.instance.people[0],
                                                         GlobalData.instance.people[2]};
      VsnController.instance.StartVSNContent("add_exp 100", "custom");
    }
    if(Input.GetKeyDown(KeyCode.Alpha3) && Input.GetKey(KeyCode.LeftShift) && Application.isEditor) {
      GlobalData.instance.observedPeople = new Person[] {GlobalData.instance.people[0],
                                                         GlobalData.instance.people[3]};
      VsnController.instance.StartVSNContent("add_exp 100", "custom");
    }
    if(Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftShift) && Application.isEditor) {
      Relationship rel = GlobalData.instance.GetCurrentRelationship();
      VsnController.instance.StartVSNContent("open_heart_lock "+(rel.heartLocksOpened+1), "custom");
    }
  }


  

  public void ShowEngagementScreen(int babies) {
    VsnAudioManager.instance.PlaySfx("date_success");

    engagementScreenImages[0].sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.CurrentBoy().faceId);
    engagementScreenImages[1].sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.CurrentGirl().faceId);
    babiesParticleGenerator.particlesToGenerate = babies;
    engagementScreen.SetActive(true);
    babiesParticleGenerator.DeleteSons();
    StartCoroutine(ShowEngagementScreenAnimation(5f+babies));
  }

  public IEnumerator ShowEngagementScreenAnimation(float waitTime){
    VsnController.instance.state = ExecutionState.WAITING;
    yield return new WaitForSeconds(waitTime);
    VsnController.instance.state = ExecutionState.PLAYING;
    HideEngagementScreen();
  }

  public void HideEngagementScreen() {
    engagementScreen.SetActive(false);
    babiesParticleGenerator.DeleteSons();
  }
}
