using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CupidDiaryController : MonoBehaviour {

  public static CupidDiaryController instance;

  public CupidDiaryEntry[] entries;
  public int currentPage = 0;
  public ScreenTransitions panel;
  public GameObject continueButton;


  public void Awake() {
    instance = this;
    gameObject.SetActive(false);
  }


  public void OnEnable() {
    continueButton.SetActive(false);
    entries[0].Initialize(GlobalData.instance.relationships[GlobalData.instance.currentRelationshipId]);
    entries[1].Initialize(null);
  }


  public void ShowContinueButton() {
    continueButton.SetActive(true);
  }

  public void ClickContinueButton() {
    SfxManager.StaticPlayConfirmSfx();
    panel.HidePanel();
    VsnController.instance.GotCustomInput();
  }
}
