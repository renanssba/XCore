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

  public List<Skill> allSkills;
  public TextAsset skillsFile;

  public List<StatusCondition> allStatusConditions;
  public TextAsset statusConditionsFile;

  public int maxHp = 10;
  public int hp = 10;

  public Person[] partyMembers;
  public TurnActionType[] selectedActionType;
  public Skill[] selectedSkills;
  public Item[] selectedItems;
  public int[] selectedTargetPartyId;
  public int dateLength;

  public DateEvent[] dateSegments;
  public DateLocation currentDateLocation;
  public int currentDateId;

  public GameObject damageParticlePrefab;
  public TextMeshProUGUI difficultyText;
  public Slider enemyHpSlider;
  const float attackAnimationTime = 0.15f;

  public Color greenColor;
  public Color redColor;


  public void Awake() {
    instance = this;
    partyMembers = new Person[0];
    selectedTargetPartyId = new int[0];
    LoadAllEnemies();
    LoadAllSkills();
    LoadAllStatusConditions();
    dateLength = 3;
  }

  public void StartBattle(Person boy, Person girl, int dateId) {
    GlobalData.instance.observedPeople = new Person[] {boy, girl};

    currentDateId = dateId;

    partyMembers = new Person[] {boy, girl, GlobalData.instance.people[4]};
    selectedSkills = new Skill[partyMembers.Length];
    selectedItems = new Item[partyMembers.Length];
    selectedActionType = new TurnActionType[partyMembers.Length];
    selectedTargetPartyId = new int[partyMembers.Length];

    maxHp = GlobalData.instance.GetCurrentRelationship().hearts * 8 + 26;

    FullHealParty();
    UIController.instance.ShowPartyPeopleCards();
    UIController.instance.UpdateDateUI();

    SetDateLengthAndLocation();
    GenerateDateEnemies();
    SetupDateLocation();
  }


  public void FullHealParty() {
    HealPartyHp(maxHp);
    RemovePartyStatusConditions();
    for(int i=0; i < partyMembers.Length; i++) {
      partyMembers[i].HealSp(partyMembers[i].maxSp);
    }
  }

  public void HealPartyHp(int value) {
    int initialHp = hp;
    hp += value;
    hp = Mathf.Min(hp, maxHp);
    hp = Mathf.Max(hp, 0);
    UIController.instance.AnimateHpChange(initialHp, hp);
  }

  public void RemovePartyStatusConditions() {
    for(int i = 0; i < partyMembers.Length; i++) {
      partyMembers[i].RemoveAllStatusConditions();
    }
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

  public int GetPartyMemberPosition(Person p) {
    for(int i=0; i<partyMembers.Length; i++) {
      if(partyMembers[i].id == p.id) {
        return i;
      }
    }
    return -1;
  }



  public void FinishSelectingCharacterAction() {
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");

    switch(selectedActionType[currentPlayerTurn]) {
      case TurnActionType.useItem:
        WaitToSelectAllyTarget(selectedActionType[currentPlayerTurn]);
        return;
      case TurnActionType.useSkill:
        if(selectedSkills[currentPlayerTurn].range == ActionRange.oneAlly) {
          WaitToSelectAllyTarget(selectedActionType[currentPlayerTurn]);
          return;
        }
        break;
      case TurnActionType.flee:
        VsnSaveSystem.SetVariable("currentPlayerTurn", partyMembers.Length);
        break;
    }

    UIController.instance.actionsPanel.EndActionSelect();
    VsnController.instance.GotCustomInput();
    UIController.instance.HideHelpMessagePanel();
  }

  public void WaitToSelectAllyTarget(TurnActionType actionType) {
    UIController.instance.actionsPanel.EndActionSelect();
    if(actionType == TurnActionType.useItem) {
      UIController.instance.ShowHelpMessagePanel("Escolha um aliado para usar o item:");
    } else if(actionType == TurnActionType.useSkill) {
      UIController.instance.ShowHelpMessagePanel("Escolha um aliado para usar a habilidade:");
    }    
    UIController.instance.selectTargetPanel.SetActive(true);
  }

  public void WaitToSelectEnemyTarget() {
    UIController.instance.selectTargetPanel.SetActive(true);
  }

  public void CharacterTurn(int partyMemberId) {
    VsnController.instance.state = ExecutionState.WAITING;
    VsnSaveSystem.SetVariable("selected_attribute", -1);

    switch(selectedActionType[partyMemberId]) {
      case TurnActionType.useSkill:
        CharacterUseAction(partyMemberId);
        break;
      case TurnActionType.useItem:
        StartCoroutine(ExecuteUseItem(partyMemberId, selectedTargetPartyId[partyMemberId], selectedItems[partyMemberId]));
        break;
      case TurnActionType.defend:
        VsnController.instance.state = ExecutionState.PLAYING;
        break;
      case TurnActionType.flee:
        StartCoroutine(ExecuteTryToFlee());
        break;
    }
  }

  public void CharacterUseAction(int partyMemberId) {
    // spend SP
    partyMembers[partyMemberId].SpendSp(selectedSkills[partyMemberId].spCost);

    switch(selectedSkills[partyMemberId].type) {
      case SkillType.attack:
        StartCoroutine(ExecuteCharacterAttack(partyMemberId, selectedSkills[partyMemberId]));
        break;
      case SkillType.active:
        StartCoroutine(ExecuteCharacterSkill(partyMemberId, selectedTargetPartyId[partyMemberId], selectedSkills[partyMemberId]));
        break;
      case SkillType.passive:
        Debug.LogError("Passive skill " + selectedSkills[partyMemberId].name + " used actively.");
        break;
    }
  }



  public int CalculateCharacterDamage(Person attacker, Skill usedSkill, DateEvent defender) {
    float damage;

    damage = (3f*attacker.AttributeValue((int)usedSkill.attribute) / Mathf.Max(2f*defender.attributes[4] + defender.attributes[(int)usedSkill.attribute], 1f));

    Debug.LogWarning("Character Hits! Damage:");
    Debug.Log("Defensive ratio (ATK/DEF):" + damage);

    damage *= (float)usedSkill.power * Random.Range(0.9f, 1.1f);

    damage *= defender.attributeEffectivity[(int)usedSkill.attribute];
    Debug.Log("Final damage: " + damage);

    // defend?
    //damage /= (selectedActionType[targetId] == TurnActionType.defend ? 2f : 1f);

    return Mathf.Max(1, Mathf.RoundToInt(damage));
  }

  public IEnumerator ExecuteCharacterAttack(int partyMemberId, Skill usedSkill) {
    // damage event HP
    int attributeId = (int)usedSkill.attribute;
    float effectivity = GetCurrentDateEvent().attributeEffectivity[attributeId];
    //int actionPowerLevel = (int)(partyMembers[partyMemberId].AttributeValue((int)usedSkill.attribute) * usedSkill.power);
    //float damageMultiplier = partyMembers[partyMemberId].DamageMultiplier();
    //int effectiveAttackDamage = (int)(actionPowerLevel * effectivity * damageMultiplier);
    int effectiveAttackDamage = CalculateCharacterDamage(partyMembers[partyMemberId], usedSkill, GetCurrentDateEvent());
    int initialHp = hp;

    VsnSaveSystem.SetVariable("selected_attribute", (int)usedSkill.attribute);


    DamageEnemyHp(effectiveAttackDamage);

    TheaterController.instance.CharacterAttackAnimation(partyMemberId, 0);

    DateEvent currentEvent = GetCurrentDateEvent();
    VsnAudioManager.instance.PlaySfx("hit_default");

    yield return new WaitForSeconds(attackAnimationTime);

    TheaterController.instance.enemyActor.Shine();
    TheaterController.instance.enemyActor.ShowDamageParticle(attributeId, effectiveAttackDamage, effectivity);

    yield return new WaitForSeconds(1f);
    enemyHpSlider.maxValue = currentEvent.maxHp;
    enemyHpSlider.DOValue(currentEvent.hp, 1f);

    yield return new WaitForSeconds(1.2f);
    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public IEnumerator ExecuteCharacterSkill(int partyMemberId, int targetId, Skill usedSkill) {
    TheaterController.instance.CharacterAttackAnimation(partyMemberId, 0);

    DateEvent currentEvent = GetCurrentDateEvent();
    VsnAudioManager.instance.PlaySfx("ui_success");

    yield return new WaitForSeconds(attackAnimationTime);

    switch(usedSkill.skillEffect) {
      case SkillEffect.sensor:
        TheaterController.instance.enemyActor.Shine();
        TheaterController.instance.enemyActor.ShowWeaknessCard(true);
        break;
      case SkillEffect.giveStatusCondition:
        partyMembers[targetId].ReceiveStatusConditionBySkill(usedSkill);
        TheaterController.instance.GetActorByIdInParty(targetId).Shine();
        break;
      case SkillEffect.healStatusCondition:
        partyMembers[targetId].RemoveStatusConditionBySkill(usedSkill);
        TheaterController.instance.GetActorByIdInParty(targetId).Shine();
        break;
    }
    
    yield return new WaitForSeconds(1.5f);
    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public IEnumerator ExecuteUseItem(int partyMemberId, int targetId, Item usedItem) {
    TheaterController.instance.CharacterAttackAnimation(partyMemberId, 0);

    DateEvent currentEvent = GetCurrentDateEvent();
    VsnAudioManager.instance.PlaySfx("ui_success");

    yield return new WaitForSeconds(attackAnimationTime);

    // spend item
    Inventory ivt = GlobalData.instance.people[0].inventory;
    ivt.ConsumeItem(usedItem.id, 1);


    // heal HP and SP
    if(usedItem.healHp > 0) {
      HealPartyHp(usedItem.healHp);
      TheaterController.instance.GetActorByIdInParty(targetId).ShowHealHpParticle(usedItem.healHp);
    }
    if(usedItem.healSp > 0) {
      partyMembers[targetId].HealSp(usedItem.healSp);
      TheaterController.instance.GetActorByIdInParty(targetId).ShowHealSpParticle(usedItem.healSp);
    }

    // give status condition
    if(usedItem.GivesStatusCondition()) {
      partyMembers[targetId].ReceiveStatusConditionByItem(usedItem);
    }

    // heal status condition
    if(usedItem.HealsStatusCondition()) {
      foreach(string condName in usedItem.healsConditionNames) {
        partyMembers[targetId].RemoveStatusCondition(condName);
      }
    }

    TheaterController.instance.GetActorByIdInParty(targetId).Shine();


    yield return new WaitForSeconds(1.5f);
    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public IEnumerator ExecuteTryToFlee() {
    VsnSaveSystem.SetVariable("currentPlayerTurn", partyMembers.Length);

    bool fleeSuccess = Random.Range(0, 100) < 60;
    fleeSuccess = true; // DEBUG TO TEST DAMAGE
    if(fleeSuccess) {
      yield return new WaitForSeconds(0.5f);
      int currentDateEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
      TheaterController.instance.EnemyLeavesScene();
      FleeDateSegment(currentDateEvent);

      yield return new WaitForSeconds(1f);
      Command.GotoCommand.StaticExecute("new_enemy_appears");
      VsnController.instance.state = ExecutionState.PLAYING;
    } else {
      yield return new WaitForSeconds(0.5f);
      VsnArgument[] args = new VsnArgument[1];
      args[0] = new VsnString("flee_fail");
      Command.GotoScriptCommand.StaticExecute("action_descriptions", args);
    }    
  }


  public void EnemyAttack() {
    int targetId = VsnSaveSystem.GetIntVariable("enemyAttackTargetId");
    VsnController.instance.state = ExecutionState.WAITING;
    StartCoroutine(ExecuteEnemyAttack(targetId, attackAnimationTime));
  }

  public int CalculateEnemyDamage(DateEvent attacker, int targetId) {
    float damage;
    Person defender = partyMembers[targetId];

    damage = (3f * attacker.attributes[((int)attacker.attackAttribute)] / Mathf.Max(2f * defender.AttributeValue(4) + defender.AttributeValue((int)attacker.attackAttribute), 1) );

    Debug.LogWarning("Enemy Hits! Damage:");
    Debug.Log("Defensive ratio (ATK/DEF):" + damage);

    damage *= (float)attacker.attackPower * Random.Range(0.9f, 1.1f);
    Debug.Log("Final damage: " + damage);

    // defend?
    damage /= (selectedActionType[targetId] == TurnActionType.defend ? 2f : 1f);

    return Mathf.Max(1, Mathf.RoundToInt(damage));
  }

  public IEnumerator ExecuteEnemyAttack(int targetId, float time) {
    // damage party HP
    DateEvent currentEvent = GetCurrentDateEvent();
    int attributeId = (int)currentEvent.attackAttribute;
    //int baseDamage = currentEvent.attackDamage;
    //int effectiveAttackDamage = (int)(baseDamage * partyMembers[targetId].defenses[attributeId]);
    int effectiveAttackDamage = CalculateEnemyDamage(currentEvent, targetId);
    int initialHp = hp;
    bool causeDamage = (currentEvent.attackPower != 0);

    TheaterController.instance.EnemyAttackAnimation();

    VsnAudioManager.instance.PlaySfx("hit_default");

    yield return new WaitForSeconds(time);

    TheaterController.instance.ShineCharacter(targetId);
    if(causeDamage) {
      TheaterController.instance.GetActorByIdInParty(targetId).ShowDamageParticle(attributeId, effectiveAttackDamage, 1f);
    }

    // chance to receive status condition
    int effectiveStatusConditionChance = currentEvent.giveStatusConditionChance;
    if(selectedActionType[targetId] == TurnActionType.defend) {
      if(effectiveStatusConditionChance == 100) {
        effectiveStatusConditionChance -= 30;
      } else {
        effectiveStatusConditionChance /= 2;
      }
    }
    if(Random.Range(0, 100) < effectiveStatusConditionChance) {
      foreach(string statusConditionName in currentEvent.givesConditionNames) {
        StatusCondition statusCondition = GetStatusConditionByName(statusConditionName);
        partyMembers[targetId].ReceiveStatusCondition(statusCondition);
      }
    }

    if(causeDamage) {
      yield return new WaitForSeconds(1f);
      DamagePartyHp(effectiveAttackDamage);
    }

    yield return new WaitForSeconds(1.2f);
    VsnController.instance.state = ExecutionState.PLAYING;
  }

  public void ShowChallengeResult(bool success) {
    Vector3 v = damageParticlePrefab.transform.localPosition;
    GameObject newParticle = Instantiate(damageParticlePrefab, new Vector3(transform.position.x, v.y, v.z), Quaternion.identity, transform);
    //newParticle.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

    newParticle.GetComponent<JumpingParticle>().duration = 1f;
    if(success == true) {
      newParticle.GetComponent<JumpingParticle>().jumpForce = 0.01f;
    } else {
      newParticle.GetComponent<JumpingParticle>().jumpForce = 0;
    }
    newParticle.GetComponent<TextMeshPro>().text = success ?
      Lean.Localization.LeanLocalization.GetTranslationText("date/success_message") :
      Lean.Localization.LeanLocalization.GetTranslationText("date/failure_message");
    newParticle.GetComponent<TextMeshPro>().color = success ? greenColor : redColor;
  }



  public void DamageEnemyHp(int dmg) {
    GetCurrentDateEvent().hp -= dmg;
    GetCurrentDateEvent().hp = Mathf.Max(GetCurrentDateEvent().hp, 0);
  }

  public void DamagePartyHp(int dmg) {
    int initialHp = hp;
    hp -= dmg;
    hp = Mathf.Max(hp, 0);
    UIController.instance.AnimateHpChange(initialHp, hp);
  }


  public void EndTurn() {
    foreach(Person p in partyMembers) {
      p.EndTurn();
    }
  }

  public void SetDateLengthAndLocation() {
    switch(currentDateId) {
      case 1:
        dateLength = 3;
        currentDateLocation = DateLocation.park;
        break;
      case 2:
        dateLength = 5;
        currentDateLocation = DateLocation.shopping;
        break;
      case 3:
        dateLength = 7;
        currentDateLocation = DateLocation.park;
        break;
      default:
        dateLength = 1;
        currentDateLocation = DateLocation.park;
        break;
    }
  }

  public void GenerateDateEnemies() {
    List<int> selectedEnemies = new List<int>();
    
    dateSegments = new DateEvent[dateLength];
    for(int i = 0; i < dateLength; i++) {
      int selectedId = GetNewEnemy(selectedEnemies);
      dateSegments[i] = allDateEvents[selectedId];
      selectedEnemies.Add(selectedId);
    }
    System.Array.Sort(dateSegments, new System.Comparison<DateEvent>(
                      (event1, event2) => event1.stage.CompareTo(event2.stage)));
    RecoverEnemiesHp();
  }

  public void SetupDateLocation(){
    TheaterController.instance.SetLocation(currentDateLocation.ToString());
    UIController.instance.dateTitleText.text = Lean.Localization.LeanLocalization.GetTranslationText("location/" + currentDateLocation.ToString());
  }

  public int GetNewEnemy(List<int> selectedEvents) {
    //return 7;
    return Random.Range(0, 10);

    int selectedEnemyId;
    do {
      //selectedId = Random.Range(0, allDateEvents.Count);
      selectedEnemyId = Random.Range(0, 9);

      Debug.LogWarning("selected location: " + allDateEvents[selectedEnemyId].location);
      Debug.LogWarning("date location: " + currentDateLocation.ToString());

    } while(selectedEvents.Contains(selectedEnemyId) ||
            (string.Compare(allDateEvents[selectedEnemyId].location, currentDateLocation.ToString()) != 0 &&
             string.Compare(allDateEvents[selectedEnemyId].location, DateLocation.generic.ToString()) != 0));

    return selectedEnemyId;
  }

  public Person GetCurrentPlayer() {
    int currentPlayer = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    if(currentPlayer < partyMembers.Length) {
      return partyMembers[currentPlayer];
    }
    return null;
  }

  public Person GetCurrentTarget() {
    int currentPlayer = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    if(currentPlayer < partyMembers.Length) {
      int target = selectedTargetPartyId[currentPlayer];
      if(target < partyMembers.Length) {
        return partyMembers[target];
      }
    }
    return null;
  }


  public void RecoverEnemiesHp() {
    for(int i = 0; i < dateLength; i++) {
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

    int selectedId = GetNewEnemy(currentUsedEvents);
    //selectedId = 1;
    dateSegments[positionId] = allDateEvents[selectedId];
    currentUsedEvents.Clear();

    RecoverEnemiesHp();
  }



  public void LoadAllEnemies() {
    allDateEvents = new List<DateEvent>();

    float guts, intelligence, charisma, magic;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(dateEventsFile, 1);
    foreach(Dictionary<string, string> dic in spreadsheetData.data) {
      guts = GetEffectivityByName(dic["guts effectivity"]);
      intelligence = GetEffectivityByName(dic["intelligence effectivity"]);
      charisma = GetEffectivityByName(dic["charisma effectivity"]);
      magic = GetEffectivityByName(dic["magic effectivity"]);
      allDateEvents.Add(new DateEvent {
        id = int.Parse(dic["id"]),
        name = dic["name"],
        scriptName = dic["script name"],
        level = int.Parse(dic["level"]),
        maxHp = int.Parse(dic["max hp"]),
        attributeEffectivity = new float[] { guts, intelligence, charisma, magic },
        spriteName = dic["sprite name"],
        stage = int.Parse(dic["stage"]),
        location = dic["location"],
        attackAttribute = Utils.GetAttributeByString(dic["attack attribute"]),
        attackPower = int.Parse(dic["attack power"]),
        attributes = new int[]{int.Parse(dic["guts"]), int.Parse(dic["intelligence"]),
          int.Parse(dic["charisma"]), int.Parse(dic["magic"]), int.Parse(dic["endurance"])},
        givesConditionNames = ItemDatabase.GetStatusConditionNamesByString(dic["gives status conditions"]),
        giveStatusConditionChance = int.Parse(dic["give status chance"])
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
  

  public void LoadAllSkills() {
    SpreadsheetData data = SpreadsheetReader.ReadTabSeparatedFile(skillsFile, 1);

    allSkills = new List<Skill>();
    foreach(Dictionary<string, string> entry in data.data) {
      //Debug.LogWarning("Importing Card: ");

      Skill newSkill = new Skill();

      newSkill.id = int.Parse(entry["id"]);

      newSkill.name = entry["name"];
      newSkill.description = entry["description"];
      newSkill.type = GetSkillTypeByString(entry["type"]);

      switch(entry["range"]) {
        case "self":
          newSkill.range = ActionRange.self;
          break;
        case "one_ally":
          newSkill.range = ActionRange.oneAlly;
          break;
        case "one_enemy":
          newSkill.range = ActionRange.oneEnemy;
          break;
        case "all_allies":
          newSkill.range = ActionRange.allAllies;
          break;
        case "all_enemies":
          newSkill.range = ActionRange.allEnemies;
          break;
        case "random_enemy":
          newSkill.range = ActionRange.randomEnemy;
          break;
      }

      newSkill.attribute = Utils.GetAttributeByString(entry["attribute"]);
      if(!string.IsNullOrEmpty(entry["power"])) {
        newSkill.power = int.Parse(entry["power"]);
      }
      if(!string.IsNullOrEmpty(entry["sp cost"])) {
        newSkill.spCost = int.Parse(entry["sp cost"]);
      }

      if(newSkill.type == SkillType.attack) {
        newSkill.sprite = ResourcesManager.instance.attributeSprites[(int)newSkill.attribute];
      } else {
        newSkill.sprite = Resources.Load<Sprite>("Icons/" + entry["sprite"]);
      }
      
      newSkill.skillEffect = GetSkillEffectByString(entry["skill effect"]);
      newSkill.healsConditionNames = ItemDatabase.GetStatusConditionNamesByString(entry["heals status conditions"]);
      newSkill.givesConditionNames = ItemDatabase.GetStatusConditionNamesByString(entry["gives status conditions"]);
      if(!string.IsNullOrEmpty(entry["duration"])) {
        newSkill.duration = int.Parse(entry["duration"]);
      }

      allSkills.Add(newSkill);
    }
  }

  public SkillEffect GetSkillEffectByString(string skillEffect) {
    for(int i = 0; i <= (int)SkillEffect.none; i++) {
      if(((SkillEffect)i).ToString() == skillEffect) {
        return (SkillEffect)i;
      }
    }
    return SkillEffect.none;
  }

  public SkillType GetSkillTypeByString(string skillType) {
    switch(skillType) {
      case "attack":
        return SkillType.attack;
      case "active":
        return SkillType.active;
      case "passive":
        return SkillType.passive;
      default:
        return SkillType.active;
    }
  }

  public Skill GetSkillById(int id) {
    foreach(Skill skill in allSkills) {
      if(skill.id == id) {
        return skill;
      }
    }
    return null;
  }

  public Skill GetSkillByName(string name) {
    foreach(Skill skill in allSkills) {
      if(skill.name == name) {
        return skill;
      }
    }
    return null;
  }


  public void LoadAllStatusConditions() {
    SpreadsheetData data = SpreadsheetReader.ReadTabSeparatedFile(statusConditionsFile, 1);

    allStatusConditions = new List<StatusCondition>();
    foreach(Dictionary<string, string> entry in data.data) {
      StatusCondition newStatusCondition = new StatusCondition();

      newStatusCondition.id = int.Parse(entry["id"]);
      newStatusCondition.name = entry["name"];
      //newStatusCondition.description = entry["description"];
      newStatusCondition.stackable = (entry["stackable"] == "TRUE" ? true : false);

      newStatusCondition.sprite = Resources.Load<Sprite>("Icons/" + entry["sprite"]);

      List<StatusConditionEffect> effects = new List<StatusConditionEffect>();
      List<float> effectsPower = new List<float>();
      for(int i=1; i<=3; i++) {
        if(!string.IsNullOrEmpty(entry["effect "+i])) {
          effects.Add(GetStatusConditionEffectByString(entry["effect " + i]));
          effectsPower.Add(float.Parse(entry["effect " + i+" power"]));
        }
      }
      newStatusCondition.statusEffect = effects.ToArray();
      newStatusCondition.statusEffectPower = effectsPower.ToArray();

      allStatusConditions.Add(newStatusCondition);
    }
  }

  public StatusConditionEffect GetStatusConditionEffectByString(string effect) {
    for(int i = 0; i <= (int)StatusConditionEffect.count; i++) {
      if(((StatusConditionEffect)i).ToString() == effect) {
        return (StatusConditionEffect)i;
      }
    }
    return StatusConditionEffect.raiseGuts;
  }


  public StatusCondition GetStatusConditionByName(string name) {
    foreach(StatusCondition c in allStatusConditions) {
      if(c.name == name) {
        return c;
      }
    }
    return null;
  }

  public StatusCondition GetStatusConditionById(int id) {
    foreach(StatusCondition c in allStatusConditions) {
      if(c.id == id) {
        return c;
      }
    }
    return null;
  }
}
