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
  public TextMeshProUGUI[] relationshipLevelTexts;

  public int slotId;


  public void OnEnable() {
    UpdateUI(slotId);
  }

  public void UpdateUI(int id) {
    shade.SetActive(false);
    if(slotId == 0) {
      titleText.text = "Auto-Save";
      shade.SetActive(SystemScreen.instance!=null && SystemScreen.instance.isInSaveMode);
    } else {
      titleText.text = "File " + slotId;
    }
    if(VsnSaveSystem.IsSaveSlotBusy(slotId)) {
      emptyIcon.SetActive(false);
      Dictionary<string, string> dic = VsnSaveSystem.GetSavedDictionary(slotId);

      playtimeText.text = Utils.GetTimeFormattedAsString(float.Parse(dic["VARNUMBER_playtime"])/60);
      dayText.text = Lean.Localization.LeanLocalization.GetTranslationText("ui/day") + " " + int.Parse(dic["VARNUMBER_day"]);
      moneyText.text = "<sprite=\"Attributes\" index=4>" + int.Parse(dic["VARNUMBER_money"]);
      humanFaceImages[0].sprite = ResourcesManager.instance.GetFaceSprite(0);
      humanFaceImages[1].sprite = ResourcesManager.instance.GetFaceSprite(0);
      humanFaceImages[2].sprite = ResourcesManager.instance.GetFaceSprite(0);

      humanFaceImages[3].sprite = ResourcesManager.instance.GetFaceSprite(5);
      humanFaceImages[4].sprite = ResourcesManager.instance.GetFaceSprite(6);
      humanFaceImages[5].sprite = ResourcesManager.instance.GetFaceSprite(7);

      //relationshipLevelTexts[0].text = GlobalData.instance.

      dataContent.SetActive(true);
    } else {
      emptyIcon.SetActive(true);
      dataContent.SetActive(false);
    }
  }


  public void Clicked() {
    Debug.LogWarning("Clicked entry " + slotId);

    if(SystemScreen.instance != null && SystemScreen.instance.isInSaveMode) {
      /// SAVE GAME
      if(slotId == 0) {
        SfxManager.StaticPlayForbbidenSfx();
        return;
      }
      GlobalData.instance.SavePersistantGlobalData();
      VsnSaveSystem.Save(slotId);
      VsnController.instance.StartVSN("action_descriptions", new VsnArgument[] { new VsnString("saved_successfully") });
    } else {
      /// LOAD GAME
      VsnSaveSystem.SetVariable("save_file_selected", slotId);
      VsnController.instance.StartVSN("action_descriptions", new VsnArgument[] { new VsnString("system_load") });
    }
    UpdateUI(slotId);
    if(MenuController.instance != null) {
      MenuController.instance.myPanel.CloseMenuScreen();
    }
  }
}
