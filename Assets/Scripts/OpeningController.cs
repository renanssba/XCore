using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class OpeningController : MonoBehaviour {

  public VideoPlayer player;

  private bool canAdvanceScene = false;


  void Awake(){
    canAdvanceScene = false;
    StartCoroutine(WaitToLoadScene(1f));
  }

  //private void Start() {
  //  VsnController.instance.StartVSN("cap0");
  //}


  void Update() {
    if(!player.isPlaying && canAdvanceScene){
      GotoTitle();
    }

    if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
      GotoTitle();
    }
	}

  IEnumerator WaitToLoadScene(float time){
    yield return new WaitForSeconds(time);
    canAdvanceScene = true;
  }

  void GotoTitle(){
    VsnController.instance.StartVSN("goto title");
  }
}
