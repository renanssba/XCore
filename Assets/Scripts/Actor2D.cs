using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using DG.Tweening;
using TMPro;


public class Actor2D : MonoBehaviour {
  public string actorReference;
  public SpriteRenderer[] renderers;
  public SpriteRenderer[] dirtySplashRenderers;
  public SpriteRenderer[] buffAuraRenderers;
  public List<SpriteRenderer> overlays;
  public new ParticleSystem particleSystem;
  public SpriteRenderer shadowRenderer;

  public float[] initialRendererFlashPowers;
  public Color[] initialrendererColors;

  public SpriteRenderer weaknessCardRenderer;
  public TextMeshPro weaknessCardText;

  public Battler battler;
  public Enemy enemy;
  public Animator animator;

  public Button targetSelectButton;

  public GameObject choosingActionIndicator;
  public GameObject spottedIcon;


  const float attackAnimTime = 0.18f;

  public MaterialPropertyBlock materialProperties;



  [ContextMenu("Print Material Property Block")]
  public void PrintMaterialPropertyBlock() {
    Debug.Log("Flash Color:" + materialProperties.GetColor("_FlashColor"));
    Debug.Log("Flash Amount:" + materialProperties.GetFloat("_FlashAmount"));
  }


  void Awake() {
    materialProperties = new MaterialPropertyBlock();
    overlays = new List<SpriteRenderer>();
  }


  public void SetCharacter(Pilot p) {
    battler = p;
    UpdateGraphics();
  }

  public void UpdateGraphics() {
    if(battler.GetType() == typeof(Pilot)) {
      UpdateCharacterGraphics();
    }
  }

