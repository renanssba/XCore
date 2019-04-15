using UnityEngine;
using System.Collections;

public class DecorationSpawner : MonoBehaviour {

  public GameObject decorationPrefab;
  float screenWidth;
  float screenHeight;

	void Start(){
    screenWidth = GetComponent<RectTransform>().sizeDelta.x;
    screenHeight = GetComponent<RectTransform>().sizeDelta.y;

    SpawnDecorations();
	}

  void SpawnDecorations(){
    int numberOfColumns = 7;
    int numberOfRows = 6;

    for(int i=0; i<numberOfColumns; i++){
      for(int j=0; j<numberOfRows; j++){
        float x = ((-numberOfColumns+1 - ((j%2==0)?1:0)) + (int)(2*i)) * ((screenWidth/numberOfColumns/2));
        float y = ((-numberOfRows+1) + (int)(2*j)) * ((screenHeight/numberOfRows/2));
        SpawnDecoration(x, y);
      }
    }
  }

  void SpawnDecoration(float x, float y){
    GameObject newObject = (GameObject)Instantiate(decorationPrefab, transform);
    Transform t = newObject.transform;
    t.localPosition = new Vector3(x, y, 0f);
    t.localScale = Vector3.one;
    newObject.GetComponent<Decoration>().screenTransform = GetComponent<RectTransform>();
  }
}
