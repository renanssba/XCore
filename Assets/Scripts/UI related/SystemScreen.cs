using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemScreen : MonoBehaviour {

  public ScreenTransitions basePanel;
  public ScreenTransitions savePanel;
  public bool isInSaveMode = true;

  public static SystemScreen instance;


  public void Awake() {
    instance = this;
  }


  public void ClickOpenSavePanel() {
    isInSaveMode = true;
    SfxManager.StaticPlayConfirmSfx();
    basePanel.HidePanel();
    savePanel.ShowPanel();
  }

  public void ClickOpenLoadPanel() {
    isInSaveMode = false;
    SfxManager.StaticPlayConfirmSfx();
    basePanel.HidePanel();
    savePanel.ShowPanel();
  }

  public void ClickBackToMenu() {
    VsnController.instance.StartVSN("action_descriptions", new VsnArgument[] { new VsnString("back_to_menu") });
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
