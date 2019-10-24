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

  public DateCardContent content;


  public void Initialize(DateCardContent cont) {
    content = cont;
    UpdateUI();
  }

  public void UpdateUI() {
    Color attributeColor = ResourcesManager.instance.attributeColor[(int)content.attribute];
    cardNameText.text = content.name;
    cardNameText.color = attributeColor;
    descriptionText.text = content.description;

    cardIlustration.sprite = content.sprite;
    cardIlustration.color = attributeColor;

    if(content.type == DateCardType.actionCard) {
      attributeValuePanel.gameObject.SetActive(true);
      attributeValuePanel.color = attributeColor;
      attributeValueText.text = GetEffectivePower().ToString();
      cardBgImage.sprite = ResourcesManager.instance.cardSprites[2];
    } else {
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
    Debug.LogWarning("Clicked date card! Selected attribute: " + content.attribute);

    switch(content.type) {
      case DateCardType.actionCard:
        VsnSaveSystem.SetVariable("attribute_effective_level", GetEffectivePower());
        VsnSaveSystem.SetVariable("selected_attribute", (int)content.attribute);
        VsnController.instance.GotCustomInput();
        GameController.instance.dateCardsPanel.HidePanel();
        GameController.instance.DiscardCard(content);
        break;
      //case DateCardType.itemCard:
      //  break;
      //case DateCardType.characterSkillCard:
      //  break;
      //case DateCardType.bondSkillCard:
      //  break;
      default:
        gameObject.SetActive(false);
        break;
    }

    
  }
}
