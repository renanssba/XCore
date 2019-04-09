using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ItemInteractionType{
  store,
  input,
  inventory
}

public class ItemUI : MonoBehaviour {

  public Item item;

  public TextMeshProUGUI nameText;
  public TextMeshProUGUI descriptionText;
  public Image typeImage;
  public TextMeshProUGUI costText;
  public TextMeshProUGUI quantityText;
  public Button button;

  public ItemInteractionType interactionType;
  public int amount;

  public void Initialize(int itemId, ItemInteractionType interaction, int qtty){
    item = Item.GetItem(itemId);
    interactionType = interaction;
    amount = qtty;
    UpdateUI();
  }


  public void UpdateUI(){
    nameText.text = item.name;
    descriptionText.text = item.description;
//    if(item.type == ItemType.mundane) {
//      typeImage.sprite = UIController.GetInstance().itemSelectorScreen.mundaneSprite;
//    } else {
//      typeImage.sprite = UIController.GetInstance().itemSelectorScreen.celestialSprite;
//    }
    typeImage.sprite = item.sprite;

    costText.text = item.price.ToString();
    quantityText.text = "x" + amount;
    if(interactionType == ItemInteractionType.store) {
      quantityText.gameObject.SetActive(false);
    } else {
      costText.gameObject.SetActive(false);
    }

    if(interactionType == ItemInteractionType.inventory){
      button.interactable = false;
    }

    if(item.type == ItemType.celestial) {
      quantityText.gameObject.SetActive(false);
    }
  }

  public void Clicked(){
    //ItemSelectorScreen.instance.screenTransition.FadeOutShade(ScreenTransitions.fadeTime);
    VsnSaveSystem.SetVariable("item_id", item.id);
    switch(interactionType) {
      case ItemInteractionType.store:
        VsnSaveSystem.SetVariable("item_price", item.price);
        break;
      case ItemInteractionType.input:
        Person p = VsnSaveSystem.GetIntVariable("person_equip_selected")==1?GlobalData.instance.GetCurrentBoy(): GlobalData.instance.GetCurrentGirl();
        p.EquipItemInSlot(VsnSaveSystem.GetIntVariable("slot_id"), item);
        break;
    }

    VsnController.instance.GotItemInput();
    //UIController.GetInstance().itemSelectorScreen.gameObject.SetActive(false);
    ItemSelectorScreen.instance.screenTransition.CloseMenuScreen();
  }
}
