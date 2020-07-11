using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSprite : MonoBehaviour {

  public Sprite[] sprites;
  public Image image;
  public SpriteRenderer sRenderer;

  void Awake(){
    if(image != null) {
      image.sprite = sprites[Random.Range(0, sprites.Length)];
    }
    if(sRenderer != null) {
      sRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }
  }
}
