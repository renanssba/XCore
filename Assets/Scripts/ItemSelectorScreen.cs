using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

  private ItemInteractionType interactionType;

  public void Awake() {
    instance = this;
    gameObject.SetActive(false);
  }

  public void OpenStore() {
    Initialize(ItemInteractionType.store_buy, new Inventory(ItemDatabase.instance.itemsForSale));
    screenTransition.OpenMenuScreen();
  }

  public void OpenInventory() {
    Initialize(ItemInteractionType.inventory, GlobalData.instance.inventory);
    screenTransition.OpenMenuScreen();
  }

  public void OpenInput() {
    Initialize(ItemInteractionType.input, GlobalData.instance.inventory);
    screenTransition.OpenMenuScreen();
  }

  public void Initialize(ItemInteractionType type, Inventory currentItems){
    if (VsnSaveSystem.GetStringVariable("language") == "pt_br") {
      SetScreenNamePtBr(type);
    }else{
      SetScreenNameEng(type);
    }

    currentMoneyText.transform.parent.gameObject.SetActive(true);
    currentMoneyText.text = VsnSaveSystem.GetIntVariable("money").ToString();

    interactionType = type;
    ResetItems();
    InitializeItems(currentItems);
  }

  void SetScreenNamePtBr(ItemInteractionType type) {
    switch (type) {
      case ItemInteractionType.store_buy:
        screenNameText.text = "Loja - Comprar";
        break;
      case ItemInteractionType.store_sell:
        screenNameText.text = "Loja - Vender";
        break;
      case ItemInteractionType.input:
        screenNameText.text = "Escolha item para equipar:";
        break;
      case ItemInteractionType.inventory:
        screenNameText.text = "Inventário";
        break;
    }
  }

  void SetScreenNameEng(ItemInteractionType type) {
    switch (type) {
      case ItemInteractionType.store_buy:
        screenNameText.text = "Store - Buy";
        break;
      case ItemInteractionType.store_sell:
        screenNameText.text = "Store - Sell";
        break;
      case ItemInteractionType.input:
        screenNameText.text = "Choose item to equip:";
        break;
      case ItemInteractionType.inventory:
        screenNameText.text = "Inventory";
        break;
    }
  }


  void InitializeItems(Inventory currentItems){
    for(int i = 0; i < currentItems.items.Count; i++) {
      CreateItem(currentItems.items[i].id, currentItems.items[i].amount);
    }
  }


  void CreateItem(int itemId, int amount){
    if(interactionType == ItemInteractionType.store_buy &&
       Item.GetItem(itemId).type == ItemType.celestial &&
       GlobalData.instance.inventory.HasItem(itemId)){
      return;
    }

    GameObject obj = Instantiate(itemPrefab, itemsHolder.transform) as GameObject;
    obj.GetComponent<ItemUI>().Initialize(itemId, interactionType, amount);
  }


  public void SetName(string name){
    screenNameText.text = name;
  }

  public void ResetItems(){
    int childCount = itemsHolder.transform.childCount;

    Debug.Log("child count: " + childCount);

    for(int i=0; i<childCount; i++){
      Destroy(itemsHolder.transform.GetChild(i).gameObject);
    }
  }

  public void ClickExitButton(){
    VsnSaveSystem.SetVariable("item_id", -1);
    VsnController.instance.GotItemInput();
    screenTransition.CloseMenuScreen();
  }
}
