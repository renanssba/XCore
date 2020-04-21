using System;
using UnityEngine;
using UnityEngine.UI;

public class VsnCharacter : MonoBehaviour{

  public Image faceImage;
	public string label;
  public GameObject sighPrefab;

	public void SetData(Sprite sprite, string argLabel){
    faceImage.sprite = sprite;
    label = argLabel;

    if(sprite.name.Contains("tired")) {
      Sigh();
    }
	}

  public void Sigh() {
    Instantiate(sighPrefab, transform);
  }
}


