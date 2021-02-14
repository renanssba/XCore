using UnityEngine;
using System.Runtime.InteropServices;

public class Link : MonoBehaviour {

  public string url = "";


  public void OpenLinkJSPlugin() {
    if(string.IsNullOrEmpty(url)) {
      return;
    }

    if(!url.Contains("http")) {
      VsnUIManager.instance.ClickedButton(url);
      return;
    }

    Debug.LogWarning("Calling OpenLinkJSPlugin function. URL: " + url);
#if !UNITY_EDITOR
		openWindow(url);
#endif
  }

  [DllImport("__Internal")]
  private static extern void openWindow(string url);
}