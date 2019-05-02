using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSearchEntry : MonoBehaviour {

  public Image img;

	public void ClickUseImageBoy(){
    WebcamCapture.instance.boyImage.sprite = img.sprite;
  }

  public void ClickUseImageGirl() {
    WebcamCapture.instance.girlImage.sprite = img.sprite;
  }
}
