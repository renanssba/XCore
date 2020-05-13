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
  public SpriteRenderer[] buffAuraRenderers;
  public new ParticleSystem particleSystem;
  public SpriteRenderer shadowRenderer;

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

      if(GlobalData.instance.GetCurrentRelationship() != null) {
        renderers[5].sprite = ResourcesManager.instance.heartlockSprites[GlobalData.instance.GetCurrentRelationship().heartLocksOpened];
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

      foreach(Person p in GlobalData.instance.people) {
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


  public IEnumerator CharacterAttackAnim(SkillAnimation animation) {
    switch(animation) {
      case SkillAnimation.active_offensive:
      case SkillAnimation.active_support:
      case SkillAnimation.long_charge:
      default:
        yield return TackleAnimation();
        break;
      case SkillAnimation.run_over:
        yield return RunOverAnimation();
        break;
    }
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
    yield return ShowThrowItemAnimation(item.sprite, destiny, new Vector3(0.08f, 0.08f, 0.08f));
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
    FlashRenderer(0.1f, 0.8f, 0.2f, Color.red);
  }

  public void ShineGreen() {
    FlashRenderer(0.1f, 0.8f, 0.2f, Color.green);
  }

  public void ShowDefendHitParticle() {
    GameObject particlePrefab = BattleController.instance.defendHitParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;
    //TODO: add "reduce damage" SFX
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);
  }

  public void FlashRenderer(float minFlash, float maxFlash, float flashTime, Color finalColor) {
    foreach(SpriteRenderer currentRenderer in renderers) {
      //DOTween.Kill(renderers[i].material);
      float initialFlashPower;
      Color initialColor;

      currentRenderer.GetPropertyBlock(materialProperties);

      initialColor = materialProperties.GetColor("_FlashColor");
      initialFlashPower = materialProperties.GetFloat("_FlashAmount");

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
    }
  }

  public void TintActorToColor(float finalTint, float animTime, Color finalColor) {
    foreach(SpriteRenderer currentRenderer in renderers) {

      currentRenderer.GetPropertyBlock(materialProperties);
      float currentTintPower = materialProperties.GetFloat("_FlashAmount");
      Color currentColor = materialProperties.GetColor("_FlashColor");

      Debug.Log("Initial tint values. Color: " + currentColor+", amount: "+ currentTintPower);

      DOTween.To(() => currentColor, y => currentColor = y, finalColor, animTime);

      DOTween.To(() => currentTintPower, x => currentTintPower = x, finalTint, animTime).OnUpdate(() => {
        Debug.Log("New tint values. Color: " + currentColor + ", amount: " + currentTintPower);
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

  public IEnumerator ShowThrowItemAnimation(Sprite itemSprite, Actor2D targetPerson, Vector3 scale) {
    GameObject particlePrefab = BattleController.instance.itemParticlePrefab;
    GameObject newParticle = Instantiate(particlePrefab, transform);
    newParticle.transform.SetParent(newParticle.transform.parent.parent);

    newParticle.GetComponent<JumpingParticle>().finalPosition = new Vector3(targetPerson.renderers[0].transform.position.x, particlePrefab.transform.position.y, particlePrefab.transform.position.z);
    newParticle.GetComponent<SpriteRenderer>().sprite = itemSprite;
    newParticle.transform.localScale = scale;

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
    UIController.instance.CleanHelpMessagePanel();
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
