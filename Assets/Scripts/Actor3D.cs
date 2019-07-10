using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor3D : MonoBehaviour {

  public List<Material> myMaterials;
  public List<Color> myColors;

  public void Start() {
    // TODO: FIX THIS to not create many copies for each material

    //Initialize(0);
  }


  public void ClearBody() {
    int childCount = transform.childCount;

    for(int i = 0; i < childCount; i++) {
      Destroy(transform.GetChild(i).gameObject);
      //itemsHolder.transform.GetChild(i).gameObject.SetActive(false);
    }
  }

  public void SetGraphics(Person person) {
    ClearBody();
    GameObject actor = ResourcesManager.instance.baseActorPrefab[person.isMale ? 0 : 1];
    Instantiate(actor, transform);


    myMaterials = new List<Material>();
    myColors = new List<Color>();


    SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

    foreach(SkinnedMeshRenderer renderer in renderers) {
      for(int i = renderer.materials.Length - 1; i >= 0; i--) {
        if(!myMaterials.Contains(renderer.materials[i])) {
          Material newMat = new Material(renderer.materials[i]);

          renderer.materials[i] = newMat;
          myMaterials.Add(renderer.materials[i]);
          myColors.Add(new Color(renderer.materials[i].color.r,
            renderer.materials[i].color.g,
            renderer.materials[i].color.b, 1f));
        } else {
          Debug.LogError("did nothing on mat: " + gameObject.name);
        }
      }
    }
  }


  public void SetBrightness(float value) {
    for(int i=0; i<myMaterials.Count; i++) {
      Color c = new Color(myColors[i].r * value, myColors[i].g * value, myColors[i].b * value, 1f);
      myMaterials[i].color = c;
    }
  }
}
