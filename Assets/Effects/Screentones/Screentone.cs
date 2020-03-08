using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Screentone : MonoBehaviour {
    private Renderer r;
    private Material mat;
    public float duration = 5.0f;
    public float rotationSpeed = 2.0f;
	// Use this for initialization
	void Start () {
        mat = GetComponent<Image>().material;
	}
	
	// Update is called once per frame
	void Update () {
	}

    [ContextMenu("Play Transition")]
    public void playTransition()
    {
        StartCoroutine(transition());
    }

    private IEnumerator transition()
    {
        float startTime = Time.time;
        float currentTime = startTime;
        
        while( currentTime - startTime< duration)
        {
            float scale = Mathf.Lerp(2, -0.5f, (currentTime - startTime)/duration);
            mat.SetFloat("_MaskScale", scale);
            yield return null;
            currentTime += Time.deltaTime;
            transform.Rotate(0, 0, rotationSpeed);
        }
    }
}
