using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum TurnActionType {
  useSkill,
  useItem,
  flee,
  defend,
  idle
}


public class ActionButton : MonoBehaviour {

  public TurnActionType actionType = TurnActionType.useSkill;

  public Person person;
  public Skill skill;

  public ItemListing itemListing;

  public TextMeshProUGUI nameText;
  public Image spCostPanel;
  public TextMeshProUGUI spCostText;
  public Image iconImage;
  public Image improvementIconImage;
  public GameObject shade;


  public void InitializeAsSkill(Person p, Skill newSkill) {
    person = p;
    skill = newSkill;
    actionType = TurnActionType.useSkill;
    UpdateUI();
  }

  public void InitializeAsItem(Person p, ItemListing newItem) {
    person = p;
    itemListing = newItem;
    actionType = TurnActionType.useItem;
    UpdateUI();
  }

  public void InitializeGeneric(Person p) {
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
      case TurnActionType.flee:
        shade.gameObject.SetActive(!ActionCanBeUsed());
        break;
    }
  }

  public void UpdateUIAsSkill() {
    nameText.text = skill.GetPrintableName();
    if(skill.type == SkillType.attack) {
      string prefix = "";
      string[] nameParts = skill.GetPrintableName().Split(' ');
      if(nameParts.Length > 1) {
        prefix = nameParts[0] + " ";
      }

      /// TODO: Implement correct skill names
      nameText.text = prefix + Lean.Localization.LeanLocalization.GetTranslationText("action/button/strike");
      iconImage.sprite = ResourcesManager.instance.attributeSprites[(int)skill.damageAttribute];
      iconImage.color = ResourcesManager.instance.attributeColor[(int)skill.damageAttribute];

      /// improvement icon
      improvementIconImage.gameObject.SetActive(skill.id >= 3);
    } else {
      iconImage.sprite = skill.sprite;
      iconImage.color = Color.white;
      improvementIconImage.gameObject.SetActive(false);
    }

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


  public void SetHelpText() {
    if(transform.parent.GetComponent<CanvasGroup>().interactable == false) {
      return;
    }

    string s = "";
    switch(actionType) {
      case TurnActionType.useSkill:
        s = skill.GetPrintableDescription();
        break;
      case TurnActionType.useItem:
        Item it = Item.GetItemById(itemListing.id);
        s = it.GetBattleDescription(true);
        break;
    }
    UIController.instance.SetHelpMessageText(s);
  }

  public void SetBackHelpText() {
    if(transform.parent.GetComponent<CanvasGroup>().interactable == false) {
      return;
    }

    string s = Lean.Localization.LeanLocalization.GetTranslationText("choices/cancel");
    UIController.instance.SetHelpMessageText(s);
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
      if(actionType != TurnActionType.useSkill || skill == null || skill.type != SkillType.attack || skill.damageAttribute != Attributes.guts) {
        SfxManager.StaticPlayForbbidenSfx();
        return;
      } else {
        VsnSaveSystem.SetVariable("tut_require_click_guts", false);
     }
    } 

    SfxManager.StaticPlayConfirmSfx();
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");

    BattleController.instance.selectedActionType[currentPlayerTurn] = actionType;
    switch(actionType) {
      case TurnActionType.useSkill:
        BattleController.instance.selectedSkills[currentPlayerTurn] = skill;
        break;
      case TurnActionType.useItem:
        BattleController.instance.selectedItems[currentPlayerTurn] = Item.GetItemById(itemListing.id);
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
