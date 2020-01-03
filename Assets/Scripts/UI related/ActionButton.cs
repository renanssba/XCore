using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum TurnActionType {
  useSkill,
  useItem,
  flee,
  defend
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

  public void UpdateUI() {
    switch(actionType) {
      case TurnActionType.useSkill:
        UpdateUIAsSkill();
        break;
      case TurnActionType.useItem:
        UpdateUIAsItem();
        break;
    }    
  }

  public void UpdateUIAsSkill() {
    string sexModifier = (person.isMale ? "_boy" : "_girl");
    nameText.text = skill.GetPrintableName();
    if(skill.type == SkillType.attack) {
      if(skill.id != 9) {
        nameText.text = SpecialCodes.InterpretStrings("\\vsn[" + skill.attribute.ToString() + "_action" +sexModifier+ "_name]");
      }
      iconImage.sprite = ResourcesManager.instance.attributeSprites[(int)skill.attribute];
      iconImage.color = ResourcesManager.instance.attributeColor[(int)skill.attribute];
    } else {
      iconImage.sprite = skill.sprite;
      iconImage.color = Color.white;
    }

    if(skill.spCost > 0) {
      spCostText.text = "SP: " + skill.spCost;
      spCostText.gameObject.SetActive(true);
      shade.gameObject.SetActive(!SkillCanBeUsed());
    } else {
      spCostText.gameObject.SetActive(false);
      shade.gameObject.SetActive(false);
    }
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
    string s = "";
    switch(actionType) {
      case TurnActionType.useSkill:
        s = skill.GetPrintableDescription();
        break;
      case TurnActionType.useItem:
        Item it = Item.GetItemById(itemListing.id);
        s = it.GetPrintableDescription();
        break;
    }
    UIController.instance.ShowHelpMessagePanel(s);
  }

  public void SetBackHelpText() {
    string s = Lean.Localization.LeanLocalization.GetTranslationText("choices/cancel");
    UIController.instance.ShowHelpMessagePanel(s);
  }


  public void ClickedActionButton() {
    if(!SkillCanBeUsed()) {
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

  public bool SkillCanBeUsed() {
    return person.sp >= skill.spCost;
  }
}
