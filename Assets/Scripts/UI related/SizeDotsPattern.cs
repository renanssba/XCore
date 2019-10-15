using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SizeDotsPattern : MonoBehaviour {

  public float minSize;
  public float maxSize;

  public Transform[] rows;

  [ExecuteInEditMode]
  public void Start (){
    SizeDots();
  }
	
  [ExecuteInEditMode]
  public void Update (){
    SizeDots();
  }

  void SizeDots() {
    Debug.Log("Size Dots in pattern");

    rows = new Transform[transform.childCount];
    for(int i = 0; i < transform.childCount; i++) {
      rows[i] = transform.GetChild(i);
      Image[] imgs = rows[i].GetComponentsInChildren<Image>();
      for(int j = 0; j < imgs.Length; j++) {
        imgs[j].transform.localScale = (minSize + (maxSize-minSize)*((float)i/24) ) * Vector3.one;
      }
    }
  }
}
