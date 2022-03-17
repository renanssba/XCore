using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using DG.Tweening;
using TMPro;


public class Actor2D : MonoBehaviour {

  [Header("- Data -")]
  public Battler battler;

  [Header("- Renderers -")]
  public SpriteRenderer[] renderers;
  public SpriteRenderer shadowRenderer;
  public GameObject choosingActionIndicator;

  [Header("- Animator -")]
  public Animator animator;

  private MaterialPropertyBlock materialProperties;

  const float attackAnimTime = 0.18f;


  [ContextMenu("Print Material Property Block")]
  public void PrintMaterialPropertyBlock() {
    Debug.Log("Flash Color:" + materialProperties.GetColor("_FlashColor"));
    Debug.Log("Flash Amount:" + materialProperties.GetFloat("_FlashAmount"));
  }


  void Awake() {
    materialProperties = new MaterialPropertyBlock();
  }


  public void SetCharacter(Battler p) {
    battler = p;
    UpdateGraphics();
  }

  public void UpdateGraphics() {
    if(battler.GetType() == typeof(Pilot)) {
      UpdateCharacterGraphics();
    } else if(battler.GetType() == typeof(Enemy)){
      UpdateEnemyGraphics();
    }
    renderers[1].gameObject.SetActive(false);
  }

  void UpdateCharacterGraphics() {
    if(battler.GetType() != typeof(Pilot)) {
      return;
    }

    if(!string.IsNullOrEmpty(battler.GetName())) {
      //Debug.LogWarning("UpdateCharacterGraphics. battler not NULL");
      renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.mecha);
      renderers[0].gameObject.SetActive(true);
    } else {
      gameObject.SetActive(false);
    }
  }

  void UpdateEnemyGraphics() {
    if(battler == null || battler.GetType() != typeof(Enemy)) {
      return;
    }

    renderers[0].sprite = LoadSprite("Enemies/" + ((Enemy)battler).spriteName);
    renderers[0].flipX = false;
    //shadowRenderer.transform.localScale = new Vector3(1.4f, 1f, 1f);
  }

  public void FaceLeft() {
    Vector3 scale = transform.localScale;
    if(battler == null || battler.GetType() == typeof(Enemy)) {
      scale.x = Mathf.Abs(scale.x);
    } else {
      scale.x = -Mathf.Abs(scale.x);
    }
    transform.localScale = scale;
  }

  public void FaceRight() {
    Vector3 scale = transform.localScale;
    if(battler == null || battler.GetType() == typeof(Enemy)) {
      scale.x = -Mathf.Abs(scale.x);
    } else {
      scale.x = Mathf.Abs(scale.x);
    }
    transform.localScale = scale;
  }

  public void MoveToPosition(Vector3 destination, float duration) {
    if(duration == 0f) {
      transform.position = destination;
    } else {
      transform.DOLocalMove(destination, duration);
    }
  }

  public Sprite LoadSprite(string spriteName) {
    Sprite sprite = Resources.Load<Sprite>(spriteName);
    if(sprite == null) {
      Debug.LogError("Error loading " + spriteName + " sprite. Please check its path");
    }
    return sprite;
  }

  public void SetChooseActionMode(bool value) {
    animator.SetBool("Choosing Action", value);
    choosingActionIndicator.SetActive(value);
  }

  public void SetAnimationParameter(string param, bool value) {
    animator.SetBool(param, value);
  }


  public IEnumerator CharacterAttackAnim(ActionSkin actionSkin, Actor2D[] targetActors = null) {
    switch(actionSkin.animation) {
      case SkillAnimation.throw_object:
        VsnAudioManager.instance.PlaySfx(actionSkin.sfxName);
        yield return ShowThrowItemAnimation(actionSkin.animationArgument, targetActors[0], new Vector3(0.08f, 0.08f, 0.08f));
        break;

      case SkillAnimation.multi_throw:
        VsnAudioManager.instance.PlaySfx(actionSkin.sfxName);
        foreach(Actor2D targetActor in targetActors) {
          StartCoroutine(ShowThrowItemAnimation(actionSkin.animationArgument, targetActor, new Vector3(0.08f, 0.08f, 0.08f)));
        }
        yield return new WaitForSeconds(1.5f);
        break;

      case SkillAnimation.projectile:
        VsnAudioManager.instance.PlaySfx(actionSkin.sfxName);
        yield return ProjectileAnimation(actionSkin);
        UpdateGraphics();
        break;


      case SkillAnimation.interact:
        VsnAudioManager.instance.PlaySfx(actionSkin.sfxName);
        yield return TackleAnimation();
        UpdateGraphics();
        break;


      case SkillAnimation.attack:
      case SkillAnimation.charged_attack:
        VsnAudioManager.instance.PlaySfx(actionSkin.sfxName);
        yield return TackleAnimation();
        UpdateGraphics();
        break;


      case SkillAnimation.active_offensive:
      case SkillAnimation.active_support:
      case SkillAnimation.long_charge:
      default:
        VsnAudioManager.instance.PlaySfx(actionSkin.sfxName);
        yield return TackleAnimation();
        //UpdateGraphics();
        break;
    }
  }

  public IEnumerator ProjectileAnimation(ActionSkin actionSkin) {
    GameObject projectileUsed = Resources.Load<GameObject>("Projectiles/" + actionSkin.animationArgument);
    Vector3 particlePrefabPos = projectileUsed.transform.localPosition;
    GameObject newParticle = Instantiate(projectileUsed, transform);

    float waitTime = attackAnimTime * 2f + 0.5f;
    if(newParticle.GetComponent<DieAfterTime>() != null) {
      waitTime = newParticle.GetComponent<DieAfterTime>().timeToWait;
    }    

    yield return new WaitForSeconds(waitTime);
  }

  public IEnumerator TackleAnimation() {
    float movementX = 0.3f;
    if(transform.position.x > 0f) {
      movementX = -0.3f;
    }
    transform.DOMoveX(movementX, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
    yield return new WaitForSeconds(attackAnimTime*2f + 0.5f);
  }

  public IEnumerator UseItemAnimation(Actor2D destiny, Item item) {
    yield return ShowThrowItemAnimation(item.spriteName, destiny, new Vector3(0.08f, 0.08f, 0.08f));
  }

  public void DefendActionAnimation() {
    GameObject particlePrefab = BattleController.instance.defenseActionParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;

    VsnAudioManager.instance.PlaySfx("buff_default");
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);
  }


  [ContextMenu("Shine Red")]
  public void ShineRed() {
    FlashRenderers(0.1f, 0.8f, 0.2f, Color.red);
  }

  [ContextMenu("Shine Green")]
  public void ShineGreen() {
    FlashRenderers(0.1f, 0.8f, 0.2f, Color.green);
  }

  public void ShowDefendHitParticle() {
    GameObject particlePrefab = BattleController.instance.defendHitParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;
    //TODO: add "reduce damage" SFX
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);
  }

  public void FlashRenderers(float minFlash, float maxFlash, float flashTime, Color shineColor) {
    foreach(SpriteRenderer currentRenderer in renderers) {
      float initialFlashPower = 0f;

      if(currentRenderer.material.HasProperty("_FlashAmount")) {
        initialFlashPower = currentRenderer.material.GetFloat("_FlashAmount");
        Debug.Log("Material has property _FlashColor. Value: " + initialFlashPower);

        if(initialFlashPower == 0f) {
          FlashRendererAmount(currentRenderer, minFlash, maxFlash, flashTime, shineColor);
        } else {
          FlashRendererColor(currentRenderer, shineColor, flashTime);
        }
      } else {
        continue;
      }
    }
  }

  public void FlashRendererAmount(SpriteRenderer currentRenderer, float minFlash, float maxFlash, float flashTime, Color shineColor) {
    float currentFlashPower = minFlash;

    Debug.LogWarning("Calling FlashRendererAmount!");

    DOTween.To(() => currentFlashPower, x => currentFlashPower = x, maxFlash, flashTime).SetLoops(2, LoopType.Yoyo).OnUpdate(() => {
      currentRenderer.GetPropertyBlock(materialProperties);
      materialProperties.SetFloat("_FlashAmount", currentFlashPower);
      materialProperties.SetColor("_FlashColor", shineColor);
      currentRenderer.SetPropertyBlock(materialProperties);
    }).OnComplete(() => {
      //Debug.LogWarning("Finish FLASH: final color: " + shineColor + ", flash amount: " + 0f);
      currentRenderer.GetPropertyBlock(materialProperties);
      materialProperties.SetFloat("_FlashAmount", 0f);
      materialProperties.SetColor("_FlashColor", shineColor);
      currentRenderer.SetPropertyBlock(materialProperties);
    });
  }

  public void FlashRendererColor(SpriteRenderer currentRenderer, Color shineColor, float flashTime) {
    Color initialColor = currentRenderer.material.GetColor("_FlashColor");
    Color currentColor = initialColor;

    Debug.LogWarning("Calling FlashRendererColor!");

    DOTween.To(() => currentColor, x => currentColor = x, shineColor, flashTime).SetLoops(2, LoopType.Yoyo).OnUpdate(() => {
      currentRenderer.GetPropertyBlock(materialProperties);
      materialProperties.SetFloat("_FlashAmount", 1f);
      materialProperties.SetColor("_FlashColor", currentColor);
      currentRenderer.SetPropertyBlock(materialProperties);
    }).OnComplete(() => {
      //Debug.LogWarning("Finish FLASH: final color: " + initialColor + ", flash amount: " + 1f);
      currentRenderer.GetPropertyBlock(materialProperties);
      materialProperties.SetFloat("_FlashAmount", 1f);
      materialProperties.SetColor("_FlashColor", initialColor);
      currentRenderer.SetPropertyBlock(materialProperties);
    });
  }

  public void TintActorToColor(float finalTint, float animTime, Color finalColor) {
    foreach(SpriteRenderer currentRenderer in renderers) {

      currentRenderer.GetPropertyBlock(materialProperties);
      float currentTintPower = materialProperties.GetFloat("_FlashAmount");
      Color currentColor = materialProperties.GetColor("_FlashColor");

      //Debug.Log("Initial tint values. Color: " + currentColor+", amount: "+ currentTintPower);

      DOTween.To(() => currentColor, y => currentColor = y, finalColor, animTime);

      DOTween.To(() => currentTintPower, x => currentTintPower = x, finalTint, animTime).OnUpdate(() => {
        //Debug.Log("New tint values. Color: " + currentColor + ", amount: " + currentTintPower);
        currentRenderer.GetPropertyBlock(materialProperties);
        materialProperties.SetFloat("_FlashAmount", currentTintPower);
        materialProperties.SetColor("_FlashColor", currentColor);
        currentRenderer.SetPropertyBlock(materialProperties);
      });
    }
  }

  [ContextMenu("Tint Actor To Pink")]
  public void TintActorToPink() {
    Color c = new Color(0.92f, 0.73f, 0.81f);
    TintActorToColor(1f, 1f, c);
  }

  [ContextMenu("Tint Actor To Normal")]
  public void TintActorToNormalColor() {
    Color c = new Color(0.92f, 0.73f, 0.81f);
    TintActorToColor(0f, 1f, c);
  }


  public void ShowDamageParticle(int damage, float effectivity) {
    string particleString = damage.ToString();
    Color particleColor = ResourcesManager.instance.attributeColor[0];

    Debug.LogWarning("damage: "+damage+", effectivity: "+effectivity);

    if(effectivity < 0f) {
      particleColor = Color.green;
    } else if(effectivity > 1f) {
      particleString += "\n<size=12>" + Lean.Localization.LeanLocalization.GetTranslationText("battle/super_effective") + "</size>";
    } else if(effectivity < 1f) {
      particleString += "\n<size=12>" + Lean.Localization.LeanLocalization.GetTranslationText("battle/ineffective") + "</size>";
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

  public IEnumerator ShowThrowItemAnimation(string itemName, Actor2D targetPerson, Vector3 scale) {
    GameObject particlePrefab = Resources.Load<GameObject>("Projectiles/" + itemName);
    GameObject newParticle;

    if(particlePrefab != null) {
      newParticle = Instantiate(particlePrefab, transform);
      newParticle.transform.SetParent(newParticle.transform.parent.parent);
    } else {
      particlePrefab = BattleController.instance.itemParticlePrefab;
      newParticle = Instantiate(particlePrefab, transform);
      Sprite itemSprite = Resources.Load<Sprite>("Icons/" + itemName);
      newParticle.GetComponent<SpriteRenderer>().sprite = itemSprite;
      newParticle.transform.SetParent(newParticle.transform.parent.parent);
      newParticle.transform.localScale = scale;
    }

    newParticle.GetComponent<JumpingParticle>().finalPosition =
      new Vector3(targetPerson.renderers[0].transform.position.x, particlePrefab.transform.position.y,
                  targetPerson.renderers[0].transform.position.z);

    yield return new WaitForSeconds(1.5f);
  }

  public void ClickedTargetSelectButton() {
    int currentBattler = VsnSaveSystem.GetIntVariable("currentBattlerTurn");

    Debug.LogWarning("clicked person: " + battler.GetName());

    for(int i = 0; i < BattleController.instance.partyMembers.Length; i++) {
      if(BattleController.instance.partyMembers[i] == battler) {
        BattleController.instance.selectedTargetPartyId[currentBattler] = (SkillTarget)i;
      }
    }
    UIController.instance.selectTargetPanel.SetActive(false);
    VsnController.instance.GotCustomInput();
  }


  public void SetFocusedSortingLayer(bool value) {
    ArmyAnimatorController army = GetComponent<ArmyAnimatorController>();
    int orderValue = value ? 100 : 0;

    if(army == null) {
      GetComponent<SortingGroup>().sortingOrder = orderValue;
    } else {
      foreach(Animator sr in army.bodyAnimators) {
        if(sr.GetComponent<SortingGroup>().sortingOrder > 50 && value == false) {
          sr.GetComponent<SortingGroup>().sortingOrder -= 100;
        } else if(sr.GetComponent<SortingGroup>().sortingOrder < 50 && value == true) {
          sr.GetComponent<SortingGroup>().sortingOrder += 100;
        }
      }
    }
  }
}
