using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour {
  public Camera camera;
  public Vector3 defaultCameraPos;
  public float cameraMoveTime = 0.3f;


  public IEnumerator GoToDefaultPosition() {
    Camera.main.transform.DOMove(defaultCameraPos, cameraMoveTime);
    Camera.main.DOOrthoSize(5f, cameraMoveTime);
    yield return new WaitForSeconds(cameraMoveTime);
  }

  public IEnumerator FocusOnCombat(Combat combat) {
    Vector3 combatCenter = combat.CenterPosition();
    combatCenter.z = -10;
    Camera.main.transform.DOMove(combatCenter, cameraMoveTime);
    Camera.main.DOOrthoSize(3.5f, cameraMoveTime);
    yield return new WaitForSeconds(cameraMoveTime);
  }
}
