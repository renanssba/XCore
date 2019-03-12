using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

  public static GameController instance;
  public PersonCard[] couple;


  public void Awake() {
    instance = this;
  }

  public void Start() {
    GlobalData.instance.InitializeChapter();
    UpdateUI();
  }

  public void Update() {
    if(Input.GetKeyDown(KeyCode.F5)){
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
  }

  public void UpdateUI() {
    int coupleId = GlobalData.instance.currentCouple;
    couple[0].Initialize(GlobalData.instance.people[coupleId * 2]);
    couple[1].Initialize(GlobalData.instance.people[coupleId * 2 + 1]);
  }

  public void ClickSelectNewCouple(){
    GlobalData.instance.SelectNewCouple();
  }
}
