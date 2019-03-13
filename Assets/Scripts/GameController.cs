﻿using System.Collections;
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

  public TextAsset dateEventsFile;
  public List<DateEvent> allDateEvents;
  public DateEvent[] date;



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
    allDateEvents = new List<DateEvent>();

    int id;
    int guts;
    int intelligence;
    int charisma;
    EventInteractionType interaction = EventInteractionType.male;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(dateEventsFile, 1);
    foreach(Dictionary<string, string> dic in spreadsheetData.data) {
      id = int.Parse(dic["Id"]);
      guts = int.Parse(dic["Dificuldade Guts"]);
      intelligence = int.Parse(dic["Dificuldade Intelligence"]);
      charisma = int.Parse(dic["Dificuldade Charisma"]);
      switch(dic["Tipo de Interação"]){
        case "male":
          interaction = EventInteractionType.male;
          break;
        case "female":
          interaction = EventInteractionType.female;
          break;
        case "couple":
          interaction = EventInteractionType.couple;
          break;
        case "compatibility":
          interaction = EventInteractionType.compatibility;
          break;
      }
      allDateEvents.Add(new DateEvent(id, dic["Nome do Script"], guts, intelligence, charisma, interaction));
    }
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

  public void GenerateDate(int location){
    int dateSize = 2;
    date = new DateEvent[dateSize];
    for(int i=0; i< dateSize; i++){
      date[i] = allDateEvents[i];
    }
  }

  public DateEvent GetCurrentEvent(){
    int currentDateEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if (date.Length <= currentDateEvent) {
      return null;
    }
    return date[currentDateEvent];
  }

  public string GetCurrentEventName() {
    if(GetCurrentEvent() == null) {
      return "";
    }
    return date[VsnSaveSystem.GetIntVariable("currentDateEvent")].scriptName;
  }




  public void ClickSelectNewCouple(){
    GlobalData.instance.SelectNewCouple();
  }
}
