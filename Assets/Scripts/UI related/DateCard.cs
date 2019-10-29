using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;



public class DateCard : MonoBehaviour {
  public TextMeshProUGUI cardNameText;
  public Image cardBgImage;
  public Image cardIlustration;

  public TextMeshProUGUI descriptionText;
  public Image[] starImages;

  public Image attributeValuePanel;
  public TextMeshProUGUI attributeValueText;

  public GameObject shadeImage;

  public DateCardContent content;
  public Color defaultColor;


  public void Initialize(DateCardContent cont) {
    content = cont;
    UpdateUI();
  }

  public void UpdateUI() {
    Color attributeColor = ResourcesManager.instance.attributeColor[(int)content.attribute];
    cardNameText.text = content.name;
    cardNameText.color = defaultColor;
    descriptionText.text = content.description;

    cardIlustration.sprite = content.sprite;
    cardIlustration.color = attributeColor;

    shadeImage.SetActive(!IsCardUsable());


    if(content.type == DateCardType.actionCard) {
      cardNameText.color = attributeColor;

      attributeValuePanel.gameObject.SetActive(true);
      attributeValuePanel.color = attributeColor;
      attributeValueText.text = GetEffectivePower().ToString();
      cardBgImage.sprite = ResourcesManager.instance.cardSprites[2];
    } else {
      if(content.type == DateCardType.characterSkillCard ||
         content.type == DateCardType.bondSkillCard) {
        descriptionText.text += "\n<color=#842042>-" + content.cost + "<sprite=\"attributes\" index=5></color>";
      }      

      Person ownerOfSkill;
      if(GlobalData.instance.observedPeople[0].skillId == content.id) {
        ownerOfSkill = GlobalData.instance.observedPeople[0];
      } else {
        ownerOfSkill = GlobalData.instance.observedPeople[1];
      }

      cardIlustration.color = Color.white;

      attributeValuePanel.gameObject.SetActive(false);
      if(ownerOfSkill.isMale) {
        cardBgImage.sprite = ResourcesManager.instance.cardSprites[0];
      } else {
        cardBgImage.sprite = ResourcesManager.instance.cardSprites[1];
      }
    }
  }

  public int GetEffectivePower() {
    return (int) (GlobalData.instance.EventSolvingAttributeLevel((int)content.attribute) * content.multiplier);
  }


  public void ClickDateCard() {
    if(!IsCardUsable()) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    switch(content.type) {
      case DateCardType.actionCard:
        UseActionCard();
        break;
      case DateCardType.itemCard:
        UseItemCard();
        break;
      case DateCardType.characterSkillCard:
      case DateCardType.bondSkillCard:
        UseSkillCard();
        break;
      default:
        gameObject.SetActive(false);
        break;
    }
  }

  public bool IsCardUsable() {
    if((content.type == DateCardType.bondSkillCard ||
        content.type == DateCardType.characterSkillCard) && GlobalData.instance.currentDateHearts < content.cost) {
      return false;
    }
    return true;
  }

  public void UseActionCard() {
    Debug.LogWarning("Clicked action card! Selected attribute: " + content.attribute);

    VsnSaveSystem.SetVariable("attribute_effective_level", GetEffectivePower());
    VsnSaveSystem.SetVariable("selected_attribute", (int)content.attribute);
    VsnController.instance.GotCustomInput();
    GameController.instance.dateCardsPanel.HidePanel();
    GameController.instance.DiscardCard(content);
  }

  public void UseSkillCard() {
    GlobalData.instance.currentDateHearts -= content.cost;

    switch(content.skill) {
      case Skill.raiseAttribute:
        VsnAudioManager.instance.PlaySfx("relationship_up");

        DateEvent evt = GameController.instance.GetCurrentDateEvent();
        switch(evt.interactionType) {
          case DateEventInteractionType.male:
            GlobalData.instance.CurrentBoy().GetAttributeBonus(content.attribute, (int)content.multiplier);
            break;
          case DateEventInteractionType.female:
            GlobalData.instance.CurrentGirl().GetAttributeBonus(content.attribute, (int)content.multiplier);
            break;
        }
        break;
      case Skill.sensor:
        TheaterController.instance.ShowWeaknessCard(true);
        break;
      case Skill.flee:
        Debug.LogWarning("Flee from event!");
        VsnAudioManager.instance.PlaySfx("relationship_up");
        GameController.instance.FleeDateSegment(VsnSaveSystem.GetIntVariable("currentDateEvent"));

        GameController.instance.SetScreenLayout("date");
        GameController.instance.dateCardsPanel.HidePanel();
        GameController.instance.StartCoroutine(WaitThenContinueFromFlee());
        break;
    }

    GameController.instance.UpdateDateUI();

    gameObject.SetActive(false);
  }

  public IEnumerator WaitThenContinueFromFlee() {
    yield return new WaitForSeconds(1f);
    VsnController.instance.CurrentScriptReader().GotoWaypoint("start_interaction");
    VsnController.instance.GotCustomInput();
  }

  public void UseItemCard() {
    gameObject.SetActive(false);
  }
}
