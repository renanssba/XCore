using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesManager : MonoBehaviour {
  public Sprite[] faceSprites;
  public Sprite[] cardSprites;
  public Sprite[] itemSprites;

  public static ResourcesManager instance;

  public void Awake() {
    instance = this;
  }
}
