using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TextRenderDebug : MonoBehaviour {

	void Start () {
    transform.DOMoveX(3f, 0.2f).SetRelative(true).SetLoops(2, LoopType.Yoyo);
	}
}
