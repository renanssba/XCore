using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Actor2D : MonoBehaviour {

  public new SpriteRenderer renderer;

  const float attackAnimTime = 0.15f;


  public void SetCharacterGraphics(Person p) {
    /// TODO: implement
  }

  public void SetChallengeGraphics(DateEvent currentEvent) {
    if(!string.IsNullOrEmpty(currentEvent.spriteName)) {
      renderer.sprite = LoadSprite("Challenges/" + currentEvent.spriteName);
    } else {
      gameObject.SetActive(false);
    }
  }

  public Sprite LoadSprite(string sprite) {
    Sprite backgroundSprite = Resources.Load<Sprite>(sprite);
    if(backgroundSprite == null) {
      Debug.LogError("Error loading " + sprite + " sprite. Please check its path");
    }
    return backgroundSprite;
  }


  public void CharacterAttackAnim() {
    transform.DOMoveX(0.3f, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public void EnemyAttackAnim() {
    transform.DOMoveX(-0.3f, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public void Shine() {
    FlashRenderer(transform, 0.1f, 0.8f, 0.2f);
  }

  public void FlashRenderer(Transform obj, float minFlash, float maxFlash, float flashTime) {
    //SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
    DOTween.Kill(renderer.material);
    renderer.material.SetFloat("_FlashAmount", minFlash);
    renderer.material.DOFloat(maxFlash, "_FlashAmount", flashTime).SetLoops(2, LoopType.Yoyo);
  }
}
