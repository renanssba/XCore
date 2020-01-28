using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class CharacterSpriteCollection {
  public string name;
  public Sprite body;
  public Sprite schoolClothes;
  public Sprite casual;
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

  public Sprite GetCharacterSprite(int id, string situation) {
    if(id >= characterSpritesCollections.Count) {
      return null;
    }
    CharacterSpriteCollection col = characterSpritesCollections[id];
    switch(situation) {
      case "nude":
        return col.body;
      case "uniform":
        return col.schoolClothes;
      case "casual":
      default:
        return col.casual;
    }
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
    spriteCollection.body = Resources.Load<Sprite>(characterSpritesPath + charName + "-base");

    Texture2D bodyTex = Resources.Load<Texture2D>(characterSpritesPath + charName + "-base");
    bodyPixels = bodyTex.GetPixels();
    Texture2D casualTex = Resources.Load<Texture2D>(characterSpritesPath + charName + "-casual");

    // If there's no casual texture, use the base for everything
    if(casualTex == null) {
      spriteCollection.schoolClothes = spriteCollection.body;
      spriteCollection.casual = spriteCollection.body;
      return;
    }
    casualPixels = casualTex.GetPixels();
    Texture2D uniformTex = Resources.Load<Texture2D>(characterSpritesPath + charName + "-uniform");
    uniformPixels = uniformTex.GetPixels();


    Debug.LogWarning("body texture format: " + bodyTex.format);
    Debug.LogWarning("casual texture format: " + casualTex.format);
    Debug.LogWarning("uniform texture format: " + uniformTex.format);

    auxColorArray = ImageAddition(bodyPixels, uniformPixels);
    Texture2D auxTexture = new Texture2D(casualTex.width, casualTex.height);
    auxTexture.SetPixels(auxColorArray);
    auxTexture.Apply();
    newSprite = Sprite.Create(auxTexture, new Rect(0, 0, auxTexture.width, auxTexture.height), new Vector2(0.5f, 0f));
    spriteCollection.schoolClothes = newSprite;


    auxColorArray = ImageAddition(bodyPixels, casualPixels);
    auxTexture = new Texture2D(casualTex.width, casualTex.height);
    auxTexture.SetPixels(auxColorArray);
    auxTexture.Apply();
    newSprite = Sprite.Create(auxTexture, new Rect(0, 0, auxTexture.width, auxTexture.height), new Vector2(0.5f, 0f));
    spriteCollection.casual = newSprite;
    

    characterSpritesCollections.Add(spriteCollection);
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
