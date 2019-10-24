using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DieAfterTime : MonoBehaviour {

  private ParticleSystem particleSystem;
  public float timeToWait;

  void Awake(){
    ActivateAndDie();
  }

  public void ActivateAndDie(){
    particleSystem = GetComponent<ParticleSystem>();
    if(particleSystem != null){
      particleSystem.Play();
    }

    StartCoroutine(WaitAndDie());
  }

  public IEnumerator WaitAndDie() {
    yield return new WaitForSeconds(timeToWait);
    if(particleSystem != null) {
      particleSystem.Stop();
    }
    Destroy(gameObject);
  }
}
