using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuTabs {
  status,
  inventory,
  system,
  extras,
  none
}


public class MenuController : MonoBehaviour {

  public CoupleStatusScreen coupleStatusScreen;
  public ItemSelectorScreen inventoryScreen;
  public SystemScreen systemScreen;
  public ScreenTransitions extrasPanel;

  public ScreenTransitions myPanel;

  public ToggleGroup tabsToggleGroup;
  public Toggle[] tabToggles;

  public MenuTabs currentTab;
  public GameObject cancelTabsShade;

  public static MenuController instance;


  public void Awake() {
    instance = this;
    gameObject.SetActive(false);
  }

  public void Update() {
    if(myPanel.IsInteractable() && currentTab != GetSelectedId()) {
      // change current tab
      SfxManager.StaticPlayConfirmSfx();
      currentTab = GetSelectedId();
      UpdateMenu();
    }
  }



  public void OpenMenuOnInventory() {
    currentTab = MenuTabs.inventory;
    myPanel.OpenMenuScreen();
    UpdateMenu();
    VsnAudioManager.instance.PlaySfx("ui_menu_open");
    BlockTabsNavigation(false);
  }

  public void OpenMenuOnStatus(int personId) {
    currentTab = MenuTabs.status;
    coupleStatusScreen.Initialize(GlobalData.instance.people[personId]);
    myPanel.OpenMenuScreen();
    UpdateMenu();
    VsnAudioManager.instance.PlaySfx("ui_menu_open");
    BlockTabsNavigation(false);
  }

  public void OpenMenuOnSystem() {
    currentTab = MenuTabs.system;
    myPanel.OpenMenuScreen();
    UpdateMenu();
    VsnAudioManager.instance.PlaySfx("ui_menu_open");
    BlockTabsNavigation(false);
  }

  public void OpenMenuOnExtras() {
    currentTab = MenuTabs.extras;
    extrasPanel.OpenMenuScreen();
    UpdateMenu();
    VsnAudioManager.instance.PlaySfx("ui_menu_open");
    BlockTabsNavigation(false);
  }


  public void CloseOtherMenus() {
    tabsToggleGroup.SetAllTogglesOff();

    coupleStatusScreen.panel.gameObject.SetActive(false);
    coupleStatusScreen.skilltreeScreen.screenTransitions.gameObject.SetActive(false);
    inventoryScreen.screenTransition.gameObject.SetActive(false);
    systemScreen.optionsPanel.gameObject.SetActive(false);
    systemScreen.savePanel.gameObject.SetActive(false);
    extrasPanel.gameObject.SetActive(false);
  }

  public void UpdateMenu() {
    CloseOtherMenus();
    tabToggles[(int)currentTab].isOn = true;

    switch(currentTab) {
      case MenuTabs.status:
        coupleStatusScreen.panel.ShowPanel();
        if(VsnSaveSystem.GetBoolVariable("tutorial_menu_attributes") == false) {
          VsnController.instance.StartVSN("tutorials", new VsnArgument[] { new VsnString("menu_attributes") });
        }
        break;
      case MenuTabs.inventory:
        inventoryScreen.OpenInventory();
        break;
      case MenuTabs.system:
        systemScreen.Initialize();
        systemScreen.optionsPanel.ShowPanel();
        break;
      case MenuTabs.extras:
        extrasPanel.ShowPanel();
        break;
    }
  }

  public MenuTabs GetSelectedId() {
    for(int i=0; i<tabToggles.Length; i++) {
      if(tabToggles[i].isOn) {
        return (MenuTabs)i;
      }
    }
    return MenuTabs.none;
  }

  public void BlockTabsNavigation(bool value) {
    cancelTabsShade.SetActive(value);
  }
}
