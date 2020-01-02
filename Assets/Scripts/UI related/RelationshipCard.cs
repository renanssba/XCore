using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class RelationshipCard : MonoBehaviour {

  public Image[] characterFaceImage;
  public Image[] heartIcons;

  public TextMeshProUGUI coupleNameText;
  public ScreenTransitions screenTransitions;

  public Relationship relationship;


  public void Initialize(Relationship rel) {
    relationship = rel;
    UpdateUI();
  }

  public void UpdateUI() {
    characterFaceImage[0].sprite = ResourcesManager.instance.GetFaceSprite(relationship.GetBoy().faceId);
    characterFaceImage[1].sprite = ResourcesManager.instance.GetFaceSprite(relationship.GetGirl().faceId);

    coupleNameText.text = relationship.GetBoy().name + " e " + relationship.GetGirl().name;
    UpdateHeartIcons(relationship.hearts);
  }

  public void UpdateHeartIcons(int value) {
    for(int i = 0; i < heartIcons.Length; i++) {
      heartIcons[i].color = (i < value ? Color.white : new Color(0f, 0f, 0f, 0.5f));
    }
  }

  public void RaiseHeartsAnimation(int heartsToAdd) {
    int initValue = relationship.hearts;
    screenTransitions.ShowPanel();
    StartCoroutine(ShowRaiseHeartsAnimation(initValue, heartsToAdd));
  }

  public IEnumerator ShowRaiseHeartsAnimation(int initValue, int heartsToAdd) {
    yield return new WaitForSeconds(0.8f);

    int finalValue = Mathf.Min(initValue + heartsToAdd, 6);
    int actualRaise = finalValue - initValue;

    for(int i=1; i<= actualRaise; i++) {
      VsnAudioManager.instance.PlaySfx("relationship_up");
      UpdateHeartIcons(initValue + i);
      heartIcons[initValue+i-1].transform.DOScale(1.4f, 0.4f).SetRelative().SetLoops(2, LoopType.Yoyo);
      yield return new WaitForSeconds(1.2f);
    }
    yield return new WaitForSeconds(0.8f);

    VsnController.instance.GotCustomInput();
    screenTransitions.HidePanel();


    VsnArgument[] sayArgs = new VsnArgument[2];
    sayArgs[0] = new VsnString("char_name/none");
    if(actualRaise > 1) {
      sayArgs[1] = new VsnString("add_heart/say_1");
    } else {
      sayArgs[1] = new VsnString("add_heart/say_0");
    }
    Command.SayCommand.StaticExecute(sayArgs);
  }
}
