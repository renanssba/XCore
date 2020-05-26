using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SaveSlotEntry : MonoBehaviour {

  public TextMeshProUGUI titleText;
  public TextMeshProUGUI contentText;
  public GameObject emptyIcon;
  public bool currentlySaving = true;

  public int slotId;


  public void OnEnable() {
    Initialize(slotId);
  }

  public void Initialize(int id) {
    titleText.text = "Arquivo " + (slotId+1);
    if(VsnSaveSystem.IsSaveSlotBusy(slotId)) {
      emptyIcon.SetActive(false);
      Dictionary<string, string> dic = VsnSaveSystem.GetSavedDictionary(slotId);
      contentText.text = SaveSlotText(dic);
      contentText.gameObject.SetActive(true);
    } else {
      emptyIcon.SetActive(true);
      contentText.gameObject.SetActive(false);
    }
  }

  public string SaveSlotText(Dictionary<string, string> dic) {
    string saveString;
    //foreach(KeyValuePair<string, string> entry in dic) {
    //  Debug.LogWarning("entry: " + entry.Key + ", " + entry.Value);
    //}
    saveString = "Dia: " + dic["VARNUMBER_day"] + "\nDinheiro: " + dic["VARNUMBER_money"];
    return saveString;
  }

  public void Clicked() {
    Debug.LogWarning("Clicked entry " + slotId);
    if(currentlySaving == true) {
      GlobalData.instance.SaveVsnVariables();
      VsnSaveSystem.Save(slotId);
      VsnController.instance.StartVSN("action_descriptions", new VsnArgument[] { new VsnString("saved_successfully") });
    } else {
      VsnSaveSystem.Load(slotId);
      GlobalData.instance.LoadVsnVariables();
      VsnController.instance.StartVSN("action_descriptions", new VsnArgument[] { new VsnString("loaded_successfully") });
    }
    Initialize(slotId);
    UIController.instance.cellphonePanel.HidePanel();
  }
}
