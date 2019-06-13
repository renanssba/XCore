using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor3D : MonoBehaviour {

	void Awake() {
    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    spriteRenderer.material = new Material(spriteRenderer.material.shader);
  }
}
