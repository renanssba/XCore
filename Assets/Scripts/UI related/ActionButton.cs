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

  public void InitializeAsItem(ItemListing newItem) {
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
      string[] nameParts = skill.name.Split(' ');
      if(nameParts.Length > 1) {
        prefix = nameParts[0] + " ";
      }

      ActionSkin actionSkin = GetActionSkin();
      nameText.text = prefix + actionSkin.buttonName;
      iconImage.sprite = ResourcesManager.instance.attributeSprites[(int)skill.attribute];
      iconImage.color = ResourcesManager.instance.attributeColor[(int)skill.attribute];

      /// improvement icon
      improvementIconImage.gameObject.SetActive(skill.id >= 3);
    } else {
      iconImage.sprite = skill.sprite;
      iconImage.color = Color.white;
      improvementIconImage.gameObject.SetActive(false);
    }

    /// SP cost
    if(skill.spCost > 0) {
      spCostText.text = "SP: " + skill.spCost;
      spCostText.gameObject.SetActive(true);
    } else {
      spCostText.gameObject.SetActive(false);
    }

    /// shade
    shade.gameObject.SetActive(!ActionCanBeUsed());
  }

  public ActionSkin GetActionSkin() {
    string sexModifier = (person.isMale ? "_boy" : "_girl");
    string actionSkinName = SpecialCodes.InterpretStrings("\\vsn[" + skill.attribute.ToString() + "_action" + sexModifier + "_name]");
    return BattleController.instance.GetActionSkinByName(actionSkinName);
  }

  public void UpdateUIAsItem() {
    Item item = ItemDatabase.instance.GetItemById(itemListing.id);

    /// name
    nameText.text = item.GetPrintableName();

    /// icon
    iconImage.sprite = item.sprite;
    iconImage.color = Color.white;

    /// quantity
    spCostText.text = "x" + itemListing.amount;
    spCostText.gameObject.SetActive(true);
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
