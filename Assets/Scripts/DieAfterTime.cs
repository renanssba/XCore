using UnityEngine;
using System.Collections;

public class DieAfterTime : MonoBehaviour {

  private ParticleSystem particleSystem;
  public float timeToWait;

  void Awake() {
    StartCoroutine(ActivateAndDie());
  }

  public IEnumerator ActivateAndDie() {
    particleSystem = GetComponent<ParticleSystem>();
    if(particleSystem != null) {
      particleSystem.Play();
    }

    yield return new WaitForSeconds(timeToWait);

    if(particleSystem != null) {
      particleSystem.Stop();
    }
    Destroy(gameObject);
  }
}
