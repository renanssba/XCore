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


  public void UpdateUI() {
    nameText.text = Lean.Localization.LeanLocalization.GetTranslationText("item/name/" + item.name);
    descriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText("item/description/" + item.description);
    //    if(item.type == ItemType.mundane) {
    //      typeImage.sprite = UIController.GetInstance().itemSelectorScreen.mundaneSprite;
    //    } else {
    //      typeImage.sprite = UIController.GetInstance().itemSelectorScreen.celestialSprite;
    //    }
    typeImage.sprite = item.sprite;

    if (interactionType == ItemInteractionType.store_buy){ 
      costText.text = "<sprite=\"Attributes\" index=4>" + item.price.ToString();
    }else{
      costText.text = "<sprite=\"Attributes\" index=4>" + (item.price/2).ToString();
    }
    quantityText.text = "x" + amount;
    if(interactionType == ItemInteractionType.store_buy ||
       interactionType == ItemInteractionType.store_sell) {
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
    VsnSaveSystem.SetVariable("item_name", Item.GetName(item.id));
    switch (interactionType) {
      case ItemInteractionType.store_buy:
        VsnSaveSystem.SetVariable("item_price", item.price);
        break;
      case ItemInteractionType.store_sell:
        VsnSaveSystem.SetVariable("item_price", item.price/2);
        break;
      case ItemInteractionType.equip_item:
        Person p = GlobalData.instance.people[VsnSaveSystem.GetIntVariable("person_equip_selected")];
        p.EquipItemInSlot(VsnSaveSystem.GetIntVariable("slot_id"), item);
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
