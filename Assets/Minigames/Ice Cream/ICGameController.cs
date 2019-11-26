using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class ICGameController : MonoBehaviour {

  public static ICGameController instance;

  public GameObject[] containerIcons;
  public ICClient[] clients;
  public ICInteractable selectedInteractable = null;

  public TimeCounter timer;

  public Color[] iceCreamFlavorColors;

  public Image[] currentIceCreamImages;
  public List<IceCreamFlavor> currentIceCream;

  public int score;

  public float spawnClientTime;
  public TextMeshProUGUI scoreText;

  public Coroutine spawnCoroutine;

  public bool isPlaying;


  void Awake() {
    instance = this;
    currentIceCream = new List<IceCreamFlavor>();
  }

  void Start(){
    score = 0;
    UpdateUI();

    float duration = 0f;
    switch(VsnSaveSystem.GetIntVariable("ap_spent")){
      case 2:
      default:
        duration = 40f;
        break;
      case 3:
        duration = 60f;
        break;
      case 4:
        duration = 80f;
        break;
      case 5:
        duration = 100f;
        break;
    }

    timer.Initialize(true, duration);

    VsnAudioManager.instance.PlayMusic("minigame1_intro", "minigame1_loop");
    spawnCoroutine = StartCoroutine(SpawnClients());
    isPlaying = true;
  }



  public IEnumerator SpawnClients(){
    while(true){
      InstantiateClient();
      yield return new WaitForSeconds(spawnClientTime);
    }  
  }

  public void InstantiateClient(){
    for(int i=0; i<clients.Length; i++){
      if(clients[i].gameObject.activeSelf == false){
        clients[i].Appear();
        return;
      }
    }
  }
  
  public void Update() {
    if (Input.GetKeyDown(KeyCode.F5)) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }


  public void UpdateUI(){
    for(int i=0; i<currentIceCreamImages.Length; i++){
      if(i<currentIceCream.Count){
        currentIceCreamImages[i].gameObject.SetActive(true);
        currentIceCreamImages[i].color = iceCreamFlavorColors[(int)currentIceCream[i]];
      } else{
        currentIceCreamImages[i].gameObject.SetActive(false);
      }
    }
    scoreText.text = "SCORE:\n<size=80>"+score+"</size>";
  }


  public void SelectInteractable(ICInteractable interactable) {
    DeselectInteractable();
    selectedInteractable = interactable;

    ICIceCreamContainer selectedContainer = interactable.GetComponent<ICIceCreamContainer>();
    if(selectedContainer != null){
      containerIcons[(int)selectedContainer.flavor].SetActive(true);
    }
    if(interactable.GetComponent<ICTrashBin>() != null){
      containerIcons[4].SetActive(true);
    }
    //ICClient selectedClient = interactable.GetComponent<ICClient>();
    //if (selectedClient != null) {
    //  clientIcons[selectedClient.clientId].SetActive(true);
    //}
  }

  public void DeselectInteractable() {
    foreach (GameObject obj in containerIcons) {
      obj.SetActive(false);
    }
    //foreach (GameObject obj in clientIcons) {
    //  obj.SetActive(false);
    //}
    selectedInteractable = null;
  }



  public void ClickInteraction() {
    Debug.LogWarning("Clicked interaction!");

    if (selectedInteractable != null) {
      selectedInteractable.Interact();
    }
  }

  public void AddScore(int value) {
    score += value;
    UpdateUI();
  }

  public void EndMinigameTime(){
    isPlaying = false;
    StopCoroutine(spawnCoroutine);
    VsnSaveSystem.SetVariable("minigame_ended", 1);
    VsnSaveSystem.SetVariable("minigame_score", score);
    VsnController.instance.StartVSN("minigame_result");
  }
}
