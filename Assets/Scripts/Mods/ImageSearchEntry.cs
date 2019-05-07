using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSearchEntry : MonoBehaviour {

  public Image img;

	public void ClickUseImage(){
    CustomizationController.instance.SetCharacterSprite(img.sprite);
  }
}
