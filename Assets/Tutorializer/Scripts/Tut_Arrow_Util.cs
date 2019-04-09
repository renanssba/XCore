using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Tut_Arrow_Util : MonoBehaviour {
	
  private Tut_Arrow arrow;

	void Update () {
    if(Application.isPlaying){
      return;
    }

    if(arrow == null){
      arrow = GetComponent<Tut_Arrow>();
    }

    Vector2 final = arrow.finalRect.anchoredPosition;
    Vector2 initial = GetComponent<RectTransform>().anchoredPosition;

    float angle = Vector2.Angle( final-initial, new Vector2(0f, 1f) );
    transform.eulerAngles = new Vector3(0f, 0f, angle);

    if(initial.x < final.x) {
      transform.eulerAngles = new Vector3(0f, 0f, angle) * -1f;
    }
	}
}
