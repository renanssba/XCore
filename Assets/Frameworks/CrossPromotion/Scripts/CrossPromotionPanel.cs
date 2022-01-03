using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;


public class CrossPromotionPanel : MonoBehaviour {

  public static CrossPromotionPanel instance;

  public Button crosspromotionButton;
  public CanvasGroup crosspromotionPanel;
  public Transform crosspromotionPanelContent;
  public GameObject crosspromotionEntryPrefab;
  public TextAsset content;
  public string crosspromotionCollectionUrl = "https://supernova.games/crosspromotion-mobile/crosspromotion_json.txt";

  public CrossPromotionCollection currentCollection;
  
  public void Awake() {
    instance = this;
  }

  public void Start () {
    Debug.LogWarning("Loading initial content: " + content);
    InitializeEntries(content.text);
    StartCoroutine(DownloadCrosspromotionFile());
  }

  public void InitializeEntries(string content) {
    CrossPromotionFile crosspromotionFile = JsonUtility.FromJson<CrossPromotionFile>(content);
    LoadCurrentCollection(crosspromotionFile);

    ClearEntries();
    InstantiateEntries(currentCollection);
  }

  public void LoadCurrentCollection(CrossPromotionFile file) {
    foreach(CrossPromotionCollection col in file.collections) {
      foreach(string g in col.games) {
        if(string.Compare(g, Application.productName) == 0) {
          currentCollection = col;
          return;
        }
      }
    }

    foreach(CrossPromotionCollection col in file.collections) {
      foreach(string g in col.games) {
        if(string.Compare(g, "default") == 0) {
          currentCollection = col;
          return;
        }
      }
    }
  }

  public void ClearEntries() {
    int childCount = crosspromotionPanelContent.transform.childCount;

    for(int i = 0; i < childCount; i++) {
      Destroy(crosspromotionPanelContent.transform.GetChild(i).gameObject);
    }
  }

  public void InstantiateEntries(CrossPromotionCollection linksFile) {
    for(int i=0; i < linksFile.entries.Length; i++) {
      if(string.IsNullOrEmpty(CrossPromotionEntry.UrlToLoad(linksFile.entries[i])) ||
         string.Compare(linksFile.entries[i].name, Application.productName) == 0) {
        continue;
      }

      GameObject newEntry = Instantiate(crosspromotionEntryPrefab, crosspromotionPanelContent) as GameObject;
      newEntry.GetComponent<CrossPromotionEntry>().SetContent(linksFile.entries[i]);
    }
  }


	
	public void ClickOpenPanel() {
    if(DOTween.IsTweening(crosspromotionPanel) == false) {
      crosspromotionButton.gameObject.SetActive(false);
      SfxManager.StaticPlayConfirmSfx();

      ShowCrosspromotionScreen();
    }
  }

  public void CloseScreen() {
    if(DOTween.IsTweening(crosspromotionPanel) == false) {
      crosspromotionButton.gameObject.SetActive(true);
      SfxManager.StaticPlayCancelSfx();

      HideCrosspromotionScreen();
    }
  }

  public void ShowCrosspromotionScreen() {
    crosspromotionPanel.gameObject.SetActive(true);
    crosspromotionPanel.DOFade(1f, 0.2f).SetUpdate(true);
  }

  public void HideCrosspromotionScreen() {
    crosspromotionPanel.DOFade(0f, 0.2f).SetUpdate(true).OnComplete( ()=> {
      crosspromotionPanel.gameObject.SetActive(false);
    });
  }



  public IEnumerator DownloadCrosspromotionFile() {
    Debug.LogWarning("Starting to load crosspromotion collection: " + crosspromotionCollectionUrl);
    UnityWebRequest req = UnityWebRequest.Get(crosspromotionCollectionUrl);
    yield return req.Send();

    if(req.isNetworkError) {
      Debug.LogWarning("Error getting collection: " + req.error);
    } else {
      Debug.LogWarning("Received content: " + req.downloadHandler.text);
      Debug.LogWarning("Received collection successfully! Updating now...");
      InitializeEntries(req.downloadHandler.text);
    }
  }

  public IEnumerator DownloadImage(string image_url, Image gameImage) {
    Debug.LogWarning("Starting to load image: " + image_url);
    UnityWebRequest req = UnityWebRequestTexture.GetTexture(image_url);
    yield return req.Send();

    if(req.isNetworkError) {
      Debug.LogWarning("Error getting image: " + req.error);
    } else {
      Texture2D t = DownloadHandlerTexture.GetContent(req);
      gameImage.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
      Debug.LogWarning("Received image successfully!");
    }
  }
}
