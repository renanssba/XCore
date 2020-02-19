using System.Collections;
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

    expText.text = currentLevelExp.ToString()+" /"+neededExp;

    coupleNameText.text = relationship.GetBoy().name + " e " + relationship.GetGirl().name;
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

    for(int i=0; i<=totalParts; i++) {
      float part = ((float)i) / totalParts;
      int currentExp = initiValue + (int)(part*actualRaise);

      bool didLevelUp = relationship.GetExp(currentExp-relationship.exp);
      UpdateUI();
      if(didLevelUp) {
        HeartPulseAnimation();
        yield return new WaitForSeconds(1.2f);
      } else {
        yield return new WaitForSeconds(0.01f);
      }
    }
    yield return new WaitForSeconds(1.3f);

    VsnController.instance.GotCustomInput();
    screenTransitions.HidePanel();


    VsnArgument[] sayArgs = new VsnArgument[2];
    sayArgs[0] = new VsnString("char_name/none");
    if(actualRaise > 20) {
      sayArgs[1] = new VsnString("add_heart/say_1");
    } else {
      sayArgs[1] = new VsnString("add_heart/say_0");
    }
    Command.SayCommand.StaticExecute(sayArgs);
  }

  public void HeartPulseAnimation() {
    VsnAudioManager.instance.PlaySfx("relationship_up");
    heartIcon.transform.DOScale(1.4f, 0.4f).SetRelative().SetLoops(2, LoopType.Yoyo);
  }
}
