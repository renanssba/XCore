﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class CharacterSpriteCollection {
  public string name;
  public Sprite characterBody;
  public Sprite mechaBody;

  [Header("Poses")]
  public Sprite pose_punch;
  public Sprite pose_shout;
  public Sprite pose_interact;
}

public enum CharacterSpritePart {
  character,
  mecha,
  pose_punch,
  pose_shout,
  pose_interact
}


[System.Serializable]
public class VsnCharacterData {
  public string nameKey;
  public Sprite faceSprite;
  public AudioClip dialogSfx;
  public float pitch;
}



public class ResourcesManager : MonoBehaviour {
  [Header("- Character Sprites -")]
  public Sprite[] faceSprites;
  public Sprite[] tacticalFaceSprites;
  public List<CharacterSpriteCollection> characterSpritesCollections;

  [Header("- UI Sprites -")]
  public Sprite[] cardSprites;
  public Sprite[] itemSprites;
  public Sprite[] attributeSprites;
  public Sprite[] daytimeSprites;
  public Sprite unknownSprite;
  public Color[] attributeColor;
  public Sprite[] buttonSprites;

  [Header("- Misc. -")]
  public GameObject[] baseActorPrefab;
  public VsnCharacterData[] vsnCharacterData;


  public static ResourcesManager instance;

  public void Awake() {
    instance = this;
    characterSpritesCollections = new List<CharacterSpriteCollection>();
  }

  public Sprite GetFaceSprite(int index){
    return faceSprites[index];
  }

  public Sprite GetCharacterSprite(int id, CharacterSpritePart spritePart) {
    if(id >= characterSpritesCollections.Count) {
      return null;
    }
    CharacterSpriteCollection collection = characterSpritesCollections[id];
    switch(spritePart) {
      case CharacterSpritePart.mecha:
        return collection.mechaBody;
      case CharacterSpritePart.pose_punch:
        return collection.pose_punch;
      case CharacterSpritePart.pose_shout:
        return collection.pose_shout;
      case CharacterSpritePart.pose_interact:
        return collection.pose_interact;
      default:
      case CharacterSpritePart.character:
        return collection.characterBody;
    }
  }

  public CharacterSpriteCollection GetCharacterSpriteCollection(string charName) {
    foreach(CharacterSpriteCollection collection in characterSpritesCollections) {
      if(collection.name == charName) {
        return collection;
      }
    }
    return null;
  }

  public void GenerateCharacterSprites(string[] characterNames) {
    foreach(string charName in characterNames) {
      NewSpriteCollection(charName);
    }
  }

  public void NewSpriteCollection(string charName) {
    string characterSpritesPath = "Characters/";
    CharacterSpriteCollection spriteCollection = new CharacterSpriteCollection();

    spriteCollection.name = charName;
    spriteCollection.characterBody = Resources.Load<Sprite>(characterSpritesPath + charName + "-base");
    spriteCollection.mechaBody = Resources.Load<Sprite>(characterSpritesPath + charName + "-mecha");

    characterSpritesCollections.Add(spriteCollection);
    return;
  }

  public Color[] ImageAddition(Color[] a, Color[] b) {
    Color[] c = new Color[a.Length];
    for(int i=0; i<a.Length; i++) {
      if(b[i].a > 0f) {
      //if(Random.Range(0, 100) < 50) {
        c[i] = b[i];
      } else {
        c[i] = a[i];
      }
      //c[i] = a[i];
    }
    return c;
  }
}
