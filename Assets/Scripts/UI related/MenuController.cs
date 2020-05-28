using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuTabs {
  status,
  inventory,
  system,
  none
}


public class MenuController : MonoBehaviour {

  public CoupleStatusScreen coupleStatusScreen;
  public ItemSelectorScreen inventoryScreen;
  public SystemScreen systemScreen;

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


  public void OpenMenuOnInventory() {
    currentTab = MenuTabs.inventory;
    myPanel.OpenMenuScreen();
    UpdateMenu();
    VsnAudioManager.instance.PlaySfx("ui_menu_open");

  }

  public void OpenMenuOnStatus(int relationshipId) {
    currentTab = MenuTabs.status;
    coupleStatusScreen.Initialize(GlobalData.instance.relationships[relationshipId]);
    myPanel.OpenMenuScreen();
    UpdateMenu();
    VsnAudioManager.instance.PlaySfx("ui_menu_open");
  }

  public void OpenMenuOnSystem() {
    currentTab = MenuTabs.system;
    myPanel.OpenMenuScreen();
    UpdateMenu();
    VsnAudioManager.instance.PlaySfx("ui_menu_open");
  }

  public void Update() {
    if(currentTab != GetSelectedId()) {
      // change current tab
      currentTab = GetSelectedId();
      UpdateMenu();
    }
  }


  public void CloseOtherMenus() {
    tabsToggleGroup.SetAllTogglesOff();

    coupleStatusScreen.panel.gameObject.SetActive(false);
    inventoryScreen.screenTransition.gameObject.SetActive(false);
    systemScreen.basePanel.gameObject.SetActive(false);
    systemScreen.savePanel.gameObject.SetActive(false);
  }

  public void UpdateMenu() {
    MenuTabs selected = GetSelectedId();
    CloseOtherMenus();

    tabToggles[(int)selected].isOn = true;

    switch(selected) {
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
        systemScreen.basePanel.ShowPanel();
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
}
