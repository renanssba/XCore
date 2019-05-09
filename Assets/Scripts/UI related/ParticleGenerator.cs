using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleGenerator : MonoBehaviour {

  public GameObject particlePrefab;
  public int particlesToGenerate;

  public void OnEnable(){
    StartCoroutine(CreateParticles());
  }

  public IEnumerator CreateParticles(){
    for(int i=0; i<particlesToGenerate; i++){
      GenerateParticle();
      yield return new WaitForSeconds(0.5f);
    }
    yield return null;
  }

  public void GenerateParticle(){
    GameObject newObj = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity, transform) as GameObject;
    newObj.GetComponent<Particle>().Rotate(10f);

    RectTransform rect = newObj.GetComponent<RectTransform>();
    rect.anchoredPosition = Vector2.zero;
    rect.position = new Vector3(rect.position.x, rect.position.y, 0f);
  }

  public void DeleteSons(){
    for(int i=0; i<transform.childCount; i++){
      Destroy( transform.GetChild(i).gameObject );
    }
  }
}
