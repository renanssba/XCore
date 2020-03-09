using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Actor2D : MonoBehaviour {

  public new SpriteRenderer[] renderers;
  public SpriteRenderer[] buffAuraRenderers;
  public ParticleSystem particleSystem;

  public SpriteRenderer weaknessCardRenderer;
  public TextMeshPro weaknessCardText;

  public DateEvent dateChallenge;
  public Person person;
  public Animator animator;

  public Button targetSelectButton;


  const float attackAnimTime = 0.18f;

    public MaterialPropertyBlock prop;

    [ContextMenu("Print Material Property Block")]
    public void PrintMaterialPropertyBlock()
    {
        print("Flash Color:" + prop.GetColor("_FlashColor"));
        print("Flash Amount:" + prop.GetColor("_FlashAmount"));
    }
    void Awake()
    {
        prop = new MaterialPropertyBlock();
    }


    public void SetCharacterGraphics(Person p) {
    person = p;
    UpdateCharacterGraphics();
  }

  public void UpdateCharacterGraphics() {
    if(!string.IsNullOrEmpty(person.name)) {
      if(person.CurrentStatusConditionStacks("sad") == 0) {
        renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.body);
      } else {
        renderers[0].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.sad);
      }      
      renderers[1].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.underwear);
      switch(person.CurrentStatusConditionStacks("unclothed")) {
        case 0:
          renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.casual);
          break;
        case 1:
          renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.unclothed);
          break;
        case 2:
          renderers[2].sprite = null;
          break;
      }
      SetAuraVisibility();
    } else {
      gameObject.SetActive(false);
    }
  }

  public void SetClothing(string clothingType) {
    switch(clothingType) {
      case "uniform":
        renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.school);
        break;
      case "casual":
      default:
        renderers[2].sprite = ResourcesManager.instance.GetCharacterSprite(person.id, CharacterSpritePart.casual);
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
    if(person.CurrentStatusConditionStacks("encouraged") > 0) {
      ShowAura( ResourcesManager.instance.attributeColor[(int)Attributes.guts] );
    } else if(person.CurrentStatusConditionStacks("focused") > 0) {
      ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.intelligence]);
    } else if(person.CurrentStatusConditionStacks("inspired") > 0) {
      ShowAura(ResourcesManager.instance.attributeColor[(int)Attributes.charisma]);
    } else if(person.CurrentStatusConditionStacks("blessed") > 0) {
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

  public void SetEnemyGraphics(DateEvent currentEvent) {
    dateChallenge = currentEvent;
    if(!string.IsNullOrEmpty(currentEvent.spriteName)) {
      renderers[0].sprite = LoadSprite("Enemies/" + currentEvent.spriteName);
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


  public void CharacterAttackAnim() {
    transform.DOMoveX(0.3f, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public void EnemyAttackAnim() {
    transform.DOMoveX(-0.3f, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public void UseItemAnimation(Actor2D destiny, Item item) {
    ShowThrowItemAnimation(item.sprite, destiny);
  }

  public void DefendActionAnimation() {
    GameObject particlePrefab = BattleController.instance.defenseActionParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;
    VsnAudioManager.instance.PlaySfx("buff_default");
    GameObject newParticle = Instantiate(particlePrefab, new Vector3(transform.position.x, particlePrefabPos.y, particlePrefabPos.z), Quaternion.identity, BattleController.instance.transform);
  }

  public void ShineRed() {
    foreach(SpriteRenderer r in renderers) {
      r.GetPropertyBlock(prop);
      prop.SetColor("_FlashColor", Color.red);
      r.SetPropertyBlock(prop);
      //r.material.SetColor("_FlashColor", Color.red);
    }    
    FlashRenderer(transform, 0.1f, 0.8f, 0.2f);
  }

  public void ShineGreen() {
    foreach(SpriteRenderer r in renderers) {
            r.GetPropertyBlock(prop);
            prop.SetColor("_FlashColor", Color.green);
            r.SetPropertyBlock(prop);
    }
    FlashRenderer(transform, 0.1f, 0.8f, 0.2f);
  }

  public void ShowDefendHitParticle() {
    GameObject particlePrefab = BattleController.instance.defendHitParticlePrefab;
    Vector3 particlePrefabPos = particlePrefab.transform.localPosition;
    //TODO: add "reduce damage" SFX
    GameObject newParticle = Instantiate(particlePrefab, new Vector3(transform.position.x, particlePrefabPos.y, particlePrefabPos.z), Quaternion.identity, BattleController.instance.transform);
  }

  public void FlashRenderer(Transform obj, float minFlash, float maxFlash, float flashTime) {
    foreach(SpriteRenderer r in renderers) {
      DOTween.Kill(r.material);

      r.GetPropertyBlock(prop);
      prop.SetFloat("_FlashAmount", minFlash);
      r.SetPropertyBlock(prop);

      //r.material.SetFloat("_FlashAmount", minFlash);
      float currentFlashPower = minFlash;

      DOTween.To(() => currentFlashPower, x => currentFlashPower = x, maxFlash, flashTime).SetLoops(2, LoopType.Yoyo).OnUpdate(() => {
        r.GetPropertyBlock(prop);
        prop.SetFloat("_FlashAmount", currentFlashPower);
        r.SetPropertyBlock(prop);
      }).OnComplete(()=> {
        r.GetPropertyBlock(prop);
        prop.SetFloat("_FlashAmount", 0.0f);
        r.SetPropertyBlock(prop);
      });

      //r.material.DOFloat(maxFlash, "_FlashAmount", flashTime).SetLoops(2, LoopType.Yoyo).OnComplete(() => {
      //  r.material.SetFloat("_FlashAmount", 0f);
      //});
    }
  }


  public void ShowWeaknessCard(bool activate) {
    weaknessCardRenderer.gameObject.SetActive(activate);
    if(activate) {
      VsnAudioManager.instance.PlaySfx("relationship_up");
      SetWeaknessCardText();
    }
  }

  public void SetWeaknessCardText() {
    if(dateChallenge == null) {
      return;
    }

    string text = "";
    Attributes[] weak = dateChallenge.GetWeaknesses();
    Attributes[] resistant = dateChallenge.GetResistances();

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
    string particleString = "+" + value + " "+ Lean.Localization.LeanLocalization.GetTranslationText("attribute/"+ attribute.ToString());
    Color particleColor = ResourcesManager.instance.attributeColor[(int)attribute];

    ShowParticleAnimation(particleString, particleColor);
  }

  public void ShowHealHpParticle(int value) {
    string particleString = "+" + value + " HP";
    ShowParticleAnimation(particleString, ResourcesManager.instance.attributeColor[3]);
  }

  public void ShowHealSpParticle(int value) {
    string particleString = "+" + value + " SP";
    ShowParticleAnimation(particleString, ResourcesManager.instance.attributeColor[3]);
  }

  public void ShowStatusConditionParticle(StatusCondition statusCondition) {
    ShowParticleAnimationWithSprite("<size=50%>"+statusCondition.GetPrintableName()+"</size>", Color.gray, statusCondition.sprite);
  }

  public void ShowParticleAnimationWithSprite(string text, Color color, Sprite particleSprite) {
    Vector3 particlePrefabPos = BattleController.instance.damageParticlePrefab.transform.localPosition;
    GameObject newParticle = Instantiate(BattleController.instance.damageParticlePrefab, new Vector3(transform.position.x, particlePrefabPos.y, particlePrefabPos.z), Quaternion.identity, BattleController.instance.transform);
    newParticle.GetComponent<TextMeshPro>().color = color;
    newParticle.GetComponent<TextMeshPro>().text = text;
    newParticle.GetComponentInChildren<SpriteRenderer>().sprite = particleSprite;
  }

  public void ShowParticleAnimation(string text, Color color) {
    Vector3 particlePrefabPos = BattleController.instance.damageParticlePrefab.transform.localPosition;
    GameObject newParticle = Instantiate(BattleController.instance.damageParticlePrefab, new Vector3(transform.position.x, particlePrefabPos.y, particlePrefabPos.z), Quaternion.identity, BattleController.instance.transform);
    newParticle.GetComponent<TextMeshPro>().color = color;
    newParticle.GetComponent<TextMeshPro>().text = text;
    newParticle.GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
  }

  public void ShowThrowItemAnimation(Sprite itemSprite, Actor2D targetPerson) {
    Vector3 particlePrefabPos = BattleController.instance.itemParticlePrefab.transform.localPosition;
    GameObject newParticle = Instantiate(BattleController.instance.itemParticlePrefab, new Vector3(transform.position.x, particlePrefabPos.y, particlePrefabPos.z), Quaternion.identity, BattleController.instance.transform);
    newParticle.GetComponent<JumpingParticle>().finalPosition = new Vector3(targetPerson.transform.position.x, particlePrefabPos.y, particlePrefabPos.z);
    newParticle.GetComponent<SpriteRenderer>().sprite = itemSprite;
  }

  public void ClickedTargetSelectButton() {
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");

    Debug.LogWarning("clicked person: " + person.name);

    for(int i=0; i<BattleController.instance.partyMembers.Length; i++) {
      if(BattleController.instance.partyMembers[i] == person) {
        BattleController.instance.selectedTargetPartyId[currentPlayerTurn] = i;
      }
    }
    UIController.instance.HideHelpMessagePanel();
    UIController.instance.selectTargetPanel.SetActive(false);
    VsnController.instance.GotCustomInput();
  }
}
