﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class CharacterSpriteCollection {
  public string name;
  public Sprite baseBody;
  public Sprite sadBody;
  public Sprite underwear;
  public Sprite schoolClothes;
  public Sprite casualClothes;
  public Sprite incompleteCasualClothes;
  public Sprite bruises;
}

public enum CharacterSpritePart {
  body,
  sad,
  underwear,
  school,
  unclothed,
  casual,
  bruises
}


public class ResourcesManager : MonoBehaviour {
  public Sprite[] faceSprites;
  public Sprite[] fixedCharactersFaceSprites;
  public Sprite[] cardSprites;
  public Sprite[] itemSprites;
  public Sprite[] attributeSprites;
  public Sprite[] daytimeSprites;
  public GameObject[] baseActorPrefab;
  public Sprite unknownSprite;
  public Color[] attributeColor;
  public Sprite[] buttonSprites;
  public List<CharacterSpriteCollection> characterSpritesCollections;


  public static ResourcesManager instance;

  public void Awake() {
    instance = this;
    characterSpritesCollections = new List<CharacterSpriteCollection>();
  }

  public Sprite GetFaceSprite(int index){
    if(ModsManager.instance.GetFaceSprite(index) != null) {
      Debug.LogWarning("USING MODDED FACES");
      return ModsManager.instance.GetFaceSprite(index);
    }
    return faceSprites[index];
  }

  public Sprite GetCharacterSprite(int id, CharacterSpritePart spritePart) {
    if(id >= characterSpritesCollections.Count) {
      return null;
    }
    CharacterSpriteCollection col = characterSpritesCollections[id];
    switch(spritePart) {
      case CharacterSpritePart.body:
        return col.baseBody;
      case CharacterSpritePart.sad:
        return col.sadBody;
      case CharacterSpritePart.underwear:
        return col.underwear;
      case CharacterSpritePart.school:
        return col.schoolClothes;
      case CharacterSpritePart.unclothed:
        return col.incompleteCasualClothes;
      case CharacterSpritePart.bruises:
        return col.bruises;
      case CharacterSpritePart.casual:
      default:
        return col.casualClothes;
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
    Color[] bodyPixels, casualPixels, uniformPixels, auxColorArray;
    Sprite newSprite;
    CharacterSpriteCollection spriteCollection = new CharacterSpriteCollection();

    spriteCollection.name = charName;
    spriteCollection.baseBody = Resources.Load<Sprite>(characterSpritesPath + charName + "-base");
    spriteCollection.sadBody = Resources.Load<Sprite>(characterSpritesPath + charName + "-sad");

    spriteCollection.underwear = Resources.Load<Sprite>(characterSpritesPath + charName + "-underwear");

    spriteCollection.schoolClothes = Resources.Load<Sprite>(characterSpritesPath + charName + "-uniform");
    spriteCollection.casualClothes = Resources.Load<Sprite>(characterSpritesPath + charName + "-casual");

    spriteCollection.bruises = Resources.Load<Sprite>(characterSpritesPath + charName + "-hurt");
    spriteCollection.incompleteCasualClothes = Resources.Load<Sprite>(characterSpritesPath + charName + "-unclothed");

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
