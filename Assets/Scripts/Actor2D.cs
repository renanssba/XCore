﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using DG.Tweening;
using TMPro;

public class Actor2D : MonoBehaviour {

  public SpriteRenderer[] renderers;
  public SpriteRenderer[] buffAuraRenderers;
  public new ParticleSystem particleSystem;
  public SpriteRenderer shadowRenderer;
  public Color[] rendererColors;
  public float[] rendererFlashAmounts;

  public SpriteRenderer weaknessCardRenderer;
  public TextMeshPro weaknessCardText;

  public Battler battler;
  public Enemy enemy;
  //public Person person;
  public Animator animator;

  public Button targetSelectButton;

  public GameObject choosingActionIndicator;
  public GameObject spottedIcon;


  const float attackAnimTime = 0.18f;

  public MaterialPropertyBlock materialProperties;



  [ContextMenu("Print Material Property Block")]
  public void PrintMaterialPropertyBlock() {
    Debug.Log("Flash Color:" + materialProperties.GetColor("_FlashColor"));
    Debug.Log("Flash Amount:" + materialProperties.GetColor("_FlashAmount"));
  }


  void Awake() {
    materialProperties = new MaterialPropertyBlock();

    rendererColors = new Color[renderers.Length];
    rendererFlashAmounts = new float[renderers.Length];
  }


  public void SetCharacter(Person p) {
    battler = p;
    UpdateGraphics();
  }

  public void UpdateGraphics() {
    if(battler.GetType() == typeof(Person)) {
      UpdateCharacterGraphics();
      if(TheaterController.instance.bgRenderer.sprite.name.Contains("school")) {
        SetClothing("uniform");
      } else {
        SetClothing("casual");
      }
    } else if(battler.GetType() == typeof(Enemy)) {
      // do nothing
    }
  }

