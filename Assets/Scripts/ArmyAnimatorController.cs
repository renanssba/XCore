using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyAnimatorController : MonoBehaviour {

  public Animator[] bodyAnimators;
  public float loopTime;

  public void Start() {
    foreach(Animator b in bodyAnimators) {
      StartCoroutine(StartAnimators(b, Random.Range(0f, loopTime)));
    }
  }

  public IEnumerator StartAnimators(Animator anim, float waitTime) {
    yield return new WaitForSeconds(waitTime);
    anim.enabled = true;
  }
}
