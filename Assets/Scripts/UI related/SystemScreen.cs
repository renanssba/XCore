using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemScreen : MonoBehaviour {

  public ScreenTransitions basePanel;
  public ScreenTransitions savePanel;
  public bool isInSaveMode = true;

  public Button[] systemButtons;

  public static SystemScreen instance;


  public void Awake() {
    instance = this;
  }


  public void Initialize() {
    if(BattleController.instance.IsBattleHappening()) {
      Utils.SetButtonDisabledGraphics(systemButtons[0]);
      Utils.SetButtonDisabledGraphics(systemButtons[1]);
    } else {
      Utils.SetButtonEnabledGraphics(systemButtons[0]);
      Utils.SetButtonEnabledGraphics(systemButtons[1]);
    }
  }


  public void ClickOpenSavePanel() {
    if(BattleController.instance.IsBattleHappening()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    isInSaveMode = true;
    SfxManager.StaticPlayConfirmSfx();
    basePanel.HidePanel();
    savePanel.ShowPanel();
  }

  public void ClickOpenLoadPanel() {
    if(BattleController.instance.IsBattleHappening()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    isInSaveMode = false;
    SfxManager.StaticPlayConfirmSfx();
    basePanel.HidePanel();
    savePanel.ShowPanel();
  }

  public void ClickBackToMenu() {
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
}
