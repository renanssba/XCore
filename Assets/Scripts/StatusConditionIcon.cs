using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public enum StatusConditionEffect {
  raiseGuts,
  raiseIntelligence,
  raiseCharisma,
  raiseEndurance,
  turnDamageGuts,
  turnDamageIntelligence,
  turnDamageCharisma,
  damageMultiplier,
  missAttacks,
  confusion,
  limitActionsToGuts,
  limitActionsToIntelligence,
  limitActionsToCharisma,
  limitActionsToDefense,
  chanceToAutoUseGuts,
  chanceToAutoUseIntelligence,
  chanceToAutoUseCharisma,
  chanceToAutoDefend,
  cantAct,
  becomeTarget,
  digesting,
  count
}


[System.Serializable]
public class StatusCondition {
  public string name;
  //public string description;
  public int id;
  public int duration;
  public int maxDurationShowable;
  public Sprite sprite;

  public StatusConditionEffect[] statusEffect;
  public float[] statusEffectPower;
  public int stackable = 1;

  public StatusCondition() {
    duration = -1;
  }


  public string GetPrintableName() {
    string s = Lean.Localization.LeanLocalization.GetTranslationText("status_condition/name/"+name);
    if(string.IsNullOrEmpty(s)) {
      return name;
    }
    return s;
  }

  public string GetStatusDescription() {
    string desc = "";

    for(int i = 0; i < statusEffect.Length; i++) {
      desc += GetStatusEffectDescription(i);
      if(i != statusEffect.Length - 1) {
        desc += "\n";
      }
    }
    if(duration > 0) {
      desc += "\n<color=#AAAA>(" + Mathf.Min(duration, maxDurationShowable) + " turnos)</color>";
    }

    return desc;
  }

  public string GetStatusEffectDescription(int i) {
    string desc = "";

    switch(statusEffect[i]) {
      case StatusConditionEffect.raiseGuts:
        desc += (statusEffectPower[i] > 0) ? "+" + statusEffectPower[i] + " " : statusEffectPower[i].ToString() + " ";
        desc += Lean.Localization.LeanLocalization.GetTranslationText("attribute/guts");
        break;
      case StatusConditionEffect.raiseIntelligence:
        desc += (statusEffectPower[i] > 0) ? "+" + statusEffectPower[i] + " " : statusEffectPower[i].ToString() + " ";
        desc += Lean.Localization.LeanLocalization.GetTranslationText("attribute/intelligence");
        break;
      case StatusConditionEffect.raiseCharisma:
        desc += (statusEffectPower[i] > 0) ? "+" + statusEffectPower[i] + " " : statusEffectPower[i].ToString() + " ";
        desc += Lean.Localization.LeanLocalization.GetTranslationText("attribute/charisma");
        break;
      case StatusConditionEffect.raiseEndurance:
        desc += (statusEffectPower[i] > 0) ? "+" + statusEffectPower[i] + " " : statusEffectPower[i].ToString() + " ";
        desc += Lean.Localization.LeanLocalization.GetTranslationText("attribute/endurance");
        break;
      default:
        desc += SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("status_condition/description/" + name));
        break;
    }
    return desc;
  }

  public bool ContainsStatusEffect(StatusConditionEffect effect) {
    for(int i = 0; i < statusEffect.Length; i++) {
      if(statusEffect[i] == effect) {
        return true;
      }
    }
    return false;
  }

  public int AttributeBonus(int attributeId) {
    int sum = 0;
    for(int i=0; i < statusEffect.Length; i++) {
      if((int)statusEffect[i] == (int)attributeId) {
        sum += (int)statusEffectPower[i];
      }
    }
    return sum;
  }

  public float DamageMultiplier() {
    float modifier = 1f;
    for(int i = 0; i < statusEffect.Length; i++) {
      if(statusEffect[i] == StatusConditionEffect.damageMultiplier) {
        modifier *= statusEffectPower[i];
      }
    }
    return modifier;
  }

  public StatusCondition GenerateClone() {
    return (StatusCondition)MemberwiseClone();
  }
}


public class StatusConditionIcon : MonoBehaviour {

  public StatusCondition status;

  public Image imageIcon;
  public TextMeshProUGUI durationText;

  public ScreenTransitions descriptionPanel;
  public TextMeshProUGUI nameText;
  public TextMeshProUGUI descriptionText;

  public void Initialize(StatusCondition s) {
    status = s;
    UpdateUI();
  }

  public void UpdateUI() {
    imageIcon.sprite = status.sprite;
    if(status.duration > 0) {
      durationText.text = Mathf.Min(status.duration, status.maxDurationShowable).ToString();
    } else {
      durationText.text = "";
    }    
    descriptionPanel.gameObject.SetActive(false);

    nameText.text = status.GetPrintableName();
    descriptionText.text = status.GetStatusDescription();
  }

  public void ShowDescription() {
    descriptionPanel.ShowPanel();
  }

  public void HideDescription() {
    descriptionPanel.HidePanel();
  }
}
