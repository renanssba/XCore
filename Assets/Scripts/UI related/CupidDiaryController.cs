using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CupidDiaryController : MonoBehaviour {

  public static CupidDiaryController instance;

  public CupidDiaryEntry[] entries;
  public int currentPage = 0;
  public ScreenTransitions panel;


  public void Awake() {
    instance = this;
    gameObject.SetActive(false);
  }


  public void OnEnable() {
    entries[0].Initialize(GlobalData.instance.relationships[0]);
    entries[1].Initialize(GlobalData.instance.relationships[2]);
  }
}
