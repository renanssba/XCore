using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICTrashBin : ICInteractable {

  public override void Interact() {
    ICGameController gc = ICGameController.instance;

    if (gc.currentIceCream.Count == 0) {
      SfxManager.StaticPlayForbbidenSfx();
      return;
    }

    gc.currentIceCream.Clear();
    gc.UpdateUI();

    SfxManager.StaticPlayConfirmSfx();

    return;
  }

}
