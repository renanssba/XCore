using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesManager : MonoBehaviour {
  public Sprite[] faceSprites;
  public Sprite[] cardSprites;
  public Sprite[] itemSprites;
  public Sprite[] attributeSprites;
  public Sprite[] daytimeSprites;
  public GameObject[] baseActorPrefab;
  public Sprite unknownSprite;

  public Color[] attributeColor;

  public static ResourcesManager instance;

  public void Awake() {
    instance = this;
  }

  public Sprite GetFaceSprite(int index){
    if(ModsManager.instance.GetFaceSprite(index) != null) {
      Debug.LogWarning("USING MODDED FACES");
      return ModsManager.instance.GetFaceSprite(index);
    }
    return faceSprites[index];
  }
}
