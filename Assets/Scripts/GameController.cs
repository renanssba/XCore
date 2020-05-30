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
  public bool hideTutorials;



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
      UIController.instance.UpdateUI();

      VsnSaveSystem.SetVariable("hide_tutorials", hideTutorials);
      VsnController.instance.StartVSN("cap0_intro");
      //VsnController.instance.StartVSN("select_daytime_interaction");
    }
  }


  public void Update() {

    // count playtime
    GlobalData.instance.playtime += Time.deltaTime;

    /// CHEAT INPUTS
    //if(!Application.isEditor) {
    //  return;
    //}

    if(Input.GetKeyDown(KeyCode.S)) {
      if(VsnUIManager.instance.skipButton.gameObject.activeSelf) {
        //VsnUIManager.instance.skipButton.OnPointerDown();
      }
    }
    if(Input.GetKeyDown(KeyCode.F4)) {
      SceneManager.LoadScene(StageName.TitleScreen.ToString());
    }
    if(Input.GetKeyDown(KeyCode.F5)) {
      VsnSaveSystem.Load(1);
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    if(Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftShift)) {
      VsnController.instance.StartVSN("debug_menu");
    }
    if (Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.LeftShift)) {
      if(VsnController.instance.state != ExecutionState.PLAYING){
        VsnController.instance.state = ExecutionState.PLAYING;
      } else {
        VsnController.instance.state = ExecutionState.STOPPED;
      }
    }
    if(Input.GetKeyDown(KeyCode.B) && Input.GetKey(KeyCode.LeftShift)) {
      Enemy enemy = BattleController.instance.GetCurrentEnemy();
      if(enemy.hp > 1) {
        BattleController.instance.DamageEnemyHp(enemy.hp - 1);
      } else {
        enemy.HealHP(enemy.maxHp);
      }
    }
    if(Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftShift)) {
      if(BattleController.instance.hp < BattleController.instance.maxHp) {
        BattleController.instance.HealPartyHp(BattleController.instance.maxHp);
      } else {
        BattleController.instance.DamagePartyHp(BattleController.instance.hp-1);
      }
    }
    if(Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftShift)) {
      foreach(Relationship rel in GlobalData.instance.relationships) {
        for(int i=0; i<rel.skilltree.skills.Length; i++) {
          rel.skilltree.skills[i].isUnlocked = true;
        }
      }
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
