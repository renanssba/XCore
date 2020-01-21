using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Actor2D : MonoBehaviour {

  public new SpriteRenderer renderer;

  public SpriteRenderer weaknessCardRenderer;
  public TextMeshPro weaknessCardText;

  public DateEvent dateChallenge;
  public Person person;

  public Button targetSelectButton;


  const float attackAnimTime = 0.18f;


  public void SetCharacterGraphics(Person p) {
    person = p;
    if(!string.IsNullOrEmpty(person.name)) {
      renderer.sprite = LoadSprite("Characters/" + person.name);
    } else {
      gameObject.SetActive(false);
    }
  }

  public void SetEnemyGraphics(DateEvent currentEvent) {
    dateChallenge = currentEvent;
    if(!string.IsNullOrEmpty(currentEvent.spriteName)) {
      renderer.sprite = LoadSprite("Challenges/" + currentEvent.spriteName);
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


  public void CharacterAttackAnim() {
    transform.DOMoveX(0.3f, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public void EnemyAttackAnim() {
    transform.DOMoveX(-0.3f, attackAnimTime).SetRelative().SetLoops(2, LoopType.Yoyo);
  }

  public void UseItemAnimation(Actor2D destiny, Item item) {
    ShowThrowItemAnimation(item.sprite, destiny);
  }

  //public void ReceiveItemAnimation(Item item) {

  //}

  public void Shine() {
    FlashRenderer(transform, 0.1f, 0.8f, 0.2f);
  }

  public void FlashRenderer(Transform obj, float minFlash, float maxFlash, float flashTime) {
    DOTween.Kill(renderer.material);
    renderer.material.SetFloat("_FlashAmount", minFlash);
    renderer.material.DOFloat(maxFlash, "_FlashAmount", flashTime).SetLoops(2, LoopType.Yoyo).OnComplete(()=> {
      renderer.material.SetFloat("_FlashAmount", 0f);
    });
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
    Color particleColor = ResourcesManager.instance.attributeColor[attribute];

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
    ShowParticleAnimation(particleString, Color.green);
  }

  public void ShowHealSpParticle(int value) {
    string particleString = "+" + value + " SP";
    ShowParticleAnimation(particleString, Color.green);
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
