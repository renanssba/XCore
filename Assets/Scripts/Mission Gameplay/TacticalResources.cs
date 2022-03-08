using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalResources : MonoBehaviour {
  public static TacticalResources instance;

  public Sprite[] faceSprites;


  public void Awake() {
    if(instance == null) {
      instance = this;
    } else if(instance != this) {
      Destroy(gameObject);
      return;
    }
    DontDestroyOnLoad(gameObject);
  }
}
