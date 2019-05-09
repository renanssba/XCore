using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IceCreamFlavor{
  cone,
  chocolate,
  mint,
  strawberry
}


public class ICInteractable : MonoBehaviour {

  public void OnTriggerEnter(Collider collider) {
    Debug.Log("Entered interactable object!");
    ICGameController.instance.SelectInteractable(this);
  }

  public void OnTriggerExit(Collider other) {
    Debug.Log("Exited interactable object.");
    ICGameController.instance.DeselectInteractable();
  }

  public virtual void Interact(){
    Debug.LogWarning("Did nothing!");
    return;
  }
}


public class ICIceCreamContainer : ICInteractable {

  public IceCreamFlavor flavor;

  public override void Interact() {
    Debug.LogWarning("Got " + flavor + " ice cream!");
    ICGameController gc = ICGameController.instance;
    
    if(flavor == IceCreamFlavor.cone && gc.currentIceCream.Count > 0){
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    if (flavor != IceCreamFlavor.cone && gc.currentIceCream.Count < 1) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }
    if (gc.currentIceCream.Count >= 4) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    gc.currentIceCream.Add(flavor);
    gc.UpdateUI();

    SfxManager.StaticPlayConfirmSfx();

    return;
  }
}
