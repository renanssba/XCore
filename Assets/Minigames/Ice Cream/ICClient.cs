using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ICClient : ICInteractable {

  public int clientId;
  public Image[] iceCreamBall;
  public Slider patienceSlider;
  public Color[] patienceLevelColors;

  public List<IceCreamFlavor> flavorsAsked;

  public float timeWaiting;
  public float totalTimeToWait;

  public bool waiting = false;


  public void Start(){
    waiting = false; 
  }

  public void Initialize(){
    flavorsAsked = new List<IceCreamFlavor>();
    flavorsAsked.Add(IceCreamFlavor.cone);
    int numberOfBalls = Random.Range(2, 4);
    for (int i = 0; i < numberOfBalls; i++) {
      flavorsAsked.Add((IceCreamFlavor)Random.Range(1, 4));
    }
    iceCreamBall[0].transform.parent.parent.gameObject.SetActive(true);

    for (int i = 0; i < iceCreamBall.Length; i++) {
      if(i<flavorsAsked.Count) {
        iceCreamBall[i].gameObject.SetActive(true);
        iceCreamBall[i].color = ICGameController.instance.iceCreamFlavorColors[(int)flavorsAsked[i]];
      } else{
        iceCreamBall[i].gameObject.SetActive(false);
      }
    }
    patienceSlider.maxValue = totalTimeToWait;
  }


  public void Update(){
    if(waiting) {
      timeWaiting += Time.deltaTime;

      patienceSlider.value = (totalTimeToWait - timeWaiting);
      patienceSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = patienceLevelColors[GetPatienceLevel()];

      if (timeWaiting >= totalTimeToWait) {
        timeWaiting = 0f;
        GoAway();
      }
    }

  }

  public int GetPatienceLevel(){
    if(timeWaiting >= totalTimeToWait * 2f / 3f){
      return 0;
    } else if (timeWaiting >= totalTimeToWait * 1f / 3f) {
      return 1;
    }
    return 2;
  }


  public void Appear() {
    gameObject.SetActive(true);

    DOTween.Kill(transform);
    transform.DOMoveX(-7f, 2f).OnComplete(() => {
      Initialize();
      timeWaiting = 0f;
      waiting = true;
    });
  }



  public override void Interact() {
    Debug.LogWarning("Gave ice cream to client!");

    ICGameController gc = ICGameController.instance;

    if (gc.currentIceCream.Count == 0) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    int score = 0;

    switch(GetPatienceLevel()) {
      case 2:
        score = 10;
        break;
      case 1:
        score = 8;
        break;
      case 0:
        score = 4;
        break;
    }

    switch(CompareIceCreams(flavorsAsked, gc.currentIceCream)) {
      case 0:
        SfxManager.StaticPlayCancelSfx();
        score *= 0;
        break;
      case 1:
        SfxManager.StaticPlayConfirmSfx();
        score /= 2;
        break;
      case 2:
        SfxManager.StaticPlayBigConfirmSfx();
        break;
    }
    ICGameController.instance.AddScore(score);

    gc.currentIceCream.Clear();
    gc.UpdateUI();

    GoAway();

    return;
  }

  public int CompareIceCreams(List<IceCreamFlavor> a, List<IceCreamFlavor> b) {
    if (a.Count != b.Count) {
      return 0;
    }

    bool allOk = true;
    for (int i=0; i<a.Count; i++){
      if(a[i] != b[i]){
        allOk = false;
      }    
    }
    if(allOk){
      return 2;
    }

    if(CountFlavor(a, IceCreamFlavor.chocolate) == CountFlavor(b, IceCreamFlavor.chocolate) &&
       CountFlavor(a, IceCreamFlavor.mint) == CountFlavor(b, IceCreamFlavor.mint) &&
       CountFlavor(a, IceCreamFlavor.strawberry) == CountFlavor(b, IceCreamFlavor.strawberry)) {
      return 1;
    }

    return 0;
  }
  
  public int CountFlavor(List<IceCreamFlavor> list, IceCreamFlavor counted) {
    int count = 0;
    foreach(IceCreamFlavor flavor in list) {
      if(flavor == counted){
        count++;
      }
    }
    return count;
  }

  public void GoAway(){
    waiting = false;
    iceCreamBall[0].transform.parent.parent.gameObject.SetActive(false);

    transform.DOMoveX(-10f, 2f).OnComplete( ()=> {
      gameObject.SetActive(false);
    } );
  }

}
