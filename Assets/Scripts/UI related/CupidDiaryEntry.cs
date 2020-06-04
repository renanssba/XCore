using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CupidDiaryEntry : MonoBehaviour {
  public TextMeshProUGUI title;
  public TextMeshProUGUI description;
  public Image[] personImages;
  public GameObject[] babiesIcons;
  public VsnConsoleSimulator consoleSimulator;
  public GameObject photoCover;


  public void Initialize(Relationship r) {
    if(r == null) {
      SetEmpty();
      return;
    }

    int babiesCount = Relationship.childrenNumber[r.id];

    VsnSaveSystem.SetVariable("babies", babiesCount);

    personImages[0].sprite = ResourcesManager.instance.GetCharacterSprite(r.GetBoy().id, CharacterSpritePart.body);
    personImages[1].sprite = ResourcesManager.instance.GetCharacterSprite(r.GetBoy().id, CharacterSpritePart.casual);
    personImages[2].sprite = ResourcesManager.instance.GetCharacterSprite(r.GetGirl().id, CharacterSpritePart.body);
    personImages[3].sprite = ResourcesManager.instance.GetCharacterSprite(r.GetGirl().id, CharacterSpritePart.casual);
    photoCover.SetActive(false);

    for(int i = 0; i < babiesIcons.Length; i++) {
      if(i < babiesCount) {
        babiesIcons[i].SetActive(true);
      } else {
        babiesIcons[i].SetActive(false);
      }
    }
    GlobalData.instance.currentRelationshipId = r.id;
    title.text = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("char_name/couple"));
    description.text = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("couple_epilogue/" + r.GetGirl().nameKey));

    consoleSimulator.callAfterShowCharacters = CupidDiaryController.instance.ShowContinueButton;
    consoleSimulator.StartShowingCharacters();
  }

  public void SetEmpty() {
    photoCover.SetActive(true);
    title.text = "";
    description.text = "";
  }
}
