using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlerInfoPanel : MonoBehaviour {

  public Battler character = null;

  [Header("- Visuals -")]
  public CanvasGroup canvasGroup;
  public TextMeshProUGUI nameText;
  public Image faceImage;

  [Header("- HP and SP Sliders -")]
  public HpSlider hpSlider;
  public HpSlider spSlider;

  [Header("- Status Conditions -")]
  public Transform statusConditionsContent;
  public GameObject statusConditionIconPrefab;


  public virtual void SetSelectedUnit(Battler myBattler) {
    character = myBattler;
    if(character == null) {
      canvasGroup.alpha = 0f;
      return;
    }
    canvasGroup.alpha = 1f;
    UpdateBattlerUI();
  }


  public void UpdateBattlerUI() {
    if(character == null || gameObject.activeSelf == false) {
      return;
    }

    /// Name and Face
    nameText.text = character.GetName().ToUpper();
    faceImage.sprite = ResourcesManager.instance.GetCharacterSprite(character.id, CharacterSpritePart.face);

    /// HP
    hpSlider.SetMaxValue(character.GetAttributeValue(Attributes.maxHp));
    hpSlider.SetSliderValue(character.hp);

    /// SP
    if(character.GetType() == typeof(Pilot)) {
      spSlider.SetMaxValue(((Pilot)character).GetMaxSp());
      spSlider.SetSliderValue(((Pilot)character).sp);
    } else {
      spSlider.transform.parent.gameObject.SetActive(false);
    }

    SkipHpAndSpBarAnimations();


    /// STATUS CONDITIONS
    UpdateStatusConditions();
  }

  public void SkipHpAndSpBarAnimations() {
    if(character == null) {
      return;
    }

    /// HP
    hpSlider.SetSliderValueWithoutAnimation(character.hp);

    /// SP
    if(character.GetType() == typeof(Pilot)) {
      spSlider.SetSliderValueWithoutAnimation(((Pilot)character).sp);
    }
  }


  public void UpdateStatusConditions() {
    statusConditionsContent.ClearChildren();

    for(int i = 0; i < character.statusConditions.Count; i++) {
      GameObject newObj = Instantiate(statusConditionIconPrefab, statusConditionsContent);
      newObj.GetComponent<StatusConditionIcon>().Initialize(character.statusConditions[i]);
    }
  }


  public void Clicked() {
    if(character == null || character.GetType() == typeof(Enemy)) {
      return;
    }
    if(VsnSaveSystem.GetBoolVariable("tut_require_click_action_button")) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    MenuController.instance.OpenMenuOnStatus(character.id);
  }

}
