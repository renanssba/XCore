using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class JumpingParticle : MonoBehaviour {

  public float jumpForce;
  public float duration;
  public float fadeDuration;

  public Vector3 finalPosition;

	public void Start() {
    if(finalPosition == Vector3.zero) {
      finalPosition = transform.position;
    }
    transform.DOJump(finalPosition, jumpForce, 1, duration).OnComplete(()=> {
      TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>();
      foreach(TextMeshPro t in texts) {
        t.DOFade(0f, fadeDuration);
      }

      SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
      foreach(SpriteRenderer sr in renderers) {
        sr.DOFade(0f, fadeDuration);
      }

      StartCoroutine(WaitThenBeDestroyed(fadeDuration));
    });
  }

  public IEnumerator WaitThenBeDestroyed(float time) {
    yield return new WaitForSeconds(time);
    Destroy(gameObject);
  }
}
