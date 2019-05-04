using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WebcamCapture : MonoBehaviour {

  public static WebcamCapture instance;

  public WebCamTexture webcam;
  public RawImage camImage;

  public Image boyImage;
  public Image girlImage;

  public TMP_InputField boyNameInputField;
  public TMP_InputField girlNameInputField;

  public TMP_InputField imageSearchInputField;
  public Image searchResultImage;
  public GameObject searchResultContent;
  public GameObject searchResultPrefab;

  public Sprite[] defaultSprites;


  public void Awake() {
    instance = this;
  }

  void Start () {
    WebCamDevice[] devices = WebCamTexture.devices;
    if(devices.Length <= 0){
      Debug.LogError("NO CAMERA FOUND.");
      return;
    }

    boyImage.sprite = defaultSprites[0];
    girlImage.sprite = defaultSprites[1];

    Debug.LogWarning("Using camera " + devices[0].name);
    webcam = new WebCamTexture(devices[0].name, 356, 200);
    Debug.Log("Did not break");

    webcam.Play();
    camImage.texture = webcam;
  }

  public void Update() {
    if(Input.GetKeyDown(KeyCode.Return)) {
      Debug.Log("PRESSED ENTER");
      if(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == imageSearchInputField.gameObject) {
        Debug.Log("imageSearchInputField IS SELECTED");
        ClickSearchButton();
      }
    }

    if (Input.GetKeyDown(KeyCode.F5)) {
      webcam.Stop();
      SceneManager.LoadScene(StageName.TitleScreen.ToString());
    }
  }

  public void ClickSearchButton(){
    StartCoroutine(ImageGoogleSearch( WWW.EscapeURL(imageSearchInputField.text) ));
  }

  IEnumerator ImageGoogleSearch(string _name){
    string query = "http://images.google.com/images?q=" + _name + "&hl=en&imgsz=Large";
    WWW wwwHtml = new WWW(query);

    string fullSearchResult;

    Debug.Log("Start loading " + query);

    yield return wwwHtml;
    fullSearchResult = wwwHtml.text;

    Debug.Log("RAW HTML: " + fullSearchResult);

    string[] splitElementArray = new string[] { "<img" };
    string[] parts = fullSearchResult.Split(splitElementArray, System.StringSplitOptions.None);

    ResetSearchResults();
    for(int i=1; i<parts.Length; i++){
      Debug.Log("part: " + parts[i]);
      StartCoroutine(CleanAndAddSprite(parts[i]));
    }
  }

  public void ResetSearchResults() {
    int childCount = searchResultContent.transform.childCount;

    Debug.Log("Resetting search results");

    for (int i = 0; i < childCount; i++) {
      Destroy(searchResultContent.transform.GetChild(i).gameObject);
    }
  }

  public IEnumerator CleanAndAddSprite(string imgPart){
    string imgUrl = imgPart;

    int endIndex = imgUrl.IndexOf(">");
    imgUrl = imgUrl.Substring(0, endIndex + 1);
    Debug.LogWarning("IMAGE URL: " + imgUrl);

    int startIndex = imgUrl.IndexOf("src=");
    imgUrl = imgUrl.Substring(startIndex + 5);

    endIndex = imgUrl.IndexOf("\"");
    imgUrl = imgUrl.Substring(0, endIndex);

    Debug.LogWarning("final URL: " + imgUrl);

    WWW queriedImage = new WWW(imgUrl);
    yield return queriedImage;

    Texture2D tex = queriedImage.texture;

    GameObject newObj = Instantiate(searchResultPrefab, searchResultContent.transform);
    newObj.GetComponent<Image>().sprite = GetSquareSprite(tex);
  }

  public Sprite GetSquareSprite(Texture2D tex){
    int side = Mathf.Min(tex.width, tex.height);
    return Sprite.Create(tex, new Rect((tex.width - side)/2, (tex.height - side) / 2, side, side), Vector2.zero);
  }


	
	public void ClickBoyButton(){
    SetImageSprite(boyImage);
  }

  public void ClickGirlButton() {
    SetImageSprite(girlImage);
  }

  public void SetImageSprite(Image img){
    Texture2D tex = new Texture2D(320, 240, TextureFormat.ARGB32, false);
    Graphics.CopyTexture(webcam, tex);

    img.sprite = Sprite.Create(tex, new Rect(40, 0, 240, 240), new Vector2(0.5f, 0.5f));
  }

  public void ClickPlayButton(){
    ModsManager modsManager = ModsManager.instance;

    ModsManager.instance.modsPath = "PATH NOVO";

    modsManager.setFaces = new Sprite[2];
    modsManager.setFaces[0] = boyImage.sprite;
    modsManager.setFaces[1] = girlImage.sprite;
    
    modsManager.setNames = new string[2];
    modsManager.setNames[0] = char.ToUpper(boyNameInputField.text[0]) + boyNameInputField.text.Substring(1);;
    modsManager.setNames[1] = char.ToUpper(girlNameInputField.text[0]) + girlNameInputField.text.Substring(1);
    
    webcam.Stop();

    //VsnController.instance.StartVSN("got");
    SceneManager.LoadScene(StageName.Gameplay.ToString());
  }
}
