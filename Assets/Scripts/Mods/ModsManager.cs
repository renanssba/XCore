using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModsManager : MonoBehaviour {

  public static ModsManager instance;
  public string modsPath;

  public Texture2D loadedTex;
  public Sprite loadedSprite;

  public Sprite[] setFaces = null;
  public string[] setNames = null;


  void Awake(){
    if (instance == null) {
      instance = this;
      DontDestroyOnLoad(gameObject);
    } else if (instance != this) {
      Destroy(gameObject);
      return;
    }
    setNames = null;
    setFaces = null;
    modsPath = Application.dataPath + "/Mods/Characters/";
  }

  public void Reset() {
    setFaces = null;
    setNames = null;
  }

  public string[] GetModNames(){
    List<string> names = new List<string>();
    string[] folders = Directory.GetDirectories(modsPath);

    Debug.Log("All mods: ");
    foreach(string s in folders){
      int lastBarPosition = s.LastIndexOf('/');
      string name = s.Substring(lastBarPosition + 1, s.Length - lastBarPosition - 1);
      Debug.Log(name);
      names.Add(name);
    }

    return names.ToArray();
  }


  public IEnumerator LoadSpritesMod(string modFolderName){
    string[] boyPaths = GetCharSpritePaths(modFolderName, "Boys");
    string[] girlPaths = GetCharSpritePaths(modFolderName, "Girls");// Directory.GetFiles(modsPath + modFolderName + "\\Girls");

    for(int i=0; i<5; i++){
      GlobalData.instance.people[i].name = GetNameFromPath(boyPaths[i]);
      yield return LoadSprite(boyPaths[i]);
      //SpritesDatabase.instance.faceSprites[i] = loadedSprite;
      //      Debug.Log(name);
    }
      
    for(int i=0; i<5; i++){
      GlobalData.instance.people[5+i].name = GetNameFromPath(girlPaths[i]);
      yield return LoadSprite(girlPaths[i]);
      //SpritesDatabase.instance.faceSprites[5+i] = loadedSprite;
      //      Debug.Log(name);
    }
  }


  public IEnumerator LoadSprite(string path){
    Debug.Log("LOAD PATH: " + path);

    WWW wwwForm = new WWW("file://" + path);
    yield return wwwForm;

//    wwwForm.LoadImageIntoTexture(loadedTex);

    loadedTex = wwwForm.texture as Texture2D;

    loadedSprite = Sprite.Create(loadedTex, new Rect(0, 0, loadedTex.width, loadedTex.height), Vector2.zero);
  }


  public string[] GetCharSpritePaths(string modFolderName, string subFolder){
    string[] namesArray = Directory.GetFiles(modsPath + modFolderName + "\\" + subFolder);
    List<string> namesList = new List<string>();

    foreach(string name in namesArray){
      if(name.Length >= 5 && name.Substring(name.Length-5, 5) == ".meta" ){
        continue;
      }
      namesList.Add(name.Replace('/', '\\'));
    }

    return namesList.ToArray();
  }

  string GetNameFromPath(string path){
    int lastBarPosition = path.LastIndexOf('\\');
    string pathlessName = path.Substring(lastBarPosition + 1, path.Length - lastBarPosition - 1);

    lastBarPosition = pathlessName.LastIndexOf('.');
    int extensionSize = pathlessName.Length - lastBarPosition;

    return pathlessName.Substring(0, pathlessName.Length-extensionSize);
  }



  void DebugText(string toDebug){
    string debugFilePath = "DebugFile.txt";
//    Debug.Log("Debug path: " + debugFilePath);

    if(!File.Exists(debugFilePath)){
      File.Create(debugFilePath).Close();
    }
    File.AppendAllText(debugFilePath, toDebug);
  }

  void DebugText(string[] toDebug){
    foreach(string s in toDebug){
      DebugText(s + "\n");
    }
  }

  public Sprite GetFaceSprite(int id) {
    if(setNames == null) {
      return null;
    }
    if(setFaces.Length <= id) {
      return null;
    }
    return setFaces[id];
  }

  public string GetName(int id) {
    if(setNames == null) {
      return null;
    }
    return setNames[id];
  }
}
