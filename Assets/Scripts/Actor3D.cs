using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor3D : MonoBehaviour {

  public List<Material> myMaterials;

	public void Awake() {
    myMaterials = new List<Material>();
    //List<Material> clonedMaterials = new List<Material>();

    // TODO: FIX THIS to not create many copies for each material

    //Dictionary<Material, Material> dic = new Dictionary<Material, Material>();

    SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

    foreach(SkinnedMeshRenderer renderer in renderers) {
      //for(int i = 0; i < renderer.materials.Length; i++) {
      for(int i = renderer.materials.Length - 1; i >= 0; i--) {
        if( !myMaterials.Contains(renderer.materials[i]) ) {
          Material newMat = new Material(renderer.materials[i]);
          //dic.Add(renderer.materials[i], newMat);

          renderer.materials[i] = newMat;
          myMaterials.Add(renderer.materials[i]);
        } else {
          Debug.LogError("did nothing on mat: " + gameObject.name);
        }
      }
    }
  }

  public void SetColor(Color c) {
    foreach(Material m in myMaterials) {
      m.color = c;
    }
  }
}