  public void UpdateToSpecificPose(SkillAnimation animationPose) {
    /// DEBUG
    return;

    if(actorReference == "enemy") {
      return;
    }

    renderers[0].gameObject.SetActive(true);
    renderers[1].gameObject.SetActive(false);

    switch(animationPose) {
      case SkillAnimation.attack:
      case SkillAnimation.charged_attack:
        renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.pose_punch);
        break;
      case SkillAnimation.shout:
        renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.pose_shout);
        break;
      case SkillAnimation.interact:
        renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.pose_interact);
        break;
    }
  }

  public void UpdateCharacterGraphics() {
    if(!string.IsNullOrEmpty(battler.GetName())) {
      renderers[0].gameObject.SetActive(true);
      renderers[1].gameObject.SetActive(false);

      //renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.character);
      renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.mecha);
      //renderers[1].GetComponent<SpriteMask>().sprite = renderers[1].sprite;
      //renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(battler.id, CharacterSpritePart.bruises);
      //if(battler.CurrentStatusConditionStacks("injured") > 0) {
      //  renderers[2].gameObject.SetActive(true);
      //} else {
      //  renderers[2].gameObject.SetActive(false);
      //}   

      SetAuraVisibility();
    } else {
      gameObject.SetActive(false);
    }
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

  public void SetAuraVisibility() {
    //Debug.LogWarning("SETTING AURA VISIBILITY");
    //if(battler.CurrentStatusConditionStacks("encouraged") > 0) {
    //  ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.maxHp]);
    //} else if(battler.CurrentStatusConditionStacks("focused") > 0) {
    //  ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.attack]);
    //} else if(battler.CurrentStatusConditionStacks("inspired") > 0) {
    //  ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.dodge]);
    //} else if(battler.CurrentStatusConditionStacks("blessed") > 0) {
    //  ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.endurance]);
    //} else {
    //  //Debug.LogWarning("SETTING AURA VISIBILITY TO FALSE");
    //  buffAuraRenderers[0].gameObject.SetActive(false);
    //}
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

      foreach(Pilot p in GlobalData.instance.pilots) {
        if(p.nameKey == actorName) {
          SetCharacter(p);
          return;
        }
      }
      //Debug.LogWarning("Setting enemy sprite to: " + actorName);

      //CharacterSpriteCollection spriteCollection = ResourcesManager.instance.GetCharacterSpriteCollection(actorName);

      //renderers[0].sprite = spriteCollection.baseBody;
      //renderers[0].flipX = true;

      //if(TheaterController.instance.bgRenderer.sprite.name.Contains("school")) {
      //  renderers[1].sprite = spriteCollection.schoolClothes;
      //} else {
      //  renderers[1].sprite = spriteCollection.casualClothes;
      //}
      //renderers[1].gameObject.SetActive(true);
      //renderers[1].flipX = true;
      //shadowRenderer.transform.localScale = Vector3.one;
    } else {
      renderers[0].sprite = LoadSprite("Enemies/" + spriteName);
      if(renderers[0].GetComponent<SpriteMask>() != null) {
        renderers[0].GetComponent<SpriteMask>().sprite = LoadSprite("Enemies/" + spriteName);
      }
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

  public void SetAnimationParameter(string param, bool value) {
    animator.SetBool(param, value);
  }

  public void SetAnimationTrigger(string triggerName) {
    if(triggerName == "Elimination" && buffAuraRenderers.Length > 0) {
      buffAuraRenderers[0].gameObject.SetActive(false);
    }
    animator.SetTrigger(triggerName);
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

      case SkillAnimation.run_over:
        VsnAudioManager.instance.PlaySfx("enemy_attack_sale_stampede");
        VsnAudioManager.instance.PlaySfx("enemy_attack_sale_stampede");
        yield return RunOverAnimation();
        break;

      case SkillAnimation.projectile:
        VsnAudioManager.instance.PlaySfx(actionSkin.sfxName);
        if(actionSkin.animationArgument == "shout" ||
           actionSkin.animationArgument == "talk insult") {
          UpdateToSpecificPose(SkillAnimation.shout);
        }
        yield return ProjectileAnimation(actionSkin);
        UpdateGraphics();
        break;


      case SkillAnimation.interact:
        VsnAudioManager.instance.PlaySfx(actionSkin.sfxName);
        UpdateToSpecificPose(SkillAnimation.interact);
        yield return TackleAnimation();
        UpdateGraphics();
        break;


      case SkillAnimation.attack:
      case SkillAnimation.charged_attack:
        VsnAudioManager.instance.PlaySfx(actionSkin.sfxName);
        UpdateToSpecificPose(SkillAnimation.attack);
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

  public IEnumerator RunOverAnimation() {
    float movementX = -7.8f;
    Vector3 pos = transform.localPosition;
    float animTime = 3f;

    TheaterController.instance.Screenshake(2f, animTime+0.7f);
    transform.DOMoveX(movementX, animTime).SetRelative().SetEase(Ease.InSine);
    yield return new WaitForSeconds(animTime);

    transform.localPosition = pos + new Vector3(3f, 0f, 0f);
    transform.DOMoveX(-3f, 0.8f).SetRelative();
    yield return new WaitForSeconds(0.8f);
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

  public void DistractedAnimation() {

  }


  public void DetectAnimation() {
    GameObject particlePrefab = BattleController.instance.detectParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;
    //VsnAudioManager.instance.PlaySfx("buff_default");
    GameObject newParticle = Instantiate(particlePrefab, transform);
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

    if(finalTint == 1f) {
      ShowInternalSprite();
    } else {
      HideInternalSprite();
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

  //public void TintActor

  public void ShowInternalSprite() {
    renderers[renderers.Length - 1].DOFade(1f, 1f);
  }

  public void HideInternalSprite() {
    renderers[renderers.Length - 1].DOFade(0f, 1f);
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

    if(effectivity < 0f) {
      particleColor = Color.green;
    } else if(effectivity > 1f) {
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
    //newParticle.transform.SetParent(newParticle.transform.parent.parent);

    newParticle.GetComponent<JumpingParticle>().finalPosition = new Vector3(targetPerson.renderers[0].transform.position.x, particlePrefab.transform.position.y, targetPerson.renderers[0].transform.position.z);

    yield return new WaitForSeconds(1.5f);
  }

  public void ClickedTargetSelectButton() {
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");

    Debug.LogWarning("clicked person: " + battler.GetName());

    for(int i = 0; i < BattleController.instance.partyMembers.Length; i++) {
      if(BattleController.instance.partyMembers[i] == battler) {
        BattleController.instance.selectedTargetPartyId[currentPlayerTurn] = (SkillTarget)i;
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

  public void AddOverlay(string name) {
    GameObject asd = Instantiate(TheaterController.instance.overlayPrefab, renderers[0].transform);
    asd.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Characters/" + name);
    overlays.Add(asd.GetComponent<SpriteRenderer>());
  }

  public void RemoveOveraly(string name) {
    SpriteRenderer overlay = null;
    for(int i=0; i<overlays.Count; i++) {
      if(overlays[i].sprite.name == name) {
        overlay = overlays[i];
      }
    }
    overlays.Remove(overlay);
    Destroy(overlay.gameObject);
  }
}
