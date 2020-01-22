using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class CharacterSpriteCollection {
  public Sprite body;
  public Sprite schoolClothes;
  public Sprite casual;
  
  public RenderTexture renderCasual;
  public RenderTexture renderUniform;
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

  public void GenerateCharacterSprites(string[] characterNames) {
    foreach(string charName in characterNames) {
      NewSpriteCollection(charName);
    }
  }

  public void NewSpriteCollection(string charName) {
    string asd = "Characters/";
    CharacterSpriteCollection spriteCollection = new CharacterSpriteCollection();
    spriteCollection.body = Resources.Load<Sprite>(asd + charName + "-base");
    //spriteCollection.schoolClothes = Resources.Load<Sprite>(asd + charName + "-uniform");
    //spriteCollection.casual = Resources.Load<Sprite>(asd + charName + "-casual");

    Texture2D body = Resources.Load<Texture2D>(asd + charName + "-base"); ;
    Texture2D casual = Resources.Load<Texture2D>(asd + charName + "-casual");
    Texture2D uniform = Resources.Load<Texture2D>(asd + charName + "-uniform");

    spriteCollection.renderCasual = new RenderTexture((int)spriteCollection.body.rect.width, (int)spriteCollection.body.rect.height, 24);
    spriteCollection.renderUniform = new RenderTexture((int)spriteCollection.body.rect.width, (int)spriteCollection.body.rect.height, 24);

    RenderTexture.active = spriteCollection.renderCasual;
    GL.Clear(true, true, Color.black);
    Graphics.Blit(body, spriteCollection.renderCasual);
    Graphics.Blit(casual, spriteCollection.renderCasual);

    Texture2D myTexture2D = new Texture2D(RenderTexture.active.width, RenderTexture.active.height);
    myTexture2D.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
    myTexture2D.Apply();

    Sprite newSprite = Sprite.Create(myTexture2D, new Rect(0, 0, myTexture2D.width, myTexture2D.height), new Vector2(0.5f, 0.5f));
    spriteCollection.schoolClothes = newSprite;


    RenderTexture.active = spriteCollection.renderCasual;
    GL.Clear(true, true, Color.black);
    Graphics.Blit(body, spriteCollection.renderUniform);
    Graphics.Blit(uniform, spriteCollection.renderUniform);

    //spriteCollection.texCasual = Resources.Load<Texture2D>(asd + charName + "-casual");
    //spriteCollection.renderCasual = newtex;
    //spriteCollection.casual = Resources.Load<Sprite>(asd + charName + "-casual");

    characterSpritesCollections.Add(spriteCollection);
  }
}
