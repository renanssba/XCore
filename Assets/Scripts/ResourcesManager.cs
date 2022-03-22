using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class CharacterSpriteCollection {
  public string name;
  public Sprite fullBody;

  [Header("- Battle Sprite -")]
  public Sprite mechaBody;

  [Header("- Tactical Icons -")]
  public Sprite faceIcon;
  public Sprite portraitIcon;
  public Sprite mapIcon;
}

public enum CharacterSpritePart {
  fullBody,
  mecha,
  face,
  portrait,
  mapIcon
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
  public CharacterSpriteCollection[] characterSpritesCollections;

  [Header("- UI Sprites -")]
  public Sprite[] daytimeSprites;
  public Color[] attributeColor;
  public Sprite[] buttonSprites;

  [Header("- Misc. -")]
  public GameObject[] baseActorPrefab;
  public VsnCharacterData[] vsnCharacterData;


  public static ResourcesManager instance;


  public void Awake() {
    if(instance == null) {
      instance = this;
    } else if(instance != this) {
      Destroy(gameObject);
      return;
    }
    DontDestroyOnLoad(gameObject);
    GenerateCharacterSprites();
  }

  public Sprite GetCharacterSprite(int id, CharacterSpritePart spritePart) {
    if(id >= characterSpritesCollections.Length) {
      return null;
    }
    CharacterSpriteCollection collection = characterSpritesCollections[id];

    switch(spritePart) {
      case CharacterSpritePart.mecha:
        return collection.mechaBody;
      case CharacterSpritePart.fullBody:
        return collection.fullBody;
      case CharacterSpritePart.face:
        return collection.faceIcon;
      case CharacterSpritePart.portrait:
        return collection.portraitIcon;
      case CharacterSpritePart.mapIcon:
        return collection.mapIcon;
      default:
        return null;
    }
  }

  public void GenerateCharacterSprites() {
    characterSpritesCollections = new CharacterSpriteCollection[34];
    characterSpritesCollections[0] = NewSpriteCollection("marcus");
    characterSpritesCollections[1] = NewSpriteCollection("agnes");
    characterSpritesCollections[2] = NewSpriteCollection("maya");

    characterSpritesCollections[30] = NewSpriteCollection("fly");
    characterSpritesCollections[31] = NewSpriteCollection("brute");
    characterSpritesCollections[32] = NewSpriteCollection("boss");
    characterSpritesCollections[33] = NewSpriteCollection("city");
  }

  public CharacterSpriteCollection NewSpriteCollection(string charName) {
    string characterSpritesPath = "Characters/";
    CharacterSpriteCollection spriteCollection = new CharacterSpriteCollection();

    spriteCollection.name = charName;
    spriteCollection.fullBody = Resources.Load<Sprite>(characterSpritesPath + charName + "-base");
    spriteCollection.mechaBody = Resources.Load<Sprite>(characterSpritesPath + charName + "-mecha");
    spriteCollection.faceIcon = Resources.Load<Sprite>(characterSpritesPath + charName + "-face");
    spriteCollection.portraitIcon = Resources.Load<Sprite>(characterSpritesPath + charName + "-portrait");
    spriteCollection.mapIcon = Resources.Load<Sprite>(characterSpritesPath + charName + "-mapicon");
    return spriteCollection;
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
