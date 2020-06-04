using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DiaryEntry {
  public string entryName;
  public TextMeshProUGUI title;
  public TextMeshProUGUI description;
  public Image[] personImages;
  public GameObject[] babiesIcons;


  public void Initialize(Relationship r) {
    int babiesCount = Relationship.childrenNumber[r.id];

    personImages[0].sprite = ResourcesManager.instance.GetCharacterSprite(r.GetBoy().id, CharacterSpritePart.body);
    personImages[1].sprite = ResourcesManager.instance.GetCharacterSprite(r.GetBoy().id, CharacterSpritePart.casual);
    personImages[2].sprite = ResourcesManager.instance.GetCharacterSprite(r.GetGirl().id, CharacterSpritePart.body);
    personImages[3].sprite = ResourcesManager.instance.GetCharacterSprite(r.GetGirl().id, CharacterSpritePart.casual);

    for(int i=0; i<babiesIcons.Length; i++) {
      if(i < babiesCount) {
        babiesIcons[i].SetActive(true);
      } else {
        babiesIcons[i].SetActive(false);
      }
    }
    GlobalData.instance.currentRelationshipId = r.id;
    title.text = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("char_name/couple"));
    description.text = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("couple_epilogue/"+r.GetGirl().nameKey));
  }
}


public class CupidDiaryController : MonoBehaviour {

  public DiaryEntry[] entries;
  public int currentPage = 0;


  public void OnEnable() {
    entries[0].Initialize(GlobalData.instance.relationships[0]);
    entries[1].Initialize(GlobalData.instance.relationships[1]);
  }
}
