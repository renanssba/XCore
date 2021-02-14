using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLSaveSystemTest : MonoBehaviour {

  public int slot;

  public void ClickedSaveButton() {
    VsnSaveSystem.Save(slot);
  }

  public void ClickedLoadButton() {
    VsnSaveSystem.Load(slot);
  }
}
