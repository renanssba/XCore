using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Actor2D : MonoBehaviour {

  public SpriteRenderer[] renderers;
  public SpriteRenderer[] buffAuraRenderers;
  public new ParticleSystem particleSystem;

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
    print("Flash Color:" + materialProperties.GetColor("_FlashColor"));
    print("Flash Amount:" + materialProperties.GetColor("_FlashAmount"));
  }


  void Awake() {
    materialProperties = new MaterialPropertyBlock();
  }


  public void SetCharacter(Person p) {
    battler = p;
    UpdateGraphics();
  }

  public void UpdateGraphics() {
    if(battler.GetType() == typeof(Person)) {
      UpdateCharacterGraphics();
    } else if(battler.GetType() == typeof(Enemy)) {
      // do nothing
    }
  }

  public void UpdateCharacterGraphics() {
    if(battler.id == 10) {
      renderers[4].sprite = Resources.Load<Sprite>("Characters/hiding-spot-" + BattleController.instance.currentDateLocation.ToString());
      spottedIcon.SetActive(battler.IsSpotted());
      return;
    }

    if(!string.IsNullOrEmpty(battler.name)) {
      if(battler.CurrentStatusConditionStacks("sad") == 0) {
        renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.body);
      } else {
        renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.sad);
      }
      renderers[1].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.underwear);
      switch(battler.CurrentStatusConditionStacks("unclothed")) {
        case 0:
          renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.casual);
          break;
        case 1:
          renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.unclothed);
          break;
        case 2:
          renderers[2].sprite = null;
          break;
      }
      renderers[3].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.bruises);
      if(battler.CurrentStatusConditionStacks("injured") > 0) {
        renderers[3].gameObject.SetActive(true);
      } else {
        renderers[3].gameObject.SetActive(false);
      }
      SetAuraVisibility();
    } else {
      gameObject.SetActive(false);
    }
  }

  public void SetClothing(string clothingType) {
    switch(clothingType) {
      case "uniform":
        renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.school);
        break;
      case "casual":
      default:
        renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.casual);
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
  }

  public void SetEnemyGraphics() {
    if(!string.IsNullOrEmpty(enemy.spriteName)) {
      renderers[0].sprite = LoadSprite("Enemies/" + enemy.spriteName);
    } else {
      gameObject.SetActive(false);
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


  public void CharacterAttackAnim() {
    renderers[0].transform.DOMoveX(0.3f, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public void EnemyAttackAnim() {
    renderers[0].transform.DOMoveX(-0.3f, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
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
    foreach(SpriteRenderer r in renderers) {
      r.GetPropertyBlock(materialProperties);
      materialProperties.SetColor("_FlashColor", Color.red);
      r.SetPropertyBlock(materialProperties);
    }
    FlashRenderer(transform, 0.1f, 0.8f, 0.2f);
  }

  public void ShineGreen() {
    foreach(SpriteRenderer r in renderers) {
      r.GetPropertyBlock(materialProperties);
      materialProperties.SetColor("_FlashColor", Color.green);
      r.SetPropertyBlock(materialProperties);
    }
    FlashRenderer(transform, 0.1f, 0.8f, 0.2f);
  }

  public void ShowDefendHitParticle() {
    GameObject particlePrefab = BattleController.instance.defendHitParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;
    //TODO: add "reduce damage" SFX
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);
  }

  public void FlashRenderer(Transform obj, float minFlash, float maxFlash, float flashTime) {
    foreach(SpriteRenderer r in renderers) {
      DOTween.Kill(r.material);

      r.GetPropertyBlock(materialProperties);
      materialProperties.SetFloat("_FlashAmount", minFlash);
      r.SetPropertyBlock(materialProperties);

      //r.material.SetFloat("_FlashAmount", minFlash);
      float currentFlashPower = minFlash;

      DOTween.To(() => currentFlashPower, x => currentFlashPower = x, maxFlash, flashTime).SetLoops(2, LoopType.Yoyo).OnUpdate(() => {
        r.GetPropertyBlock(materialProperties);
        materialProperties.SetFloat("_FlashAmount", currentFlashPower);
        r.SetPropertyBlock(materialProperties);
      }).OnComplete(() => {
        r.GetPropertyBlock(materialProperties);
        materialProperties.SetFloat("_FlashAmount", 0f);
        r.SetPropertyBlock(materialProperties);
      });

      //r.material.DOFloat(maxFlash, "_FlashAmount", flashTime).SetLoops(2, LoopType.Yoyo).OnComplete(() => {
      //  r.material.SetFloat("_FlashAmount", 0f);
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
      text += "Fraqueza:\n";
      for(int i = 0; i < weak.Length; i++) {
        text += Lean.Localization.LeanLocalization.GetTranslationText("attribute/" + weak[i].ToString()) + "\n";
      }
    }
    if(resistant.Length > 0) {
      if(!string.IsNullOrEmpty(text)) {
        text += "\n";
      }
      text += "Resistência:\n";
      for(int i = 0; i < resistant.Length; i++) {
        text += Lean.Localization.LeanLocalization.GetTranslationText("attribute/" + resistant[i].ToString()) + "\n";
      }
    }

    weaknessCardText.text = text;
  }


  public void ShowDamageParticle(int attribute, int damage, float effectivity) {
    string particleString = damage.ToString();
    Color particleColor = ResourcesManager.instance.attributeColor[0];

    if(effectivity > 1f) {
      particleString += "\n<size=12>SUPER!</size>";
    } else if(effectivity < 1f) {
      particleString += "\n<size=12>fraco</size>";
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
    string particleString = "+" + value + " HP";
    ShowParticleAnimation(particleString, Color.green);
  }

  public void ShowHealSpParticle(int value) {
    string particleString = "+" + value + " SP";
    ShowParticleAnimation(particleString, Color.cyan);
  }

  public void ShowStatusConditionParticle(StatusCondition statusCondition) {
    ShowParticleAnimationWithSprite("<size=50%>" + statusCondition.GetPrintableName() + "</size>", Color.gray, statusCondition.sprite);
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

    Debug.LogWarning("clicked person: " + battler.name);

    for(int i = 0; i < BattleController.instance.partyMembers.Length; i++) {
      if(BattleController.instance.partyMembers[i] == battler) {
        BattleController.instance.selectedTargetPartyId[currentPlayerTurn] = i;
      }
    }
    UIController.instance.HideHelpMessagePanel();
    UIController.instance.selectTargetPanel.SetActive(false);
    VsnController.instance.GotCustomInput();
  }


  public void SetFocusedSortingLayer(bool value) {
    foreach(SpriteRenderer s in renderers) {
      if(value && s.sortingOrder < 100) {
        s.sortingOrder += 100;
      } else if(!value && s.sortingOrder >= 100) {
        s.sortingOrder -= 100;
      }
    }

  }
}
