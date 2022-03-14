using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum TurnActionType {
  useSkill,
  useItem,
  defend,
  idle
}


public class ActionButton : MonoBehaviour {

  public TurnActionType actionType = TurnActionType.useSkill;

  public Pilot person;
  public Skill skill;

  public ItemListing itemListing;

  public TextMeshProUGUI nameText;
  public Image spCostPanel;
  public TextMeshProUGUI spCostText;
  public Image iconImage;
  public Image improvementIconImage;
  public GameObject shade;


  public void InitializeAsSkill(Pilot p, Skill newSkill) {
    person = p;
    skill = newSkill;
    actionType = TurnActionType.useSkill;
    UpdateUI();
  }

  public void InitializeAsItem(Pilot p, ItemListing newItem) {
    person = p;
    itemListing = newItem;
    actionType = TurnActionType.useItem;
    UpdateUI();
  }

  public void InitializeGeneric(Pilot p) {
    person = p;
    UpdateUI();
  }

  public void UpdateUI() {
    switch(actionType) {
      case TurnActionType.useSkill:
        UpdateUIAsSkill();
        break;
      case TurnActionType.useItem:
        UpdateUIAsItem();
        break;
      case TurnActionType.defend:
      case TurnActionType.idle:
        shade.gameObject.SetActive(!ActionCanBeUsed());
        break;
    }
  }

  public void UpdateUIAsSkill() {
    nameText.text = skill.GetPrintableName();
    iconImage.sprite = skill.sprite;
    iconImage.color = Color.white;
    improvementIconImage.gameObject.SetActive(false);

    /// SP cost
    if(skill.spCost > 0) {
      spCostText.text = Lean.Localization.LeanLocalization.GetTranslationText("attribute/sp")+ ": " + skill.spCost;
      //spCostText.gameObject.SetActive(true);
      spCostPanel.gameObject.SetActive(true);
    } else {
      //spCostText.gameObject.SetActive(false);
      spCostPanel.gameObject.SetActive(false);
    }

    /// shade
    shade.gameObject.SetActive(!ActionCanBeUsed());
  }

  public void UpdateUIAsItem() {
    Item item = ItemDatabase.instance.GetItemById(itemListing.id);

    /// name
    nameText.text = item.GetPrintableName();

    /// icon
    iconImage.sprite = item.GetSprite();
    iconImage.color = Color.white;

    /// quantity
    spCostText.text = "x" + itemListing.amount;
    //spCostText.gameObject.SetActive(true);
    spCostPanel.gameObject.SetActive(true);

    /// shade
    shade.gameObject.SetActive(false);
  }


  

  public void ClickedActionButton() {
    if(!ActionCanBeUsed()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    if(VsnSaveSystem.GetBoolVariable("tut_require_click_action_button")) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    if(VsnSaveSystem.GetBoolVariable("tut_require_click_guts") ) {
      if(actionType != TurnActionType.useSkill || skill == null || skill.type != SkillType.attack || skill.damageAttribute != Attributes.maxHp) {
        SfxManager.StaticPlayForbbidenSfx();
        return;
      } else {
        VsnSaveSystem.SetVariable("tut_require_click_guts", false);
     }
    } 

    SfxManager.StaticPlayConfirmSfx();
    int currentBattler = VsnSaveSystem.GetIntVariable("currentBattlerTurn");

    BattleController.instance.selectedActionType[currentBattler] = actionType;
    switch(actionType) {
      case TurnActionType.useSkill:
        BattleController.instance.selectedSkills[currentBattler] = skill;
        break;
      case TurnActionType.useItem:
        BattleController.instance.selectedItems[currentBattler] = Item.GetItemById(itemListing.id);
        break;
    }

    BattleController.instance.FinishSelectingCharacterAction();
  }

  public bool ActionCanBeUsed() {
    bool value = true;
    if(actionType == TurnActionType.useSkill) {
      value = value && person.CanExecuteSkill(skill);
    }
    value = value && person.CanExecuteAction(actionType);
    return value;
  }
}