  public void UpdateCharacterGraphics() {
    if(battler.id == (int)PersonId.fertiliel) {
      renderers[4].sprite = Resources.Load<Sprite>("Characters/hiding-spot-" + BattleController.instance.currentDateLocation.ToString());
      spottedIcon.SetActive(battler.IsSpotted());
      return;
    }

    if(!string.IsNullOrEmpty(battler.GetName())) {
      if(battler.CurrentStatusConditionStacks("sad") == 0) {
        renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.body);
      } else {
        renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.sad);
      }
      //renderers[1].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.underwear);
      renderers[1].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.casual);
      renderers[1].GetComponent<SpriteMask>().sprite = renderers[1].sprite;
      renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.bruises);
      if(battler.CurrentStatusConditionStacks("injured") > 0) {
        renderers[2].gameObject.SetActive(true);
      } else {
        renderers[2].gameObject.SetActive(false);
      }
      switch(battler.CurrentStatusConditionStacks("dirty")) {
        case 0:
          renderers[3].gameObject.SetActive(false);
          renderers[4].gameObject.SetActive(false);
          break;
        case 1:
          renderers[3].gameObject.SetActive(true);
          renderers[4].gameObject.SetActive(false);
          break;
        case 2:
          renderers[3].gameObject.SetActive(true);
          renderers[4].gameObject.SetActive(true);
          break;
      }
      SetAuraVisibility();
    } else {
      gameObject.SetActive(false);
    }
  }

  public void SetClothing(string clothingType) {
    if(battler.GetType() != typeof(Person)) {
      return;
    }
    switch(clothingType) {
      case "uniform":
        renderers[1].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.school);
        break;
      case "casual":
      default:
        renderers[1].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.casual);
        break;
    }
  }

  public void FaceLeft() {
    Vector3 scale = transform.localScale;
    scale.x = -Mathf.Abs(scale.x);
    transform.localScale = scale;
  }

  public void FaceRight() {
    Vector3 scale = transform.localScale;
    scale.x = Mathf.Abs(scale.x);
    transform.localScale = scale;
  }

  public void MoveToPosition(Vector3 destination, float duration) {
    if(duration == 0f) {
      transform.position = destination;
    } else {
      transform.DOLocalMove(destination, duration);
    }
  }

  public void SetAuraVisibility() {
    Debug.LogWarning("SETTING AURA VISIBILITY");
    if(battler.CurrentStatusConditionStacks("encouraged") > 0) {
      ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.guts]);
    } else if(battler.CurrentStatusConditionStacks("focused") > 0) {
      ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.intelligence]);
    } else if(battler.CurrentStatusConditionStacks("inspired") > 0) {
      ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.charisma]);
    } else if(battler.CurrentStatusConditionStacks("blessed") > 0) {
      ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.endurance]);
    } else {
      Debug.LogWarning("SETTING AURA VISIBILITY TO FALSE");
      buffAuraRenderers[0].gameObject.SetActive(false);
    }
  }

  public void ShowAura(Color c) {
    c = 2f * c;
    Debug.LogWarning("showing aura: " + c);

    ParticleSystem.MainModule settings = particleSystem.main;
    settings.startColor = new ParticleSystem.MinMaxGradient(c);

    buffAuraRenderers[0].color = c;
    buffAuraRenderers[1].color = c;
    buffAuraRenderers[0].gameObject.SetActive(true);
  }

  public void SetEnemy(Enemy currentEvent) {
    enemy = currentEvent;
    battler = currentEvent;
    SetActorGraphics(enemy.spriteName);
  }

  public void SetActorGraphics(string spriteName) {
    //if(string.IsNullOrEmpty(spriteName)){
    //  gameObject.SetActive(false);
    //  return;
    //}
    if(gameObject.name.Contains(spriteName)) {
      return;
    }

    if(spriteName.StartsWith("person(")) {
      string actorName = Utils.GetStringArgument(spriteName);

      Debug.LogWarning("Setting enemy sprite to: " + actorName);

      CharacterSpriteCollection spriteCollection = ResourcesManager.instance.GetCharacterSpriteCollection(actorName);

      renderers[0].sprite = spriteCollection.baseBody;
      renderers[0].flipX = true;
      renderers[1].sprite = spriteCollection.casualClothes;
      renderers[1].gameObject.SetActive(true);
      renderers[1].flipX = true;
      shadowRenderer.transform.localScale = Vector3.one;
    } else {
      renderers[0].sprite = LoadSprite("Enemies/" + spriteName);
      renderers[0].flipX = false;
      renderers[1].gameObject.SetActive(false);
      shadowRenderer.transform.localScale = new Vector3(1.4f, 1f, 1f);
    }
  }

  public Sprite LoadSprite(string spriteName) {
    Sprite sprite = Resources.Load<Sprite>(spriteName);
    if(sprite == null) {
      Debug.LogError("Error loading " + spriteName + " sprite. Please check its path");
    }
    return sprite;
  }

  public void SetBattleMode(bool value) {
    animator.SetBool("Battle", value);
  }

  public void SetChooseActionMode(bool value) {
    animator.SetBool("Choosing Action", value && !battler.IsSpotted());
    choosingActionIndicator.SetActive(value);
  }

  public void SetAttackMode(bool value) {
    animator.SetBool("Attacking", value);
  }

  public void SetParameter(string param, bool value) {
    animator.SetBool(param, value);
  }


  public void CharacterAttackAnim() {
    float movementX = 0.3f;
    if(transform.position.x > 0f) {
      movementX = -0.3f;
    }
    renderers[0].transform.DOMoveX(movementX, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public void UseItemAnimation(Actor2D destiny, Item item) {
    ShowThrowItemAnimation(item.sprite, destiny);
  }

  public void DefendActionAnimation() {
    GameObject particlePrefab = BattleController.instance.defenseActionParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;
    VsnAudioManager.instance.PlaySfx("buff_default");
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);
  }


  public void DetectAnimation() {
    GameObject particlePrefab = BattleController.instance.detectParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;
    //VsnAudioManager.instance.PlaySfx("buff_default");
    GameObject newParticle = Instantiate(particlePrefab, transform);
  }

  public void ShineRed() {
    //for(int i=0; i<renderers.Length; i++) {
    //  renderers[i].GetPropertyBlock(materialProperties);
    //  materialProperties.SetColor("_FlashColor", Color.red);
    //  rendererColors[i] = renderers[i].color;
    //  renderers[i].SetPropertyBlock(materialProperties);
    //}
    FlashRenderer(transform, 0.1f, 0.8f, 0.2f, Color.red);
  }

  public void ShineGreen() {
    //for(int i = 0; i < renderers.Length; i++) {
    //  renderers[i].GetPropertyBlock(materialProperties);
    //  materialProperties.SetColor("_FlashColor", Color.green);
    //  rendererColors[i] = renderers[i].color;
    //  renderers[i].SetPropertyBlock(materialProperties);
    //}
    FlashRenderer(transform, 0.1f, 0.8f, 0.2f, Color.green);
  }

  public void ShowDefendHitParticle() {
    GameObject particlePrefab = BattleController.instance.defendHitParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;
    //TODO: add "reduce damage" SFX
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);
  }

  public void FlashRenderer(Transform obj, float minFlash, float maxFlash, float flashTime, Color finalColor) {
    //MaterialPropertyBlock materialProperties = new MaterialPropertyBlock();

    foreach(SpriteRenderer currentRenderer in renderers) {
    //for(int i = 0; i < renderers.Length; i++) {
      //DOTween.Kill(renderers[i].material);
      float initialFlashPower;
      Color initialColor;

      currentRenderer.GetPropertyBlock(materialProperties);

      initialColor = currentRenderer.material.GetColor("_FlashColor");
      initialFlashPower = currentRenderer.material.GetFloat("_FlashAmount");

      minFlash = Mathf.Max(initialFlashPower, minFlash);
      maxFlash = Mathf.Max(initialFlashPower, maxFlash);

      materialProperties.SetColor("_FlashColor", finalColor);
      materialProperties.SetFloat("_FlashAmount", minFlash);
      currentRenderer.SetPropertyBlock(materialProperties);

      float currentFlashPower = Mathf.Max(initialFlashPower, minFlash);
      Color currentColor = finalColor;

      //DOTween.To(() => currentColor, y => currentColor = y, finalColor, flashTime).SetLoops(2, LoopType.Yoyo);

      DOTween.To(() => currentFlashPower, x => currentFlashPower = x, maxFlash, flashTime).SetLoops(2, LoopType.Yoyo).OnUpdate(() => {
        currentRenderer.GetPropertyBlock(materialProperties);
        materialProperties.SetFloat("_FlashAmount", currentFlashPower);
        materialProperties.SetColor("_FlashColor", currentColor);
        currentRenderer.SetPropertyBlock(materialProperties);
      }).OnComplete(() => {
        currentRenderer.GetPropertyBlock(materialProperties);
        materialProperties.SetFloat("_FlashAmount", initialFlashPower);
        materialProperties.SetColor("_FlashColor", initialColor);
        currentRenderer.SetPropertyBlock(materialProperties);
      });

      //currentRenderermaterial.DOFloat(maxFlash, "_FlashAmount", flashTime).SetLoops(2, LoopType.Yoyo).OnComplete(() => {
      // currentRenderer.material.SetFloat("_FlashAmount", 0f);
      //});
    }
  }


  public void ShowWeaknessCard(bool activate) {
    if(enemy == null) {
      return;
    }
    weaknessCardRenderer.gameObject.SetActive(activate);
    if(activate) {
      VsnAudioManager.instance.PlaySfx("relationship_up");
      SetWeaknessCardText();
    }
  }

  public void SetWeaknessCardText() {
    if(enemy == null) {
      return;
    }

    string text = "";
    Attributes[] weak = enemy.GetWeaknesses();
    Attributes[] resistant = enemy.GetResistances();

    if(weak.Length > 0) {
      text += Lean.Localization.LeanLocalization.GetTranslationText("date/weakness") + "\n";
      for(int i = 0; i < weak.Length; i++) {
        text += Lean.Localization.LeanLocalization.GetTranslationText("attribute/" + weak[i].ToString()) + "\n";
      }
    }
    if(resistant.Length > 0) {
      if(!string.IsNullOrEmpty(text)) {
        text += "\n";
      }
      text += Lean.Localization.LeanLocalization.GetTranslationText("date/resistance") + "\n";
      for(int i = 0; i < resistant.Length; i++) {
        text += Lean.Localization.LeanLocalization.GetTranslationText("attribute/" + resistant[i].ToString()) + "\n";
      }
    }

    weaknessCardText.text = text;
  }


  public void ShowDamageParticle(int damage, float effectivity) {
    string particleString = damage.ToString();
    Color particleColor = ResourcesManager.instance.attributeColor[0];

    Debug.LogWarning("damage: "+damage+", effectivity: "+effectivity);

    if(effectivity > 1f) {
      particleString += "\n<size=12>" + Lean.Localization.LeanLocalization.GetTranslationText("date/super_effective") + "</size>";
    } else if(effectivity < 1f) {
      particleString += "\n<size=12>" + Lean.Localization.LeanLocalization.GetTranslationText("date/ineffective") + "</size>";
      particleColor = new Color(0.3f, 0.3f, 0.3f);
    }
    ShowParticleAnimation(particleString, particleColor);
  }

  public void ShowEmpowerParticle(Attributes attribute, int value) {
    string particleString = "+" + value + " " + Lean.Localization.LeanLocalization.GetTranslationText("attribute/" + attribute.ToString());
    Color particleColor = ResourcesManager.instance.attributeColor[(int)attribute];

    ShowParticleAnimation(particleString, particleColor);
  }

  public void ShowHealHpParticle(int value) {
    string particleString = "+" + value +" "+Lean.Localization.LeanLocalization.GetTranslationText("attribute/hp");
    ShowParticleAnimation(particleString, Color.green);
  }

  public void ShowHealSpParticle(int value) {
    string particleString = "<size=80%>+" + value + " " + Lean.Localization.LeanLocalization.GetTranslationText("attribute/sp")+ "</size>";
    ShowParticleAnimation(particleString, Color.cyan);
  }

  public void ShowStatusConditionParticle(StatusCondition statusCondition) {
    ShowParticleAnimationWithSprite("<size=50%>" + statusCondition.GetPrintableName() + "</size>", Color.gray, statusCondition.sprite);
  }

  public void ShowResistConditionParticle() {
    ShowParticleAnimation("<size=12>"+Lean.Localization.LeanLocalization.GetTranslationText("status_condition/resisted") +"</size>", Color.gray);
  }

  public void ShowImmuneConditionParticle() {
    ShowParticleAnimation("<size=12>" + Lean.Localization.LeanLocalization.GetTranslationText("status_condition/immune") + "</size>", Color.gray);
  }

  public void ShowParticleAnimationWithSprite(string text, Color color, Sprite particleSprite) {
    GameObject particlePrefab = BattleController.instance.damageParticlePrefab;
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);

    newParticle.GetComponent<TextMeshPro>().color = color;
    newParticle.GetComponent<TextMeshPro>().text = text;
    newParticle.GetComponentInChildren<SpriteRenderer>().sprite = particleSprite;
  }

  public void ShowParticleAnimation(string text, Color color) {
    GameObject particlePrefab = BattleController.instance.damageParticlePrefab;
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);

    newParticle.GetComponent<TextMeshPro>().color = color;
    newParticle.GetComponent<TextMeshPro>().text = text;
    newParticle.GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
  }

  public void ShowThrowItemAnimation(Sprite itemSprite, Actor2D targetPerson) {
    GameObject particlePrefab = BattleController.instance.itemParticlePrefab;
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);

    newParticle.GetComponent<JumpingParticle>().finalPosition = new Vector3(targetPerson.transform.position.x, particlePrefab.transform.position.y, particlePrefab.transform.position.z);
    newParticle.GetComponent<SpriteRenderer>().sprite = itemSprite;
  }

  public void ClickedTargetSelectButton() {
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");

    Debug.LogWarning("clicked person: " + battler.GetName());

    for(int i = 0; i < BattleController.instance.partyMembers.Length; i++) {
      if(BattleController.instance.partyMembers[i] == battler) {
        BattleController.instance.selectedTargetPartyId[currentPlayerTurn] = i;
      }
    }
    UIController.instance.CleanHelpMessagePanel();
    UIController.instance.selectTargetPanel.SetActive(false);
    VsnController.instance.GotCustomInput();
  }


  public void SetFocusedSortingLayer(bool value) {
    if(value) {
      GetComponent<SortingGroup>().sortingOrder = 100;
    } else if(!value) {
      GetComponent<SortingGroup>().sortingOrder = 0;
    }
  }
}
