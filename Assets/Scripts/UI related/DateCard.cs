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

  public int handId;


  public void Initialize(int id, DateCardContent cont) {
    handId = id;
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
      cardBgImage.sprite = ResourcesManager.instance.cardSprites[4];
    } else {
      if(content.type == DateCardType.characterSkillCard ||
         content.type == DateCardType.bondSkillCard) {
        descriptionText.text += "\n<color=#842042>-" + content.cost + "<sprite=\"attributes\" index=5></color>";
      }

      cardIlustration.color = Color.white;

      attributeValuePanel.gameObject.SetActive(false);
      cardBgImage.sprite = ResourcesManager.instance.cardSprites[5];
    }

    if(content.type == DateCardType.itemCard) {
      cardBgImage.sprite = ResourcesManager.instance.cardSprites[2];
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
        CardGoToEndOfHand();
        break;
    }

    GameController.instance.UpdateDateUI();
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
    GameController.instance.DiscardCardFromHand(content);
  }

  public void UseItemCard() {
    CardGoToEndOfHand();
    ExecuteSkillEffect();
    GameController.instance.DiscardCardFromHand(content);
  }

  public void UseSkillCard() {
    DateCardContent cont = content;
    GlobalData.instance.currentDateHearts -= content.cost;

    Debug.LogWarning("Using skill " + content.skill.ToString());

    CardGoToEndOfHand();

    ExecuteSkillEffect();

    GameController.instance.CardFromHandToDeck(content);

    if(cont.skill == Skill.gluttony) {
      DateCardContent item = CardsDatabase.instance.GetCardById(Random.Range(23, 26));
      GameController.instance.cardsHand.Add(item);
      Initialize(handId, item);
      gameObject.SetActive(true);
    }
  }

  public void ExecuteSkillEffect() {
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
        VsnAudioManager.instance.PlaySfx("relationship_up");
        GameController.instance.FleeDateSegment(VsnSaveSystem.GetIntVariable("currentDateEvent"));

        GameController.instance.SetScreenLayout("date");
        GameController.instance.dateCardsPanel.HidePanel();
        GameController.instance.StartCoroutine(WaitThenContinueFromFlee());
        break;
      case Skill.gluttony:
        VsnAudioManager.instance.PlaySfx("relationship_up");
        //DateCardContent cont = CardsDatabase.instance.GetCardById(Random.Range(23, 26));
        //GameController.instance.cardsHand[handId] = cont;
        //GameController.instance.cardsDeck.Add(cont);
        //Initialize(handId, cont);
        //gameObject.SetActive(true);
        break;
    }
  }

  public void CardGoToEndOfHand() {
    gameObject.SetActive(false);
    Transform cardsPanel = transform.parent.parent;
    transform.parent.SetParent(null);
    transform.parent.SetParent(cardsPanel);
  }

  public IEnumerator WaitThenContinueFromFlee() {
    yield return new WaitForSeconds(1f);
    VsnController.instance.CurrentScriptReader().GotoWaypoint("start_interaction");
    VsnController.instance.GotCustomInput();
  }
}
