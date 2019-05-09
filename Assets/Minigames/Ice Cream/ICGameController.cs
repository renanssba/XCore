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

  public Color[] iceCreamFlavorColors;

  public Image[] currentIceCreamImages;
  public List<IceCreamFlavor> currentIceCream;

  public int score;

  public float spawnClientTime;
  public TextMeshProUGUI scoreText;


  void Awake() {
    instance = this;
    currentIceCream = new List<IceCreamFlavor>();
  }

  void Start(){
    score = 0;
    UpdateUI();
    StartCoroutine(SpawnClients());

    VsnAudioManager.instance.PlayMusic("observacao2_intro", "observacao2_loop");
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
        clients[i].gameObject.SetActive(true);
        clients[i].Appear();
        return;
      }
    }
  }
  
  public void Update() {
    if (Input.GetKeyDown(KeyCode.F5)) {
      GlobalData.instance.ResetCurrentCouples();
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

}
