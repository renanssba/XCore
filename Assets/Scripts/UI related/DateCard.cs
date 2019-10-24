using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class ActionCardData {
  public float asd;
}

public enum DateCardType {
  actionCard,
  skillCard,
  itemCard
}


public class DateCard : MonoBehaviour {
  public TextMeshProUGUI cardNameText;
  public Image cardBgImage;
  public Image cardIlustration;

  public TextMeshProUGUI descriptionText;
  public Image[] starImages;

  public Sprite[] starSprites;

  public Attributes attribute;
  public float multiplier;


  public void OnEnable() {
    UpdateUI();
  }

  public void UpdateUI() {
    switch(attribute) {
      case Attributes.guts:
        cardNameText.text = "<color=#B27535>Valentia <size=40>"+ (GlobalData.instance.EventSolvingAttributeLevel((int)Attributes.guts)* multiplier).ToString() + "</size></color>";
        break;
      case Attributes.intelligence:
        cardNameText.text = "<color=#248BCF>Inteligência <size=40>" + (GlobalData.instance.EventSolvingAttributeLevel((int)Attributes.intelligence) * multiplier).ToString() + "</size></color>";
        break;
      case Attributes.charisma:
        cardNameText.text = "<color=#A80218>Carisma <size=40>" + (GlobalData.instance.EventSolvingAttributeLevel((int)Attributes.charisma) * multiplier).ToString() + "</size></color>";
        break;
    }
    cardNameText.color = ResourcesManager.instance.attributeColor[(int)attribute];
    cardIlustration.sprite = ResourcesManager.instance.attributeSprites[(int)attribute];
    cardIlustration.color = ResourcesManager.instance.attributeColor[(int)attribute];
    descriptionText.text = "Ação de " + Lean.Localization.LeanLocalization.GetTranslationText("attribute/"+ attribute.ToString()) + " x " + multiplier;
  }


  public void ClickDateCard() {
    Debug.LogWarning("Clicked date card! Selected attribute: " + attribute);
    VsnSaveSystem.SetVariable("selected_attribute", (int)attribute);
    VsnController.instance.GotCustomInput();

    GameController.instance.dateCardsPanel.HidePanel();
  }
}
