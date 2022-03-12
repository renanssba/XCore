using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;


public class RoutineController : MonoBehaviour {

  public static RoutineController instance;

  public bool skipIntro;


  public void Awake() {
    instance = this;
  }

  public void Start() {
    GlobalData.instance.InitializePilots();

    if(GlobalData.instance.saveToLoad != -1) {
      // LOAD DATA FROM SAVE
      VsnSaveSystem.Load(GlobalData.instance.saveToLoad);
      GlobalData.instance.LoadPersistantGlobalData();
      GlobalData.instance.saveToLoad = -1;
    }

    UIController.instance.UpdateUI();

    if(skipIntro) {
      //VsnSaveSystem.SetVariable("day", 2);
      //VsnSaveSystem.SetVariable("hide_tutorials", skipIntro);
      //VsnController.instance.StartVSN("cap1_dia1");
      VsnController.instance.StartVSN("select_daytime_interaction");
    } else {
      VsnController.instance.StartVSN("cap0");
    }
  }


  public void Update() {
    // count playtime
    GlobalData.instance.playtime += Time.deltaTime;

    /// CHEAT INPUTS
    if(!Application.isEditor) {
      return;
    }

    if(Input.GetKeyDown(KeyCode.S)) {
      if(VsnUIManager.instance.skipButton.gameObject.activeSelf) {
        //VsnUIManager.instance.skipButton.OnPointerDown();
      }
    }
    if(Input.GetKeyDown(KeyCode.F4)) {
      SceneManager.LoadScene(StageName.TitleScreen.ToString());
    }
    if(Input.GetKeyDown(KeyCode.F5)) {
      VsnSaveSystem.CleanAllData();
      //VsnSaveSystem.Load(1);
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    if(Input.GetKeyDown(KeyCode.F6) && Application.isEditor) {
      AchievementsController.ResetAllAchievements();
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
    //if(Input.GetKeyDown(KeyCode.B) && Input.GetKey(KeyCode.LeftShift)) {
    //  Enemy enemy = BattleController.instance.GetCurrentEnemy();
    //  if(enemy.hp > 1) {
    //    BattleController.instance.DamageEnemyHp(enemy.hp - 1);
    //  } else {
    //    enemy.HealHP(enemy.AttributeValue((int)Attributes.maxHp));
    //  }
    //}
    //if(Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftShift)) {
    //  if(BattleController.instance.hp < BattleController.instance.maxHp) {
    //    BattleController.instance.HealPartyHp(BattleController.instance.maxHp);
    //  } else {
    //    BattleController.instance.DamagePartyHp(BattleController.instance.hp-1);
    //  }
    //}
    if(Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftShift)) {
      foreach(Relationship rel in GlobalData.instance.relationships) {
        for(int i=0; i<rel.skilltree.skills.Length; i++) {
          rel.skilltree.skills[i].isUnlocked = true;
        }
      }
    }
  }

}
