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


  public virtual void Initialize(Battler myBattler) {
    character = myBattler;
    UpdateBattlerUI();
  }


  public void UpdateBattlerUI() {
    if(character == null || gameObject.activeSelf == false) {
      return;
    }

    /// Name and Face
    nameText.text = character.GetName().ToUpper();
    faceImage.sprite = ResourcesManager.instance.GetFaceSprite(character.id);

    /// HP
    hpSlider.SetMaxValue(character.AttributeValue(Attributes.maxHp));
    hpSlider.SetSliderValue(character.hp);

    /// SP
    if(character.GetType() == typeof(Pilot)) {
      spSlider.SetMaxValue(((Pilot)character).GetMaxSp());
      spSlider.SetSliderValue(((Pilot)character).sp);
    } else {
      spSlider.transform.parent.gameObject.SetActive(false);
    }


    /// STATUS CONDITIONS
    UpdateStatusConditions();
  }

  public void SkipHpBarAnimation() {
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
      GameObject newObj = Instantiate(UIController.instance.statusConditionIconPrefab, statusConditionsContent);
      newObj.GetComponent<StatusConditionIcon>().Initialize(character.statusConditions[i]);
    }
  }


  public void Clicked() {
    if(VsnSaveSystem.GetBoolVariable("tut_require_click_action_button")) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    MenuController.instance.OpenMenuOnStatus(character.id);
  }

}
