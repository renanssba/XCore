﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemScreen : MonoBehaviour {

  public ScreenTransitions basePanel;
  public ScreenTransitions savePanel;
  public bool isInSaveMode = true;

  public TextMeshProUGUI playtimeText;

  public Button[] systemButtons;

  public static SystemScreen instance;


  public void Awake() {
    instance = this;
  }


  public void Initialize() {
    if(BattleController.instance.IsBattleHappening() || CurrentlyOnGirlInteractionScreen()) {
      Utils.SetButtonDisabledGraphics(systemButtons[0]);
      Utils.SetButtonDisabledGraphics(systemButtons[1]);
      Utils.SetButtonDisabledGraphics(systemButtons[2]);
    } else {
      Utils.SetButtonEnabledGraphics(systemButtons[0]);
      Utils.SetButtonEnabledGraphics(systemButtons[1]);
      Utils.SetButtonEnabledGraphics(systemButtons[2]);
    }
  }

  public void Update() {
    playtimeText.text = "Tempo de jogo\n" + Utils.GetTimeFormattedAsString(GlobalData.instance.playtime / 60); ;
  }


  public void ClickOpenSavePanel() {
    if(BattleController.instance.IsBattleHappening() || CurrentlyOnGirlInteractionScreen()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    isInSaveMode = true;
    SfxManager.StaticPlayConfirmSfx();
    basePanel.HidePanel();
    savePanel.ShowPanel();
  }

  public void ClickOpenLoadPanel() {
    if(BattleController.instance.IsBattleHappening() || CurrentlyOnGirlInteractionScreen()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    isInSaveMode = false;
    SfxManager.StaticPlayConfirmSfx();
    basePanel.HidePanel();
    savePanel.ShowPanel();
  }

  public void ClickBackToMenu() {
    if(BattleController.instance.IsBattleHappening() || CurrentlyOnGirlInteractionScreen()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    VsnController.instance.StartVSN("save_load_functions", new VsnArgument[] { new VsnString("back_to_menu") });
    if(MenuController.instance != null) {
      MenuController.instance.myPanel.CloseMenuScreen();
    }
  }

  public void ClickCloseSavePanel() {
    SfxManager.StaticPlayCancelSfx();
    savePanel.HidePanel();
    basePanel.ShowPanel();
  }

  public bool CurrentlyOnGirlInteractionScreen() {
    return UIController.instance.girlInteractionScreen.gameObject.activeSelf;
  }
}
