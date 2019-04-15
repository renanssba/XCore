using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSprite : MonoBehaviour {

  public Sprite[] sprites;
  public Image image;

  void Awake(){
    image.sprite = sprites[Random.Range(0, sprites.Length)];
  }
}
