using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class CustomizationController : MonoBehaviour {

  public static CustomizationController instance;

  public WebCamTexture webcam;
  public RawImage camImage;

  public Image[] characterImages;

  public int customizingCharacter;


  public TMP_InputField[] characterInputNameText;

  public TMP_InputField imageSearchInputField;
  public Image searchResultImage;
  public GameObject searchResultContent;
  public GameObject searchResultPrefab;

  public Sprite[] defaultSprites;
  public string[] defaultNames;

  public GameObject customizationPanel;
  public GameObject loadingIcon;

  public RenderTexture portraitRenderTexture;

  public Texture2D portraitTexture;

  public ScreenContext customScreenContext;

  public GameObject portraitCamera;


  public void Awake() {
    instance = this;
    ClickResetBoy();
    ClickResetGirl();
  }

  void Start () {
    characterImages[0].sprite = defaultSprites[0];
    characterImages[1].sprite = defaultSprites[1];
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
    
    loadingIcon.SetActive(true);

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
    loadingIcon.SetActive(false);
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


	
	public void ClickCustomizeBoyButton(){
    customizingCharacter = 0;
    OpenCustomizationPanel();
  }

  public void ClickCustomizeGirlButton() {
    customizingCharacter = 1;
    OpenCustomizationPanel();
  }



  public void OpenCustomizationPanel() {
    customizationPanel.SetActive(true);
    imageSearchInputField.text = "";
    ResetSearchResults();
    InitializeWebcam();
    JoystickController.instance.AddContext(customScreenContext);
  }

  public void InitializeWebcam(){
    WebCamDevice[] devices = WebCamTexture.devices;
    if (devices.Length <= 0) {
      Debug.LogError("NO CAMERA FOUND.");
      return;
    }

    Debug.LogWarning("Using camera " + devices[0].name);
    webcam = new WebCamTexture(devices[0].name, 356, 200);
    Debug.Log("Did not break");

    webcam.Play();
    camImage.texture = webcam;
  }

  public void CloseCustomizationPanel() {
    customizationPanel.SetActive(false);
    webcam.Stop();
    JoystickController.instance.RemoveContext();
  }


  public void ClickResetBoy() {
    characterImages[0].sprite = defaultSprites[0];
    characterInputNameText[0].text = defaultNames[0];
  }

  public void ClickResetGirl() {
    characterImages[1].sprite = defaultSprites[1];
    characterInputNameText[1].text = defaultNames[1];
  }



  public void ClickUseWebcamSprite(){
    SetImageSprite(characterImages[customizingCharacter]);
    CloseCustomizationPanel();
  }

  public void SetCharacterSprite(Sprite sprite){
    characterImages[customizingCharacter].sprite = sprite;
    CloseCustomizationPanel();
  }


  public void SetImageSprite(Image img){
    Texture2D tex = new Texture2D(320, 240, TextureFormat.ARGB32, false);
    Graphics.CopyTexture(webcam, tex);

    img.sprite = Sprite.Create(tex, new Rect(40, 0, 240, 240), new Vector2(0.5f, 0.5f));
  }


  public void ClickPlayButton(){
    ModsManager modsManager = ModsManager.instance;

    modsManager.setFaces = new Sprite[10];
    modsManager.setNames = new string[10];
    for(int i = 0; i < 10; i++) {
      modsManager.setFaces[i] = null;
      modsManager.setNames[i] = null;
    }

    StartCoroutine(TakePortraits());

    //modsManager.setFaces[0] = characterImages[0].sprite;
    //modsManager.setFaces[5] = characterImages[1].sprite;
    modsManager.setNames[0] = char.ToUpper(characterInputNameText[0].text[0]) + characterInputNameText[0].text.Substring(1);;
    modsManager.setNames[5] = char.ToUpper(characterInputNameText[1].text[0]) + characterInputNameText[1].text.Substring(1);

    if(webcam != null) {
      webcam.Stop();
    }

    //VsnController.instance.StartVSN("got");
    //SceneManager.LoadScene(StageName.Park.ToString());
    VsnController.instance.StartVSN("goto_gameplay");
  }

  public IEnumerator TakePortraits() {
    for(int i=0; i<5;i++) {
      SetPortraitCameraPosition(2);
      yield return new WaitForEndOfFrame();
      TakePortrait(i);
      yield return new WaitForEndOfFrame();
    }
    for(int i = 0; i<5; i++) {
      SetPortraitCameraPosition(3);
      yield return new WaitForEndOfFrame();
      TakePortrait(5+i);
      yield return new WaitForEndOfFrame();
    }
    SetPortraitCameraPosition(0);
    yield return new WaitForEndOfFrame();
    TakePortrait(0);
    yield return new WaitForEndOfFrame();
    SetPortraitCameraPosition(1);
    yield return new WaitForEndOfFrame();
    TakePortrait(5);
  }

  public void SetPortraitCameraPosition(int posx) {
    Vector3 v = portraitCamera.transform.localPosition;
    v.x = -1.9f + posx * 3.8f;
    portraitCamera.transform.localPosition = v;
  }

  public void TakePortrait(int id) {
    RenderTexture.active = portraitRenderTexture;
    portraitTexture = new Texture2D(portraitRenderTexture.width, portraitRenderTexture.height);
    portraitTexture.ReadPixels(new Rect(0, 0, portraitRenderTexture.width, portraitRenderTexture.height), 0, 0);
    portraitTexture.Apply();

    Sprite face = Sprite.Create(portraitTexture, new Rect(0, 0, portraitRenderTexture.width, portraitRenderTexture.height), Vector2.zero);
    ModsManager.instance.setFaces[id] = face;
  }


  public void ClickPortraitButton() {
    Debug.LogWarning("Taking portrait now!");

    RenderTexture.active = portraitRenderTexture;
    portraitTexture = new Texture2D(portraitRenderTexture.width, portraitRenderTexture.height);
    portraitTexture.ReadPixels(new Rect(0, 0, portraitRenderTexture.width, portraitRenderTexture.height), 0, 0);
    portraitTexture.Apply();

    Sprite face = Sprite.Create(portraitTexture, new Rect(0, 0, portraitRenderTexture.width, portraitRenderTexture.height), Vector2.zero);
    characterImages[0].sprite = face;
  }
}
