using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct CrossPromotionFile {
  public CrossPromotionCollection[] collections;
}

[System.Serializable]
public struct CrossPromotionCollection{
  public CrossPromotionEntryContent[] entries;
  public string[] games;
  public string collection_name;
  public string version;
}

[System.Serializable]
public struct CrossPromotionEntryContent {
  public string name;
  public string description;
  public string image_url;
  public string android_link;
  public string ios_link;
  public string web_link;
  public string standalone_link;
}


public class CrossPromotionEntry : MonoBehaviour {

  public CrossPromotionEntryContent content;
  public TextMeshProUGUI descriptionText;
  public GameObject textPanel;
  public Image gameImage;
  public Image descriptionDarkBg;


  public void SetContent(CrossPromotionEntryContent  newContent) {
    content = newContent;
    UpdateUI();
  }

  public void UpdateUI() {
    if(!string.IsNullOrEmpty(content.description)) {
      descriptionDarkBg.gameObject.SetActive(true);
      descriptionText.text = content.description;
    } else {
      descriptionDarkBg.gameObject.SetActive(false);
      descriptionText.text = "";
    }
    textPanel.SetActive(false);
    textPanel.SetActive(true);

    CrossPromotionPanel.instance.StartCoroutine(CrossPromotionPanel.instance.DownloadImage(content.image_url, gameImage));
  }


  public void ClickedButton() {
    Debug.LogWarning("Clicked crosspromotion button for game: " + content.name);
    LoadLink();
  }

  public void LoadLink() {
    Application.OpenURL(UrlToLoad(content));
  }

  public static string UrlToLoad(CrossPromotionEntryContent content) {
    string urlToLoad = "";

    switch(Application.platform) {
      case RuntimePlatform.Android:
        urlToLoad = content.android_link;
        break;
      case RuntimePlatform.IPhonePlayer:
        urlToLoad = content.ios_link;
        break;
      case RuntimePlatform.WebGLPlayer:
        urlToLoad = content.ios_link;
        break;
      case RuntimePlatform.WindowsEditor:
      case RuntimePlatform.WindowsPlayer:
      case RuntimePlatform.OSXPlayer:
        urlToLoad = content.standalone_link;
        break;
    }
    return urlToLoad;
  }
}
