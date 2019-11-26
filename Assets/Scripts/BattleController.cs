using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class BattleController : MonoBehaviour {
  public static BattleController instance;

  public List<DateEvent> allDateEvents;
  public TextAsset dateEventsFile;

  public int maxHp = 10;
  public int hp = 10;

  public Person[] partyMembers;
  public int dateLength;

  public DateEvent[] dateSegments;

  public GameObject damageParticlePrefab;
  public TextMeshProUGUI difficultyText;
  public Slider enemyHpSlider;
  const float animTime = 0.15f;

  public Color greenColor;
  public Color redColor;


  public void Awake() {
    instance = this;
    LoadAllDateEvents();
    dateLength = 3;
  }

  public void StartBattle(Person boy, Person girl) {
    GlobalData.instance.observedPeople = new Person[] {boy, girl};

    partyMembers = new Person[] {boy, girl, GlobalData.instance.people[4]};

    maxHp = GlobalData.instance.GetCurrentRelationship().hearts * 10;

    FullHealParty();
    UIController.instance.ShowPartyPeopleCards();
    UIController.instance.UpdateDateUI();
  }


  public void FullHealParty() {
    HealPartyHp(maxHp);
    for(int i=0; i < partyMembers.Length; i++) {
      partyMembers[i].HealSp(partyMembers[i].maxSp);
    }
  }

  public void HealPartyHp(int value) {
    hp += value;
    hp = Mathf.Min(hp, maxHp);
    hp = Mathf.Max(hp, 0);
  }

  public DateEvent GetCurrentDateEvent() {
    int currentDateEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if(dateSegments.Length <= currentDateEvent) {
      return null;
    }
    return dateSegments[currentDateEvent];
  }

  public string GetCurrentDateEventName() {
    if(GetCurrentDateEvent() == null) {
      return "";
    }
    return dateSegments[VsnSaveSystem.GetIntVariable("currentDateEvent")].scriptName;
  }


  public void CharacterAction(int partyMemberId) {
    VsnController.instance.state = ExecutionState.WAITING;
    StartCoroutine(ExecuteCharacterAttack(partyMemberId, animTime));
  }

  public void EnemyAttack() {
    int targetId = Random.Range(0, 2);
    VsnSaveSystem.SetVariable("enemyAttackTargetId", targetId);
    VsnController.instance.state = ExecutionState.WAITING;
    StartCoroutine(ExecuteEnemyAttack(targetId, animTime));
  }


  public IEnumerator ExecuteCharacterAttack(int partyMemberId, float time) {
    // damage event HP
    int attributeId = VsnSaveSystem.GetIntVariable("selected_attribute");
    int actionPowerLevel = VsnSaveSystem.GetIntVariable("attribute_effective_level");
    float effectivity = GetCurrentDateEvent().attributeEffectivity[attributeId];
    int effectiveActionPower = (int)(actionPowerLevel * effectivity);

    DamageEnemyHp(effectiveActionPower);

    TheaterController.instance.CharacterAttackAnimation(partyMemberId, 0);

    DateEvent currentEvent = GetCurrentDateEvent();
    VsnAudioManager.instance.PlaySfx("hit_default");

    yield return new WaitForSeconds(time);

    TheaterController.instance.challengeActor.Shine();
    ShowParticleAnimation(attributeId, effectiveActionPower, effectivity);

    yield return new WaitForSeconds(1f);
    enemyHpSlider.maxValue = currentEvent.maxHp;
    enemyHpSlider.DOValue(currentEvent.hp, 1f);

    yield return new WaitForSeconds(1.2f);
    VsnController.instance.state = ExecutionState.PLAYING;
  }

  public IEnumerator ExecuteEnemyAttack(int targetId, float time) {
    // damage party HP
    DateEvent currentEvent = GetCurrentDateEvent();
    int attributeId = (int)currentEvent.attackAttribute;
    int baseDamage = currentEvent.attackDamage;
    int effectiveAttackDamage = (int)(baseDamage - partyMembers[targetId].AttributeValue(attributeId));
    int initialHp = hp;
    DamagePartyHp(effectiveAttackDamage);

    TheaterController.instance.EnemyAttackAnimation();

    VsnAudioManager.instance.PlaySfx("hit_default");

    yield return new WaitForSeconds(time);

    TheaterController.instance.ShineCharacter(targetId);
    ShowParticleAnimation(attributeId, effectiveAttackDamage, 1f);

    yield return new WaitForSeconds(1f);
    UIController.instance.AnimateHpDamage(initialHp, hp);

    yield return new WaitForSeconds(1.2f);
    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public void ShowParticleAnimation(int attribute, int attributeLevel, float effectivity) {
    string effectivityString = "";
    if(effectivity > 1f) {
      effectivityString = "\n<size=40>SUPER!</size>";
    } else if(effectivity < 1f) {
      effectivityString = "\n<size=40>fraco</size>";
    }
    GameObject newobj = Instantiate(damageParticlePrefab, UIController.instance.bgImage.transform.parent);
    newobj.GetComponent<TextMeshProUGUI>().text = attributeLevel.ToString() + effectivityString;
    newobj.GetComponent<TextMeshProUGUI>().color = ResourcesManager.instance.attributeColor[attribute];
  }

  public void ShowChallengeResult(bool success) {
    GameObject newobj = Instantiate(damageParticlePrefab, UIController.instance.bgImage.transform.parent);

    newobj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

    newobj.GetComponent<JumpingParticle>().duration = 1f;
    if(success == true) {
      newobj.GetComponent<JumpingParticle>().jumpForce = 10;
    } else {
      newobj.GetComponent<JumpingParticle>().jumpForce = 0;
    }
    newobj.GetComponent<TextMeshProUGUI>().text = success ?
      Lean.Localization.LeanLocalization.GetTranslationText("date/success_message") :
      Lean.Localization.LeanLocalization.GetTranslationText("date/failure_message");
    newobj.GetComponent<TextMeshProUGUI>().color = success ? greenColor : redColor;
  }



  public void DamageEnemyHp(int dmg) {
    GetCurrentDateEvent().hp -= dmg;
    GetCurrentDateEvent().hp = Mathf.Max(GetCurrentDateEvent().hp, 0);
  }

  public void DamagePartyHp(int dmg) {
    hp -= dmg;
    hp = Mathf.Max(hp, 0);
  }


  public void GenerateDate(int dateNumber = 1) {
    List<int> selectedEvents = new List<int>();
    int dateLocation = Random.Range(0, 2);
    VsnSaveSystem.SetVariable("date_location", dateLocation);

    switch(dateNumber) {
      case 1:
        dateLength = 3;
        break;
      case 2:
        dateLength = 5;
        break;
      case 3:
        dateLength = 7;
        break;
    }

    dateSegments = new DateEvent[dateLength];
    for(int i = 0; i < dateLength; i++) {
      int selectedId = GetNewDateEvent(selectedEvents);
      dateSegments[i] = allDateEvents[selectedId];
      selectedEvents.Add(selectedId);
    }
    System.Array.Sort(dateSegments, new System.Comparison<DateEvent>(
                                  (event1, event2) => event1.stage.CompareTo(event2.stage)));
    SetDifficultyForEvents();
  }

  public int GetNewDateEvent(List<int> selectedEvents) {
    return 0;

    int selectedId;
    string dateLocationName = ((DateLocation)VsnSaveSystem.GetIntVariable("date_location")).ToString();
    do {
      selectedId = Random.Range(0, allDateEvents.Count);

      Debug.LogWarning("selected location: " + allDateEvents[selectedId].location);
      Debug.LogWarning("date location: " + dateLocationName);

    } while(selectedEvents.Contains(selectedId) ||
            (string.Compare(allDateEvents[selectedId].location, dateLocationName) != 0 &&
             string.Compare(allDateEvents[selectedId].location, "generico") != 0));

    return selectedId;
  }

  public void SetDifficultyForEvents() {
    for(int i = 0; i < dateLength; i++) {
      //if(i < 3) {
      //  dateSegments[i].difficulty = 4;
      //} else if(i < 5) {
      //  dateSegments[i].difficulty = 5;
      //} else {
      //  dateSegments[i].difficulty = 6;
      //}
      dateSegments[i].maxHp = dateSegments[i].difficulty;
      dateSegments[i].hp = dateSegments[i].maxHp;
    }
  }

  public void FleeDateSegment(int positionId) {
    List<int> currentUsedEvents = new List<int>();
    foreach(DateEvent d in dateSegments) {
      currentUsedEvents.Add(d.id);
    }

    Debug.LogWarning("currentUsedEvents: ");
    foreach(int i in currentUsedEvents) {
      Debug.Log("i: " + i);
    }

    int selectedId = GetNewDateEvent(currentUsedEvents);
    dateSegments[positionId] = allDateEvents[selectedId];
    currentUsedEvents.Clear();

    SetDifficultyForEvents();
  }



  public void LoadAllDateEvents() {
    allDateEvents = new List<DateEvent>();

    float guts, intelligence, charisma, magic;
    DateEventInteractionType interaction = DateEventInteractionType.male;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(dateEventsFile, 1);
    foreach(Dictionary<string, string> dic in spreadsheetData.data) {
      guts = GetEffectivityByName(dic["Efetividade Valentia"]);
      intelligence = GetEffectivityByName(dic["Efetividade Inteligencia"]);
      charisma = GetEffectivityByName(dic["Efetividade Carisma"]);
      magic = GetEffectivityByName(dic["Efetividade Magia"]);
      switch(dic["Tipo de Interação"]) {
        case "male":
          interaction = DateEventInteractionType.male;
          break;
        case "female":
          interaction = DateEventInteractionType.female;
          break;
        case "couple":
          interaction = DateEventInteractionType.couple;
          break;
      }
      allDateEvents.Add(new DateEvent {
        id = int.Parse(dic["Id"]),
        scriptName = dic["Nome do Script"],
        difficulty = int.Parse(dic["Dificuldade"]),
        attributeEffectivity = new float[] { guts, intelligence, charisma, magic },
        spriteName = dic["Nome Sprite"],
        stage = int.Parse(dic["Etapa"]),
        location = dic["Localidade"],
        interactionType = interaction,
        attackAttribute = Utils.GetAttributeByString(dic["Atributo Ataque"]),
        attackDamage = int.Parse(dic["Dano"])
      });
    }
  }

  public float GetEffectivityByName(string name) {
    switch(name) {
      case "baixa":
        return 0.5f;
      case "normal":
        return 1f;
      case "super":
        return 2f;
    }
    return 1f;
  }
}
