using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ICClient : ICInteractable {

  public int clientId;
  public Image[] iceCreamBall;

  public List<IceCreamFlavor> flavorsAsked;

  public float timeWaiting;
  public float totalTimeToWait;

  public bool available = false;


  public void Start(){
    available = false;
    
  }

  public void Initialize(){
    flavorsAsked = new List<IceCreamFlavor>();
    flavorsAsked.Add(IceCreamFlavor.cone);
    int numberOfBalls = Random.Range(2, 4);
    for (int i = 0; i < numberOfBalls; i++) {
      flavorsAsked.Add((IceCreamFlavor)Random.Range(1, 4));
    }

    for (int i = 0; i < iceCreamBall.Length; i++) {
      if(i<flavorsAsked.Count) {
        iceCreamBall[i].gameObject.SetActive(true);
        iceCreamBall[i].color = ICGameController.instance.iceCreamFlavorColors[(int)flavorsAsked[i]];
        iceCreamBall[i].transform.parent.parent.gameObject.SetActive(true);
      } else{
        iceCreamBall[i].gameObject.SetActive(false);
      }
    }
  }

  public void Appear() {
    available = false;
    transform.DOMoveX(-7f, 2f).OnComplete(() => {
      Initialize();
    });
  }



  public override void Interact() {
    Debug.LogWarning("Gave ice cream to client!");

    ICGameController gc = ICGameController.instance;

    if (gc.currentIceCream.Count == 0) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    switch(CompareIceCreams(flavorsAsked, gc.currentIceCream)) {
      case 0:
        SfxManager.StaticPlayCancelSfx();
        break;
      case 1:
        SfxManager.StaticPlayConfirmSfx();
        ICGameController.instance.AddScore(50);
        break;
      case 2:
        SfxManager.StaticPlayBigConfirmSfx();
        ICGameController.instance.AddScore(100);
        break;
    }

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
    iceCreamBall[0].transform.parent.parent.gameObject.SetActive(false);

    transform.DOMoveX(-10f, 2f).OnComplete( ()=> {
      gameObject.SetActive(false);
    } );
  }

}
