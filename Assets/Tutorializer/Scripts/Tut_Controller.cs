using UnityEngine;
using System.Collections;

public class Tut_Controller : MonoBehaviour {

  private static Tut_Controller instance;

  public ListedAnimation[] animations;
//  public Tut_Sliding_Finger slidingFinger;

  [System.Serializable]
  public struct ListedAnimation{
    public GameObject arrow;
    public string name;
  };


  void Awake(){
    instance = this;
  }

  public static Tut_Controller GetInstance(){
    return instance;
  }


  public void StartAnimation(string animName){
    foreach(ListedAnimation anim in animations){
      if(anim.name == animName){
        anim.arrow.SetActive(true);
        return;
      }
    }
    Debug.LogError("Invalid animation: " + animName);
  }

  public void StartAnimationLimitedTime(string animName, float animTime){
    StartAnimation(animName);
    StartCoroutine(WaitThenStopAnimation(animName, animTime));
  }

  IEnumerator WaitThenStopAnimation(string animName, float animTime){
    yield return new WaitForSeconds(animTime);
    StopAnimation(animName);
  }

  public void StopAnimation(string animName){
    foreach(ListedAnimation anim in animations){
      if(anim.name == animName){
        anim.arrow.SetActive(false);
      }
    }
  }

  public void StopAllAnimation(){
    foreach(ListedAnimation anim in animations){
      anim.arrow.SetActive(false);
    }
  }

}
