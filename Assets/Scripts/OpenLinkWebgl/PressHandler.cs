using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class PressHandler : MonoBehaviour, IPointerDownHandler {
  [Serializable]
  public class ButtonPressEvent : UnityEvent { }

  public ButtonPressEvent OnPress = new ButtonPressEvent();

  public void OnPointerDown(PointerEventData eventData) {

    if(GetComponent<OpenHyperlinks>() != null && !GetComponent<OpenHyperlinks>().IsInLinkRegion()) {
      return;
    }

    OnPress.Invoke();
  }
}