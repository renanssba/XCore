using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSlider : MonoBehaviour {

  public Slider backSlider;
  public Slider fillSlider;

  public static Color healColor = Color.green;
  public static Color damageColor = Color.red;

  public float impactTime = 0.5f;
  public float currentTime;

  public float movementTime = 1f;

  public float initialValue = 0f;
  public float finalValue = 0f;


  public void Start() {
    SetSliderValue(1f);
  }


  public void SetSliderValue(float value) {
    initialValue = fillSlider.value;
    finalValue = value;
    if(finalValue < initialValue) {
      fillSlider.value = value;
      currentTime = impactTime + movementTime;
      backSlider.fillRect.GetComponent<Image>().color = damageColor;
    } else {
      backSlider.value = value;
      currentTime = impactTime + movementTime;
      backSlider.fillRect.GetComponent<Image>().color = healColor;
    }
  }

  public void Update() {
    if(currentTime > 0f) {
      currentTime -= Time.deltaTime;
    }

    /// showcase damage
    if(finalValue < initialValue) {
      if(currentTime > movementTime) {

      } else if(currentTime > 0f) {
        backSlider.value = finalValue + (Diff() * currentTime/movementTime);
      } else {
        backSlider.value = finalValue;
      }
    } else {
      if(currentTime > movementTime) {

      } else if(currentTime > 0f) {
        fillSlider.value = finalValue + (Diff() * currentTime / movementTime);
      } else {
        fillSlider.value = finalValue;
      }
    }
    
    
	}

  public float Diff() {
    return initialValue - finalValue;
  }

  //public void SetValue(float v) {
  //  SetSliderValue
  //}
}
