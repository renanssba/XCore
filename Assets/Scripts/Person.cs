using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Personality{
  heroico,
  racional,
  emotivo
}

public enum Trait{
  mirror,
  adaptative,
  simpleton,
  unpredictable
}

public enum Attributes{
  guts = 0,
  intelligence = 1,
  charisma = 2
}


[System.Serializable]
public class Person {

  public string name;
  public bool isMale;

  public int[] attributes;
  public Personality personality;
  public List<Trait> traits;

  public int faceId;


  public void Initialize() {
    attributes = new int[3];
    attributes[(int)Attributes.guts] = Random.Range(1, 4);
    attributes[(int)Attributes.intelligence] = Random.Range(1, 4);
    attributes[(int)Attributes.charisma] = Random.Range(1, 4);
    if (isMale) {
      faceId = Random.Range(0, 5);
    } else {
      faceId = 5 + Random.Range(0, 5);
    }
    personality = (Personality)Random.Range(0, 3);
  }
}
