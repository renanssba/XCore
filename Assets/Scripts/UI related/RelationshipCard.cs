using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RelationshipCard : MonoBehaviour {

  public Image[] characterFaceImage;
  public TextMeshProUGUI levelText;
  public Slider expSlider;
  public TextMeshProUGUI expText;

  public TextMeshProUGUI coupleNameText;
  public Panel screenTransitions;

  public TextMeshProUGUI proximityLevelText;

  public Relationship relationship;
  public GameObject unusedBondPointIcon;


  public void Initialize(Relationship rel) {
    relationship = rel;
    UpdateUI();
  }

  public void UpdateUI() {
    characterFaceImage[0].sprite = ResourcesManager.instance.GetFaceSprite(relationship.people[0].id);
    characterFaceImage[1].sprite = ResourcesManager.instance.GetFaceSprite(relationship.people[1].id);

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

    int currentCoupleId = GlobalData.instance.currentRelationshipId;
    GlobalData.instance.currentRelationshipId = relationship.id;
    coupleNameText.text = Lean.Localization.LeanLocalization.GetTranslationText("char_name/couple")
      .Replace("XXXXX", relationship.people[0].GetName())
      .Replace("YYYYY", relationship.people[1].GetName());
    GlobalData.instance.currentRelationshipId = currentCoupleId;
    levelText.text = relationship.level.ToString();
    unusedBondPointIcon.SetActive(relationship.bondPoints > 0);
  }

  public void RaiseExp(int expToAdd) {
    screenTransitions.ShowPanel();
    StartCoroutine(ShowRaiseHeartsAnimation(expToAdd));
  }

  public IEnumerator ShowRaiseHeartsAnimation(int expToAdd) {
    yield return new WaitForSeconds(0.8f);

    Debug.Log("final Exp to add: " + expToAdd);

    int finalValue = Mathf.Min(relationship.exp + expToAdd, 2880);
    int initiValue = relationship.exp;
    int actualRaise = finalValue - relationship.exp;
    const int totalParts = 100;

    VsnSaveSystem.SetVariable("previousLevel", relationship.level);

    if(relationship.level == 10) {
      //yield break;
      // dont raise exp if level is at max
    } else {
      VsnAudioManager.instance.PlayAmbience("experience_up");
      for(int i = 0; i <= totalParts; i++) {
        float part = ((float)i) / totalParts;
        int currentExp = initiValue + (int)(part * actualRaise);

        bool didLevelUp = relationship.GetExp(currentExp - relationship.exp);
        UpdateUI();
        if(didLevelUp) {
          if(relationship.level == 10) {
            AchievementsController.ReceiveAchievement("MAX_LEVEL");
          }

          VsnAudioManager.instance.StopAmbience("experience_up");
          VsnAudioManager.instance.PlaySfx("level_up");
          yield return new WaitForSeconds(1.2f);
          VsnAudioManager.instance.PlayAmbience("experience_up");
        } else {
          yield return new WaitForSeconds(0.01f);
        }
      }
      VsnAudioManager.instance.StopAmbience("experience_up");
    }

    
    yield return new WaitForSeconds(1.3f);

    VsnController.instance.GotCustomInput();
    screenTransitions.HidePanel();

    UIController.instance.UpdateUI();

    VsnArgument[] sayArgs = new VsnArgument[1];
    sayArgs[0] = new VsnString("get_exp");
    Command.GotoScriptCommand.StaticExecute("after_get_exp", sayArgs);
  }

  //public void ClickRelationshipCard() {
  //  MenuController.instance.OpenMenuOnStatus(relationship.id);
  //}
}
