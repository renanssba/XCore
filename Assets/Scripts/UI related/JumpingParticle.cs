using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class JumpingParticle : MonoBehaviour {

  public float jumpForce;
  public float duration;
  public float fadeDuration;

	public void Start() {
    transform.DOJump(transform.position, jumpForce, 1, duration).OnComplete(()=> {
      TextMeshProUGUI t = GetComponent<TextMeshProUGUI>();
      t.DOFade(0f, fadeDuration).OnComplete(() => {
        //Destroy(gameObject);
        gameObject.SetActive(false);
      });
    });
  }
}
