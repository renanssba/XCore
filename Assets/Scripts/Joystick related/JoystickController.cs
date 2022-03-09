using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[System.Serializable]
public class ScreenContext {
  public string name;

  public GameObject startingSelectedObject;
  public GameObject lastSelectedObject;

  public bool preventNoElementSelected = true;

  public UnityEvent confirmButtonEvent;
  public UnityEvent menuButtonEvent;
  public UnityEvent squareButtonEvent;
  public UnityEvent backButtonEvent;


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
      // prevent from having no UI element selected
      if(CurrentContext().preventNoElementSelected && EventSystem.current.currentSelectedGameObject == null) {
        Utils.SelectUiElement(CurrentContext().lastSelectedObject);
      }

      // set last selected object
      if(CurrentContext().lastSelectedObject != EventSystem.current.currentSelectedGameObject &&
         EventSystem.current.currentSelectedGameObject != null) {
        SfxManager.StaticPlaySelectSfx();
        CurrentContext().lastSelectedObject = EventSystem.current.currentSelectedGameObject;
      }

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
    return; /// TODO: Implement
    Utils.SelectUiElement(CurrentContext().startingSelectedObject);
  }

  public void SelectLastSelectedObject() {
    return; /// TODO: Implement
    Utils.SelectUiElement(CurrentContext().lastSelectedObject);
  }

  public void AddContext(ScreenContext context) {
    screenContexts.Add(context);
    SelectStartingObject();
  }

  public ScreenContext GetContext(string name) {
    for(int i = 0; i < screenContexts.Count; i++) {
      if(screenContexts[i].name == name) {
        return screenContexts[i];
      }
    }
    return null;
  }

  public void RemoveContext(string name) {
    for(int i=0; i<screenContexts.Count; i++) {
      if(screenContexts[i].name == name) {
        screenContexts.RemoveAt(i);
        return;
      }
    }
  }

  public void RemoveContext() {
    screenContexts.RemoveAt(screenContexts.Count-1);
    SelectLastSelectedObject();
  }

  public ScreenContext CurrentContext() {
    if(screenContexts.Count <= 0) {
      return null;
    }
    return screenContexts[screenContexts.Count-1];
  }
}
