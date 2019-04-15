using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Decoration : MonoBehaviour {

  public Vector2 movementSpeed;
  public RectTransform screenTransform;

  public RectTransform myTransform;

	
  void Update(){
    myTransform.anchoredPosition += movementSpeed*Time.deltaTime;
    KeepInTheLimits();
	}

  void KeepInTheLimits(){
    if(myTransform.anchoredPosition.x < -screenTransform.sizeDelta.x/2f){
      myTransform.anchoredPosition += new Vector2(screenTransform.sizeDelta.x, 0f);
    }
    if(myTransform.anchoredPosition.x > screenTransform.sizeDelta.x/2f){
      myTransform.anchoredPosition -= new Vector2(screenTransform.sizeDelta.x, 0f);
    }

    if(myTransform.anchoredPosition.y < -screenTransform.sizeDelta.y/2f){
      myTransform.anchoredPosition += new Vector2(0f, screenTransform.sizeDelta.y);
    }
    if(myTransform.anchoredPosition.y > screenTransform.sizeDelta.y/2f){
      myTransform.anchoredPosition -= new Vector2(0f, screenTransform.sizeDelta.y);
    }
  }
}
