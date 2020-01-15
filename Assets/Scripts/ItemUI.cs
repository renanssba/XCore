using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ItemInteractionType{
  store_buy,
  equip_item,
  inventory,
  store_sell,
  give_gift
}

public class ItemUI : MonoBehaviour {

  //public Item item;

  public TextMeshProUGUI nameText;
  public TextMeshProUGUI descriptionText;
  public Image typeImage;
  public TextMeshProUGUI costText;
  public TextMeshProUGUI quantityText;
  public Button button;

  public ItemListing itemListing;
  public ItemInteractionType interactionType;

  public void Initialize(ItemListing listing, ItemInteractionType interaction){
    itemListing = listing;
    interactionType = interaction;
    UpdateUI();
  }


  public void UpdateUI() {
    Item item = Item.GetItemById(itemListing.id);
    string name_suffix = "";
    string description_suffix = "";
    if(itemListing.ownerId != -1) {
      Person owner = GlobalData.instance.people[itemListing.ownerId];
      name_suffix = " d" + (owner.isMale ? "o" : "a") + " " + owner.name;
      description_suffix = " Pertence a " + owner.name + ".";
    }

    nameText.text = item.GetPrintableName() + name_suffix;
    descriptionText.text = item.GetPrintableDescription() +" "+ item.GetBattleDescription() + description_suffix;
    typeImage.sprite = item.sprite;

    if(interactionType == ItemInteractionType.store_buy) {
      costText.text = "<sprite=\"Attributes\" index=4>" + item.price.ToString();
    } else {
      costText.text = "<sprite=\"Attributes\" index=4>" + (item.price / 2).ToString();
    }
    quantityText.text = "x" + itemListing.amount;
    if(interactionType == ItemInteractionType.store_buy ||
       interactionType == ItemInteractionType.store_sell) {
      quantityText.gameObject.SetActive(false);
    } else {
      costText.gameObject.SetActive(false);
    }

    if(interactionType == ItemInteractionType.inventory) {
      button.interactable = false;
    }

    if(item.HasType(ItemType.key)) {
      quantityText.gameObject.SetActive(false);
    }
  }

  public void Clicked(){
    Item item = Item.GetItemById(itemListing.id);
    //ItemSelectorScreen.instance.screenTransition.FadeOutShade(ScreenTransitions.fadeTime);
    VsnSaveSystem.SetVariable("item_id", item.id);
    VsnSaveSystem.SetVariable("item_name", Item.GetPrintableNameById(item.id));
    VsnSaveSystem.SetVariable("item_key", Item.GetKeyById(item.id));
    switch (interactionType) {
      case ItemInteractionType.store_buy:
        VsnSaveSystem.SetVariable("item_price", item.price);
        break;
      case ItemInteractionType.store_sell:
        VsnSaveSystem.SetVariable("item_price", item.price/2);
        break;
      case ItemInteractionType.equip_item:
        //Person p = GlobalData.instance.people[VsnSaveSystem.GetIntVariable("person_equip_selected")];
        //p.EquipItemInSlot(VsnSaveSystem.GetIntVariable("slot_id"), item);
        break;
      case ItemInteractionType.give_gift:
        // do nothing
        break;
    }

    VsnController.instance.GotItemInput();
    //UIController.GetInstance().itemSelectorScreen.gameObject.SetActive(false);
    ItemSelectorScreen.instance.screenTransition.CloseMenuScreen();
    JoystickController.instance.RemoveContext();
  }
}
