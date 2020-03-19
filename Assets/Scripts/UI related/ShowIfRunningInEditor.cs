using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIfRunningInEditor : MonoBehaviour {

	void Awake() {
		if(!Application.isEditor){
			gameObject.SetActive(false);
		}
	}
}
