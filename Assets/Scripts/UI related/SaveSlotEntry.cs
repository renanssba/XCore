using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SaveSlotEntry : MonoBehaviour {

  public TextMeshProUGUI titleText;
  public GameObject emptyIcon;
  public GameObject dataContent;
  public GameObject shade;
  //public bool currentlySaving = true;

  public TextMeshProUGUI playtimeText;
  public TextMeshProUGUI dayText;
  public TextMeshProUGUI moneyText;

  public Image[] humanFaceImages;
  public Image[] heartIcons;
  public TextMeshProUGUI[] relationshipLevelTexts;

  public int slotId;


  public void OnEnable() {
    UpdateUI(slotId);
  }

  public void UpdateUI(int id) {
    RelationshipSaveStruct relationshipStruct;
    Relationship rel = new Relationship();

    shade.SetActive(false);

    if(slotId == 0) {
      titleText.text = Lean.Localization.LeanLocalization.GetTranslationText("save_load_panel/auto_save");
      shade.SetActive(SystemScreen.instance!=null && SystemScreen.instance.isInSaveMode);
    } else {
      titleText.text = Lean.Localization.LeanLocalization.GetTranslationText("save_load_panel/file") + " " + slotId;
    }

    if(VsnSaveSystem.IsSaveSlotBusy(slotId)) {
      emptyIcon.SetActive(false);
      Dictionary<string, string> dic = VsnSaveSystem.GetSavedDictionary(slotId);

      if(dic.ContainsKey("VARNUMBER_playtime")) {
        playtimeText.text = Utils.GetTimeFormattedAsString(float.Parse(dic["VARNUMBER_playtime"]) / 60);
      } else {
        playtimeText.text = "??:??";
      }
      
      dayText.text = Lean.Localization.LeanLocalization.GetTranslationText("ui/day") + " " + int.Parse(dic["VARNUMBER_day"]);
      moneyText.text = "<sprite=\"Attributes\" index=4 tint=1>" + int.Parse(dic["VARNUMBER_money"]);
      humanFaceImages[0].sprite = ResourcesManager.instance.GetCharacterSprite(5, CharacterSpritePart.face);
      humanFaceImages[1].sprite = ResourcesManager.instance.GetCharacterSprite(6, CharacterSpritePart.face);
      humanFaceImages[2].sprite = ResourcesManager.instance.GetCharacterSprite(7, CharacterSpritePart.face);


      for(int i=0; i<2; i++) {
        relationshipStruct = JsonUtility.FromJson<RelationshipSaveStruct>(dic["VARSTRING_relationship_" + i]);
        relationshipLevelTexts[i].text = relationshipStruct.level.ToString();
        //heartIcons[i].sprite = ResourcesManager.instance.heartlockSprites[relationshipStruct.heartLocksOpened];
        if(relationshipStruct.level > 0) {
          humanFaceImages[i].gameObject.SetActive(true);
        } else {
          humanFaceImages[i].gameObject.SetActive(false);
        }
      }

      dataContent.SetActive(true);
    } else {
      emptyIcon.SetActive(true);
      dataContent.SetActive(false);
    }
  }


  public void Clicked() {
    Debug.LogWarning("Clicked entry " + slotId);
    

    VsnSaveSystem.SetVariable("save_file_selected", slotId);
    if(SystemScreen.instance != null && SystemScreen.instance.isInSaveMode) {
      /// SAVE GAME
      if(slotId == 0) {
        SfxManager.StaticPlayForbbidenSfx();
        return;
      }
      if(HasData()) {
        VsnController.instance.StartVSN("save_load_functions", new VsnArgument[] { new VsnString("system_save") });
      } else {
        VsnController.instance.StartVSN("save_load_functions", new VsnArgument[] { new VsnString("confirm_save") });
      }
    } else {
      /// LOAD GAME
      if(!HasData()) {
        SfxManager.StaticPlayForbbidenSfx();
        return;
      }

      if(!ExecutingInTitleScreen()) {
        // this is used during 
        VsnController.instance.StartVSN("save_load_functions", new VsnArgument[] { new VsnString("system_load") });
      } else {
        VsnController.instance.StartVSN("save_load_functions", new VsnArgument[] { new VsnString("confirm_load") });
      }
    }
    UpdateUI(slotId);
    if(MenuController.instance != null) {
      MenuController.instance.myPanel.CloseMenuScreen();
    }
  }

  public bool ExecutingInTitleScreen() {
    return MenuController.instance == null;
  }

  public bool HasData() {
    return VsnSaveSystem.IsSaveSlotBusy(slotId);
  }
}
