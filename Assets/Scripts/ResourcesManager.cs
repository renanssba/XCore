using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesManager : MonoBehaviour {
  public Sprite[] faceSprites;
  public Sprite[] cardSprites;
  public Sprite[] itemSprites;
  public Sprite[] attributeSprites;
  public Sprite unknownSprite;

  public static ResourcesManager instance;

  public void Awake() {
    instance = this;
  }

  public Sprite GetFaceSprite(int index){
    if (ModsManager.instance.setFaces != null && ModsManager.instance.setFaces.Length >= 2) {
      Debug.LogWarning("USING MODDED FACES");
      if (index < 5) {
        return ModsManager.instance.setFaces[0];
      } else {
        return ModsManager.instance.setFaces[1];
      }
    }
    return faceSprites[index];
  }
}
