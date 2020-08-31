using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateAroundTornado : MonoBehaviour {

  public Transform child;
  public SpriteRenderer renderer;
  public float orbitTime = 1f;

  public void Start () {
    float rot = Random.Range(0f, 360f);
    transform.eulerAngles = new Vector3(0f, rot, 0f);
    child = transform.GetChild(0);
    renderer = child.GetComponent<SpriteRenderer>();
    child.localEulerAngles = new Vector3(0f, -transform.localEulerAngles.y, 0f);

    transform.DORotate(new Vector3(0f, 360f, 0f), orbitTime).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1).OnUpdate(()=> {
      float toRotate = AngleToRotate(transform.localEulerAngles.y);
      child.localEulerAngles = new Vector3(0f, toRotate, 0f);

      //Debug.Log("rotation: "+ (PositiveAngle(transform.localEulerAngles.y) % 360f));

      if(child.transform.position.z > transform.parent.position.z) {
        renderer.sortingOrder = -1;
      } else {
        renderer.sortingOrder = 1;
      }
    });
	}

  public float PositiveAngle(float x) {
    if(x < 0f) {
      x += 360f;
    }
    return x;
  }

  public float AngleToRotate(float x) {
    if(x < 0f) {
      x += 360f;
    }
    return -x;
  }
}
