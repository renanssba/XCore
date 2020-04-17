﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RelationshipCard : MonoBehaviour {

  public Image[] characterFaceImage;
  public Image heartIcon;
  public TextMeshProUGUI levelText;
  public Slider expSlider;
  public TextMeshProUGUI expText;

  public TextMeshProUGUI coupleNameText;
  public ScreenTransitions screenTransitions;

  public TextMeshProUGUI proximityLevelText;

  public Relationship relationship;


  public void Initialize(Relationship rel) {
    relationship = rel;
    UpdateUI();
  }

  public void UpdateUI() {
    characterFaceImage[0].sprite = ResourcesManager.instance.GetFaceSprite(relationship.GetBoy().faceId);
    characterFaceImage[1].sprite = ResourcesManager.instance.GetFaceSprite(relationship.GetGirl().faceId);

    int startingPoint = Relationship.LevelStartingExp(relationship.level);
    int neededExp = Relationship.LevelUpNeededExp(relationship.level);
    int currentLevelExp = relationship.exp - startingPoint;

    expSlider.maxValue = neededExp;
    expSlider.value = currentLevelExp;

    proximityLevelText.text = relationship.GetRelationshipLevelDescription();

    if(relationship.level < 10) {
      expText.text = currentLevelExp.ToString() + " /" + neededExp;
    } else {
      expText.text = "MAX";
    }
    

    coupleNameText.text = relationship.GetBoy().GetName() + " e " + relationship.GetGirl().GetName();
    levelText.text = relationship.level.ToString();
  }

  public void RaiseExp(int expToAdd) {
    screenTransitions.ShowPanel();
    StartCoroutine(ShowRaiseHeartsAnimation(expToAdd));
  }

  public IEnumerator ShowRaiseHeartsAnimation(int expToAdd) {
    yield return new WaitForSeconds(0.8f);

    Debug.Log("final Exp to add: " + expToAdd);

    int finalValue = Mathf.Min(relationship.exp + expToAdd, 7220);
    int initiValue = relationship.exp;
    int actualRaise = finalValue - relationship.exp;
    const int totalParts = 100;

    if(relationship.level == 10) {
      yield break;
    }

    VsnAudioManager.instance.PlayAmbience("experience_up");
    for(int i=0; i<=totalParts; i++) {
      float part = ((float)i) / totalParts;
      int currentExp = initiValue + (int)(part*actualRaise);

      bool didLevelUp = relationship.GetExp(currentExp-relationship.exp);
      UpdateUI();
      if(didLevelUp) {
        VsnAudioManager.instance.StopAmbience("experience_up");
        VsnAudioManager.instance.PlaySfx("level_up");
        HeartPulseAnimation();
        yield return new WaitForSeconds(1.2f);
        VsnAudioManager.instance.PlayAmbience("experience_up");
      } else {
        yield return new WaitForSeconds(0.01f);
      }
    }
    VsnAudioManager.instance.StopAmbience("experience_up");
    yield return new WaitForSeconds(1.3f);

    VsnController.instance.GotCustomInput();
    screenTransitions.HidePanel();

    UIController.instance.UpdateUI();

    VsnArgument[] sayArgs = new VsnArgument[2];
    sayArgs[0] = new VsnString("char_name/none");
    if(expToAdd > 20) {
      sayArgs[1] = new VsnString("improve_relationship/say_1");
    } else {
      sayArgs[1] = new VsnString("improve_relationship/say_0");
    }
    Command.SayCommand.StaticExecute(sayArgs);
  }

  public void HeartPulseAnimation() {
    VsnAudioManager.instance.PlaySfx("level_up");
    heartIcon.transform.DOScale(1.4f, 0.4f).SetRelative().SetLoops(2, LoopType.Yoyo);
  }


  public void ClickRelationshipCard() {
    VsnAudioManager.instance.PlaySfx("ui_menu_open");
    UIController.instance.coupleStatusScreen.Initialize(relationship);
    UIController.instance.coupleStatusScreen.panel.ShowPanel();
  }
}
