using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSelectorScreen : MonoBehaviour {

  public static ItemSelectorScreen instance;

  public TextMeshProUGUI screenNameText;
  public TextMeshProUGUI currentMoneyText;
  public GameObject itemsHolder;
  public GameObject itemPrefab;

  public Sprite mundaneSprite;
  public Sprite celestialSprite;

  public ScreenTransitions screenTransition;

  public ScreenContext screenContext;

  private ItemInteractionType interactionType;

  public GameObject emptyIcon;

  public void Awake() {
    instance = this;
    gameObject.SetActive(false);
  }

  public void OpenBuyStore() {
    OpenItemSelectorGeneric(ItemInteractionType.store_buy, new Inventory(ItemDatabase.instance.itemsForSale));
  }

  public void OpenSellStore() {
    OpenItemSelectorGeneric(ItemInteractionType.store_sell, GlobalData.instance.CurrentBoy().inventory);
  }

  public void OpenEquipSelect() {
    OpenItemSelectorGeneric(ItemInteractionType.equip_item, GlobalData.instance.CurrentBoy().inventory);
  }

  public void OpenGiftSelect() {
    OpenItemSelectorGeneric(ItemInteractionType.give_gift, GlobalData.instance.CurrentBoy().inventory);
  }

  public void OpenInventory() {
    OpenItemSelectorGeneric(ItemInteractionType.inventory, GlobalData.instance.CurrentBoy().inventory);
  }

  public void OpenItemSelectorGeneric(ItemInteractionType interType, Inventory inv) {
    VsnAudioManager.instance.PlaySfx("ui_menu_open");
    JoystickController.instance.AddContext(screenContext);
    Initialize(interType, inv);
    screenTransition.OpenMenuScreen();
  }

  public void Initialize(ItemInteractionType type, Inventory currentItems){
    SetScreenName(type);

    currentMoneyText.transform.parent.gameObject.SetActive(true);
    currentMoneyText.text = "<sprite=\"Attributes\" index=4>" + VsnSaveSystem.GetIntVariable("money");

    interactionType = type;
    ClearItems();
    InitializeItems(currentItems);
  }

  void SetScreenName(ItemInteractionType type) {
    switch (type) {
      case ItemInteractionType.store_buy:
        screenNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("inventory/buy");
        break;
      case ItemInteractionType.store_sell:
        screenNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("inventory/sell");
        break;
      case ItemInteractionType.equip_item:
        screenNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("inventory/equip");
        break;
      case ItemInteractionType.give_gift:
        screenNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("inventory/gift");
        break;
      case ItemInteractionType.inventory:
        screenNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("inventory/title");
        break;
    }
  }

  
  void InitializeItems(Inventory currentItems){
    List<Button> createdObjects = new List<Button>();
    GameObject current;

    for(int i = 0; i < currentItems.itemListings.Count; i++) {
      current = CreateItem(currentItems.itemListings[i]);
      if(current != null) {
        createdObjects.Add(current.GetComponent<Button>());
      }
    }
    Debug.Log("child count: " + itemsHolder.transform.childCount);

    Utils.GenerateNavigation(createdObjects.ToArray());

    if(itemsHolder.transform.childCount > 0) {
      emptyIcon.SetActive(false);
      gameObject.SetActive(true);
      if(createdObjects.Count > 0) {
        Utils.SelectUiElement(createdObjects[0].gameObject);
      } else {
        Utils.SelectUiElement(null);
      }
      //Debug.LogError("item setting lastSelectedObject: " + EventSystem.current.currentSelectedGameObject.name);
      JoystickController.instance.CurrentContext().lastSelectedObject = EventSystem.current.currentSelectedGameObject;
    } else {
      emptyIcon.SetActive(true);
    }
  }


  GameObject CreateItem(ItemListing listing){
    GameObject obj = Instantiate(itemPrefab, itemsHolder.transform) as GameObject;
    obj.GetComponent<ItemUI>().Initialize(listing, interactionType);
    return obj;
  }


  public void SetName(string name){
    screenNameText.text = name;
  }

  public void ClearItems(){
    int childCount = itemsHolder.transform.childCount;

    for(int i=0; i<childCount; i++){
      Destroy(itemsHolder.transform.GetChild(i).gameObject);
      //itemsHolder.transform.GetChild(i).gameObject.SetActive(false);
    }
  }

  public void ClickExitButton(){
    VsnAudioManager.instance.PlaySfx("ui_menu_close");
    VsnSaveSystem.SetVariable("item_id", -1);
    VsnController.instance.GotItemInput();
    screenTransition.CloseMenuScreen();
    JoystickController.instance.RemoveContext();
  }
}
