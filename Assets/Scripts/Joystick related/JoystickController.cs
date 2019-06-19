using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ScreenContext {
  public string name;

  public GameObject startingSelectedObject;

  public UnityEvent confirmButtonEvent;
  public UnityEvent menuButtonEvent;
  public UnityEvent squareButtonEvent;
  public UnityEvent backButtonEvent;

  public GameObject lastSelectedObject;

  public ScreenContext() {
    lastSelectedObject = startingSelectedObject;
  }
}


public class JoystickController : MonoBehaviour {

  public static JoystickController instance;

  public List<ScreenContext> screenContexts;


  public void Awake() {
    instance = this;
  }

  public void Start() {
    SelectStartingObject();
  }

  public void Update () {
    if(CurrentContext() != null) {
      if(Input.GetButtonDown("Submit") && CurrentContext().confirmButtonEvent != null) {
        Debug.LogWarning("Clicked Select button");
        CurrentContext().confirmButtonEvent.Invoke();
      }
      if(Input.GetButtonDown("Menu") && CurrentContext().menuButtonEvent != null) {
        Debug.LogWarning("Clicked Menu button");
        CurrentContext().menuButtonEvent.Invoke();
      }
      if(Input.GetButtonDown("Square") && CurrentContext().squareButtonEvent != null) {
        Debug.LogWarning("Clicked Square button");
        CurrentContext().squareButtonEvent.Invoke();
      }
      if(Input.GetButtonDown("Cancel") && CurrentContext().backButtonEvent != null) {
        Debug.LogWarning("Clicked Cancel button");
        CurrentContext().backButtonEvent.Invoke();
      }
    }
  }

  public void SelectStartingObject() {
    Utils.SelectUiElement(CurrentContext().startingSelectedObject);
  }

  public void SelectLastSelectedObject() {
    Utils.SelectUiElement(CurrentContext().lastSelectedObject);
  }

  public void AddContext(ScreenContext context) {
    screenContexts.Add(context);
    SelectStartingObject();
  }

  public void RemoveContext() {
    screenContexts.RemoveAt(screenContexts.Count-1);
    SelectLastSelectedObject();
  }

  public ScreenContext CurrentContext() {
    return screenContexts[screenContexts.Count-1];
  }
}
