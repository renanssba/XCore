using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimMaterialValue : MonoBehaviour {
  public Material material;
  public string key;

  public float startValue;
  public float endValue;
  public float duration;

  private float time;


  void Update() {
    time += Time.deltaTime;
    if(time > duration) {
      time -= duration;
    }
    material.SetFloat(key, startValue + (time/duration)*(endValue-startValue));
  }
}
