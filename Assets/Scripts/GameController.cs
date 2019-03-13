using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour {

  public static GameController instance;
  public PersonCard[] couple;
  public TextMeshProUGUI dayText;
  public TextMeshProUGUI apText;
  
  public int maxAp;
  public int ap;

  public int day;
  public int maxDays;

  public Event[] date;
  public int currentDateEvent;



  public void Awake() {
    instance = this;
  }

  public void Start() {
    GlobalData.instance.InitializeChapter();
    Initialize();
    UpdateUI();
  }

  public void Initialize(){
    day = 0;
    PassDay();
  }

  public void PassDay(){
    ap = maxAp;
    day++;
    UpdateUI();
  }

  public void Update() {
    if(Input.GetKeyDown(KeyCode.F5)){
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
  }


  public bool SpendAP(int cost){
    if(ap < cost){
      return false;
    }  
    ap -= cost;
    return true;
  }
  

  public void UpdateUI() {
    int coupleId = GlobalData.instance.currentCouple;
    couple[0].Initialize(GlobalData.instance.people[coupleId * 2]);
    couple[1].Initialize(GlobalData.instance.people[coupleId * 2 + 1]);
    dayText.text = "Dia: " + day + "/" + maxDays;
    apText.text = "AP: " + ap;
  }

  public void StartNewDate(){
    date = new Event[7];
    for(int i=0; i<7; i++){
      date[i] = new Event();
    }
    currentDateEvent = 0;
  }

  public Event GetCurrentEvent(){
    return date[currentDateEvent];
  }




  public void ClickSelectNewCouple(){
    GlobalData.instance.SelectNewCouple();
  }
}
