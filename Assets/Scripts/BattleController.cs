using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class SkillsUsedEntry {
  public int partyMemberUsed;
  public int skillId;
}


public class BattleController : MonoBehaviour {
  public static BattleController instance;

  public List<Enemy> allEnemies;
  public TextAsset enemiesFile;

  public List<Skill> allSkills;
  public TextAsset skillsFile;
  public List<ActionSkin> allActionSkins;
  public TextAsset actionSkinsFile;

  public List<StatusCondition> allStatusConditions;
  public TextAsset statusConditionsFile;

  public int maxHp = 10;
  public int hp = 10;

  public const float maxStealth = 50f;
  public const float maxNegativeStealth = 20f;
  public float currentStealth;
  public float stealthLostWhenUsedItem = 20f;
  public float stealthRecoveredWhenIdle = 10f;
  public float stealthLostBySecond = 0f;

  public Person[] partyMembers;
  public TurnActionType[] selectedActionType;
  public Skill[] selectedSkills;
  public Item[] selectedItems;
  public int[] selectedTargetPartyId;
  public int dateLength;

  public Enemy[] dateEnemies;
  public DateLocation currentDateLocation;
  public int currentDateId;

  public GameObject defaultEnemyPrefab;

  public GameObject damageParticlePrefab;
  public GameObject itemParticlePrefab;
  public GameObject defenseActionParticlePrefab;
  public GameObject defendHitParticlePrefab;
  public GameObject detectParticlePrefab;

  public VsnConsoleSimulator consoleSimulator;

  const float attackAnimationTime = 0.15f;

  public Color greenColor;
  public Color redColor;

  public float damageShineTime;
  public float damageShineAlpha;


  public Enemy loadedEnemyCustomEvents;


  public void Awake() {
    instance = this;
    partyMembers = new Person[0];
    selectedTargetPartyId = new int[0];
    LoadAllEnemies();
    LoadAllSkills();
    LoadAllActionSkins();
    LoadAllStatusConditions();
    dateLength = 3;
  }

  public void Update() {
    int currentTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    if(currentTurn == 2 && ActionsPanel.instance.skillsPanel.gameObject.activeSelf == true) {
      RemoveStealth(Time.deltaTime * stealthLostBySecond);
    }  
  }


  public void SetupBattleStart(int dateId) {
    Person boy = GlobalData.instance.GetCurrentRelationship().GetBoy();
    Person girl = GlobalData.instance.GetCurrentRelationship().GetGirl();
    GlobalData.instance.observedPeople = new Person[] {boy, girl};

    currentDateId = dateId;

    partyMembers = new Person[] {boy, girl, GlobalData.instance.people[4]};
    selectedSkills = new Skill[partyMembers.Length];
    selectedItems = new Item[partyMembers.Length];
    selectedActionType = new TurnActionType[partyMembers.Length];
    selectedTargetPartyId = new int[partyMembers.Length];

    maxHp = GlobalData.instance.GetCurrentRelationship().GetMaxHp();

    FullHealParty();
    ClearSkillsUsageRegistry();
    UIController.instance.ShowPartyPeopleCards();
    UIController.instance.UpdateDateUI();

    VsnSaveSystem.SetVariable("battle_is_happening", true);

    SetDateLengthAndLocation();
  }

  public bool IsBattleHappening() {
    return VsnSaveSystem.GetBoolVariable("battle_is_happening");
  }


  public void FullHealParty() {
    HealPartyHp(maxHp);
    RemovePartyStatusConditions();
    for(int i=0; i < partyMembers.Length; i++) {
      partyMembers[i].HealSp(partyMembers[i].GetMaxSp(GlobalData.instance.GetCurrentRelationship().id) );
    }
    RecoverStealth(maxStealth);
  }

  public void ClearSkillsUsageRegistry() {
    partyMembers[0].ClearAllSkillsUsage();
    partyMembers[1].ClearAllSkillsUsage();
  }

  public void HealPartyHp(int value) {
    int initialHp = hp;
    hp += value;
    hp = Mathf.Min(hp, maxHp);
    hp = Mathf.Max(hp, 0);
    UIController.instance.AnimatePartyHpChange(initialHp, hp);
  }

  public void HealEnemyHp(int value) {
    int initialHp = hp;
    GetCurrentEnemy().hp += value;
    GetCurrentEnemy().hp = Mathf.Min(GetCurrentEnemy().hp, GetCurrentEnemy().maxHp);
    GetCurrentEnemy().hp = Mathf.Max(GetCurrentEnemy().hp, 0);
    UIController.instance.AnimateEnemyHpChange(initialHp, GetCurrentEnemy().hp);
  }

  public void RemovePartyStatusConditions() {
    for(int i = 0; i < partyMembers.Length; i++) {
      partyMembers[i].RemoveAllStatusConditions();
    }
  }

  public void RecoverStealth(float value) {
    Debug.LogWarning("Recover stealth: "+value);
    float initValue = currentStealth;
    currentStealth += value;
    currentStealth = Mathf.Min(currentStealth, maxStealth);
    if(initValue < 0 && currentStealth >= 0f) {
      /// SHOW FERTILIEL RECOVERED ANIMATION
      currentStealth = maxStealth;
      partyMembers[2].RemoveStatusCondition("spotted");
    }
    UIController.instance.AnimateStealthSliderChange(currentStealth, currentStealth);
    TheaterController.instance.angelActor.UpdateGraphics();
  }

  public void RemoveStealth(float value) {
    Debug.LogWarning("Remove stealth: "+value);
    float initValue = currentStealth;
    currentStealth -= value;
    currentStealth = Mathf.Max(currentStealth, 0f);
    if(initValue > 0 && currentStealth <= 0f) {
      StartCoroutine(ShowDetectionAnimation());
    } else {
      if(VsnController.instance.state == ExecutionState.WAITING) {
        VsnController.instance.state = ExecutionState.PLAYING;
      }
    }
    UIController.instance.AnimateStealthSliderChange(initValue, currentStealth);
  }

  public IEnumerator ShowDetectionAnimation() {
    /// SHOW FERTILIEL DETECTED ANIMATION
    //SfxManager.StaticPlayCancelSfx();
    HideActionButtons();
    yield return TheaterController.instance.DetectAngelAnimation();
    
    currentStealth = -maxNegativeStealth-stealthRecoveredWhenIdle;

    if(VsnController.instance.state == ExecutionState.WAITINGCUSTOMINPUT) {
      TheaterController.instance.SetCharacterChoosingAction(2);
      UIController.instance.SetupCurrentCharacterUi(2);
      UIController.instance.actionsPanel.Initialize(2);
    
      TheaterController.instance.angelActor.UpdateGraphics();
    } else {
      VsnController.instance.state = ExecutionState.PLAYING;
    }
  }



  public Enemy GetCurrentEnemy() {
    int currentDateEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if(dateEnemies.Length <= currentDateEvent) {
      return null;
    }
    return dateEnemies[currentDateEvent];
  }

  public string GetCurrentEnemyName() {
    if(GetCurrentEnemy() == null) {
      return "";
    }
    return dateEnemies[VsnSaveSystem.GetIntVariable("currentDateEvent")].scriptName;
  }

  public int GetPartyMemberPosition(Battler character) {
    if(partyMembers[0] == character) {
      return 0;
    }else if(partyMembers[1] == character) {
      return 1;
    }else if(GetCurrentEnemy() == character) {
      return 3;
    }
    return -1;
  }



  public void FinishSelectingCharacterAction() {
    int currentPlayerTurn = CurrentPlayerTargetId();

    switch(selectedActionType[currentPlayerTurn]) {
      case TurnActionType.useItem:
        WaitToSelectAllyTarget(selectedActionType[currentPlayerTurn]);
        return;
      case TurnActionType.useSkill:
        if(selectedSkills[currentPlayerTurn].range == ActionRange.oneAlly) {
          WaitToSelectAllyTarget(selectedActionType[currentPlayerTurn]);
          return;
        } else {
          selectedTargetPartyId[currentPlayerTurn] = 3;
        }
        break;
      case TurnActionType.flee:
        VsnSaveSystem.SetVariable("currentPlayerTurn", partyMembers.Length);
        break;
      case TurnActionType.idle:
        RecoverStealth(stealthRecoveredWhenIdle);
        break;
    }

    HideActionButtons();
    TheaterController.instance.SetCharacterChoosingAction(-1);
    VsnController.instance.GotCustomInput();
  }

  public void HideActionButtons() {
    UIController.instance.actionsPanel.EndActionSelect();
    ActionsPanel.instance.turnIndicator.SetActive(false);
    UIController.instance.HideHelpMessagePanel();
  }

  public void WaitToSelectAllyTarget(TurnActionType actionType) {
    UIController.instance.actionsPanel.EndActionSelect();
    if(actionType == TurnActionType.useItem) {
      UIController.instance.SetHelpMessageText("Escolha um aliado para usar o item:");
      UIController.instance.ShowHelpMessagePanel();
    } else if(actionType == TurnActionType.useSkill) {
      UIController.instance.SetHelpMessageText("Escolha um aliado para usar a habilidade:");
      UIController.instance.ShowHelpMessagePanel();
    }
    Utils.SelectUiElement(UIController.instance.selectTargets[0]);
    UIController.instance.selectTargetPanel.SetActive(true);
  }

  public void WaitToSelectEnemyTarget() {
    Utils.SelectUiElement(UIController.instance.selectTargets[CurrentPlayerTargetId()]);
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
        StartCoroutine(ExecuteDefend(partyMemberId));
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
        //StartCoroutine(ExecuteCharacterAttack(partyMemberId, selectedTargetPartyId[partyMemberId], selectedSkills[partyMemberId]));
        //break;
      case SkillType.active:
        StartCoroutine(ExecuteBattlerSkill(partyMemberId, selectedTargetPartyId[partyMemberId], selectedSkills[partyMemberId]));
        break;
      case SkillType.passive:
        Debug.LogError("Passive skill " + selectedSkills[partyMemberId].name + " used actively.");
        break;
    }
  }



  public int CalculateAttackDamage(Battler attacker, Skill usedSkill, Battler defender) {
    float damage;

    damage = (3f*attacker.AttributeValue((int)usedSkill.damageAttribute) / Mathf.Max(2f * defender.AttributeValue((int)Attributes.endurance) + defender.AttributeValue((int)usedSkill.damageAttribute), 1f));

    Debug.LogWarning("Character Hits! Damage:");
    Debug.Log("Offense/Defense ratio (ATK/DEF):" + damage);

    damage *= usedSkill.damagePower * Random.Range(0.9f, 1.1f);

    damage *= attacker.DamageMultiplier();

    damage *= defender.DamageTakenMultiplier(usedSkill.damageAttribute);
    Debug.Log("Final damage: " + damage);

    // defend?
    damage /= (defender.IsDefending() ? 2f : 1f);

    return Mathf.Max(1, Mathf.RoundToInt(damage));
  }

  //public IEnumerator ExecuteCharacterAttack(int attackerId, int targetId, Skill usedSkill) {
  //  Actor2D attackingActor = TheaterController.instance.GetActorByIdInParty(attackerId);
  //  Battler attacker = attackingActor.battler;
  //  Actor2D targetActor = TheaterController.instance.GetActorByIdInParty(targetId);
  //  Battler target = GetBattlerByTargetId(targetId);
    
  //  float effectivity = target.GetAttributeEffectivity(usedSkill.damageAttribute);
  //  int effectiveAttackDamage = CalculateAttackDamage(attacker, usedSkill, target);

  //  VsnSaveSystem.SetVariable("selected_attribute", (int)usedSkill.damageAttribute);

  //  yield return ShowBattleDescription(VsnSaveSystem.GetStringVariable("pre_attack"));


  //  TheaterController.instance.FocusActors(new Actor2D[] { attackingActor, targetActor });
  //  yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);


  //  attackingActor.CharacterAttackAnim();

  //  ActionSkin skin = GetActionSkin(partyMembers[attackerId], usedSkill);
  //  VsnAudioManager.instance.PlaySfx(skin.sfxName);

  //  yield return new WaitForSeconds(attackAnimationTime + 0.8f);

  //  if (effectivity > 1f) {
  //    VsnAudioManager.instance.PlaySfx("damage_effective");
  //  } else if (effectivity < 1f) {
  //    VsnAudioManager.instance.PlaySfx("damage_ineffective");
  //  } else {
  //    VsnAudioManager.instance.PlaySfx("damage_default");
  //  }
    
  //  targetActor.ShineRed();
  //  targetActor.ShowDamageParticle(effectiveAttackDamage, effectivity);

  //  yield return new WaitForSeconds(1f);
    
  //  target.TakeDamage(effectiveAttackDamage);
  //  yield return new WaitForSeconds(1.5f);

  //  UIController.instance.HideHelpMessagePanel();
  //  TheaterController.instance.UnfocusActors();
  //  yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);

  //  VsnController.instance.state = ExecutionState.PLAYING;
  //}



  public IEnumerator ShowBattleDescription(string text) {
    //UIController.instance.helpMessagePanel.ShowPanel();
    UIController.instance.helpMessageText.text = text;
    yield return consoleSimulator.ShowCharactersFromTheStart();
  }

  public IEnumerator AddtoBattleDescription(string textAdded) {
    //UIController.instance.helpMessagePanel.ShowPanel();
    int oldLength = UIController.instance.helpMessageText.text.Length;
    string oldText = UIController.instance.helpMessageText.text;

    if(!string.IsNullOrEmpty(consoleSimulator.consoleText.text)) {
      consoleSimulator.consoleText.text = oldText + "\n" + textAdded;
    } else {
      consoleSimulator.consoleText.text = oldText + textAdded;
    }

    Debug.LogWarning("AddtoBattleDescription: " + textAdded + ". old length: " + oldLength+". old text: " + consoleSimulator.consoleText.text);
    yield return consoleSimulator.ShowCharactersFromPoint(oldLength);
  }



  public IEnumerator ShowUseSkillMessage(string userName, Skill skill) {
    VsnSaveSystem.SetVariable("active", userName);
    VsnSaveSystem.SetVariable("selected_action_name", skill.GetPrintableName());

    string code = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("action/use_skill"));
    yield return ShowBattleDescription(code);
  }

  public IEnumerator ShowUseItemMessage(string userName, string actionName) {
    VsnSaveSystem.SetVariable("active", userName);
    VsnSaveSystem.SetVariable("selected_action_name", actionName);

    string code = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("action/use_item"));
    yield return ShowBattleDescription(code);
  }

  public IEnumerator ShowDefendMessage() {
    string code = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("action/defend"));
    yield return ShowBattleDescription(code);
  }

  public IEnumerator ShowGetStatusConditionMessage(string targetName, string statusConditionName) {
    VsnSaveSystem.SetVariable("target_name", targetName);
    VsnSaveSystem.SetVariable("status_condition", statusConditionName);

    string code = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("status_condition/receive"));
    yield return new WaitForSeconds(0.5f);
    yield return AddtoBattleDescription(code);
    yield return new WaitForSeconds(0.8f);
  }

  public void ShowTakeStatusConditionDescription(string targetName, StatusConditionEffect statusEffect) {    
    VsnSaveSystem.SetVariable("target_name", targetName);
    string code = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("status_condition/effect/"+statusEffect.ToString()));
    StartCoroutine(ShowStatusConditionDamage(code));
  }

  public IEnumerator ShowStatusConditionDamage(string code) {
    VsnController.instance.state = ExecutionState.WAITING;

    yield return ShowBattleDescription(code);
    yield return new WaitForSeconds(1.5f);

    UIController.instance.HideHelpMessagePanel();
    VsnController.instance.state = ExecutionState.PLAYING;
  }



  public ActionSkin GetActionSkin(Person user, Skill usedSkill) {
    string sexModifier = (user.isMale ? "_boy" : "_girl");
    string actionSkinName = SpecialCodes.InterpretStrings("\\vsn(" + usedSkill.damageAttribute.ToString() + "_action" + sexModifier + "_name)");
    return GetActionSkinByName(actionSkinName);
  }


  public IEnumerator ExecuteBattlerSkill(int skillUserId, int targetId, Skill usedSkill) {
    Actor2D skillUserActor = TheaterController.instance.GetActorByIdInParty(skillUserId);
    Actor2D targetActor = TheaterController.instance.GetActorByIdInParty(targetId);
    Battler target = targetActor.battler;
    Battler skillUser = skillUserActor.battler;


    /// register skill usage
    skillUser.RegisterUsedSkill(usedSkill.id);

    if(usedSkill.type == SkillType.attack) {
      yield return ShowBattleDescription(VsnSaveSystem.GetStringVariable("pre_attack"));
    } else {
      yield return ShowUseSkillMessage(skillUser.name, usedSkill);
    }

    TheaterController.instance.FocusActors(new Actor2D[] { skillUserActor, targetActor });
    yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);


    // skill cast animation
    if(usedSkill.animation != SkillAnimation.none &&
       usedSkill.animation != SkillAnimation.passive) {
      switch(usedSkill.animation) {
        case SkillAnimation.attack:
          if(skillUserId < 3) {
            ActionSkin skin = GetActionSkin(partyMembers[skillUserId], usedSkill);
            VsnAudioManager.instance.PlaySfx(skin.sfxName);
          } else {
            VsnAudioManager.instance.PlaySfx(skillUserActor.enemy.attackSfxName);
          }
          break;
        case SkillAnimation.active_offensive:
        case SkillAnimation.active_support:
          VsnAudioManager.instance.PlaySfx("skill_cast");
          break;
      }
      skillUserActor.CharacterAttackAnim();
      yield return new WaitForSeconds(attackAnimationTime + 0.8f);
    }


    // skill receive animation
    if(usedSkill.animation != SkillAnimation.none &&
       usedSkill.animation != SkillAnimation.passive) {
      switch(usedSkill.animation) {
        case SkillAnimation.attack:
        case SkillAnimation.charged_attack:
        case SkillAnimation.active_offensive:
          targetActor.ShineRed();
          break;
        case SkillAnimation.active_support:
          targetActor.ShineGreen();
          break;
      }
      //yield return new WaitForSeconds(0.4f);
    }


    // cause damage
    if(usedSkill.damagePower != 0) {
      int effectiveAttackDamage = CalculateAttackDamage(skillUser, usedSkill, target);

      float effectivity = target.DamageTakenMultiplier(usedSkill.damageAttribute);

      if(target.IsDefending()) {
        targetActor.ShowDefendHitParticle();
        VsnAudioManager.instance.PlaySfx("damage_block");
        TheaterController.instance.Screenshake(0.5f);
      } else if(effectivity > 1f) {
        VsnAudioManager.instance.PlaySfx("damage_effective");
        TheaterController.instance.Screenshake(2f);
      } else if(effectivity < 1f) {
        VsnAudioManager.instance.PlaySfx("damage_ineffective");
        TheaterController.instance.Screenshake(0.5f);
      } else {
        VsnAudioManager.instance.PlaySfx("damage_default");
        TheaterController.instance.Screenshake(1f);
      }

      targetActor.ShowDamageParticle(effectiveAttackDamage, effectivity);
      yield return new WaitForSeconds(1f);

      target.TakeDamage(effectiveAttackDamage);
      yield return new WaitForSeconds(1f);
    }


    // execute special effects
    switch(usedSkill.specialEffect) {
      case SkillSpecialEffect.sensor:
        VsnAudioManager.instance.PlaySfx("skill_activate_good");
        targetActor.ShineRed();
        targetActor.ShowWeaknessCard(true);
        yield return new WaitForSeconds(1f);
        break;
      case SkillSpecialEffect.becomeEnemyTarget:
        VsnSaveSystem.SetVariable("enemyAttackTargetId", skillUserId);
        break;
      case SkillSpecialEffect.divertEnemyTarget:
        VsnSaveSystem.SetVariable("enemyAttackTargetId", OtherPartyMemberId(skillUserId));
        break;
    }


    // heal status condition
    if(usedSkill.healsConditionNames.Length > 0) {
      target.RemoveStatusConditionBySkill(usedSkill);
    }


    // heal HP and SP
    if(usedSkill.healHp > 0 || usedSkill.healHpPercent > 0f || usedSkill.healSp > 0) {
      VsnAudioManager.instance.PlaySfx("heal_default");
      int effectiveHeal;

      if(usedSkill.healHp > 0) {
        effectiveHeal = (int)(usedSkill.healHp * skillUser.HealingSkillMultiplier());
        target.HealHP(effectiveHeal);
        targetActor.ShowHealHpParticle(effectiveHeal);
        yield return new WaitForSeconds(1f);
      }
      if(usedSkill.healHpPercent > 0) {
        effectiveHeal = (int)(usedSkill.healHpPercent * skillUser.HealingSkillMultiplier() * target.MaxHP());
        target.HealHP(effectiveHeal);
        targetActor.ShowHealHpParticle(effectiveHeal);
        yield return new WaitForSeconds(1f);
      }
      if(usedSkill.healSp > 0) {
        target.HealSp(usedSkill.healSp);
        targetActor.ShowHealSpParticle(usedSkill.healSp);
        yield return new WaitForSeconds(1f);
      }
    }


    // chance to give status condition
    if(usedSkill.givesConditionNames.Length > 0) {
      int effectiveStatusConditionChance = usedSkill.giveStatusChance;
      if(target.IsDefending() && target != skillUser) {
        effectiveStatusConditionChance -= Mathf.Min(effectiveStatusConditionChance / 2, 30);
      }

      if(Random.Range(0, 100) < effectiveStatusConditionChance) {
        VsnAudioManager.instance.PlaySfx("skill_activate_bad");

        foreach(string givenCondition in usedSkill.givesConditionNames) {
          target.ReceiveStatusConditionBySkill(usedSkill);
          StatusCondition statusCondition = GetStatusConditionByName(givenCondition);
          yield return ShowGetStatusConditionMessage(target.name, statusCondition.GetPrintableName());
        }
      } else {
        targetActor.ShowResistConditionParticle();
      }
      yield return new WaitForSeconds(1f);
    }


    // wait then end
    yield return new WaitForSeconds(0.5f);


    TheaterController.instance.UnfocusActors();
    UIController.instance.HideHelpMessagePanel();
    yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);

    VsnController.instance.state = ExecutionState.PLAYING;
  }

  public int OtherPartyMemberId(int id) {
    if(id == 0) {
      return 1;
    }
    return 0;
  }


  public IEnumerator ExecuteUseItem(int partyMemberId, int targetId, Item usedItem) {
    Actor2D userActor = TheaterController.instance.GetActorByIdInParty(partyMemberId);
    Actor2D targetActor = TheaterController.instance.GetActorByIdInParty(targetId);

    yield return ShowUseItemMessage(userActor.battler.name, usedItem.GetPrintableName());

    TheaterController.instance.FocusActors(new Actor2D[] { userActor, targetActor });
    yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);

    Enemy currentEvent = GetCurrentEnemy();
    VsnAudioManager.instance.PlaySfx("challenge_default");

    userActor.UseItemAnimation(targetActor, usedItem);

    yield return new WaitForSeconds(1.5f);

    // spend item
    Inventory ivt = GlobalData.instance.people[0].inventory;
    ivt.ConsumeItem(usedItem.id, 1);

    VsnAudioManager.instance.PlaySfx("item_use");
    targetActor.ShineGreen();

    // heal status condition
    if(usedItem.HealsStatusCondition()) {
      foreach(string condName in usedItem.healsConditionNames) {
        partyMembers[targetId].RemoveStatusCondition(condName);
      }
    }

    // heal HP and SP
    if(usedItem.healHp > 0) {
      int healingpower = (int)(usedItem.healHp * GlobalData.instance.GetCurrentRelationship().HealingItemMultiplier());
      HealPartyHp(healingpower);
      TheaterController.instance.GetActorByIdInParty(targetId).ShowHealHpParticle(healingpower);
      yield return new WaitForSeconds(1f);
    }
    if(usedItem.healSp > 0) {
      partyMembers[targetId].HealSp(usedItem.healSp);
      TheaterController.instance.GetActorByIdInParty(targetId).ShowHealSpParticle(usedItem.healSp);
      yield return new WaitForSeconds(1f);
    }

    // give status condition
    if(usedItem.GivesStatusCondition()) {
      partyMembers[targetId].ReceiveStatusConditionByItem(usedItem);
      bool receivedNewStatus = true;

      if(receivedNewStatus) {
        StatusCondition statusCondition = GetStatusConditionByName(usedItem.givesConditionNames[0]);
        yield return ShowGetStatusConditionMessage(targetActor.battler.name, statusCondition.GetPrintableName());
      }
    }
    yield return new WaitForSeconds(0.5f);

    TheaterController.instance.UnfocusActors();
    UIController.instance.HideHelpMessagePanel();
    yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);


    if(currentStealth <= stealthLostWhenUsedItem) {
      TheaterController.instance.angelActor.SetAttackMode(true);
    }

    // spend stealth bar
    RemoveStealth(stealthLostWhenUsedItem);
  }


  public IEnumerator ExecuteDefend(int partyMemberId) {
    Actor2D currentActor = TheaterController.instance.GetActorByIdInParty(partyMemberId);

    yield return ShowDefendMessage();
    currentActor.DefendActionAnimation();

    yield return new WaitForSeconds(1.5f);
    UIController.instance.HideHelpMessagePanel();
    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public IEnumerator ExecuteTryToFlee() {
    VsnSaveSystem.SetVariable("currentPlayerTurn", partyMembers.Length);

    bool fleeSuccess = Random.Range(0f, 1f) < GlobalData.instance.GetCurrentRelationship().FleeChance();
    //fleeSuccess = true; // DEBUG TO TEST DAMAGE
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

    Skill skillUsed = GetCurrentEnemy().DecideWhichSkillToUse();

    if(skillUsed.type == SkillType.attack) {
      StartCoroutine(ExecuteBattlerSkill(3, targetId, skillUsed));
    } else {
      StartCoroutine(ExecuteBattlerSkill(3, targetId, skillUsed));
    }
  }


  //public IEnumerator ExecuteEnemyAttack(int targetId) {
  //  Enemy attacker = GetCurrentEnemy();
  //  int attributeId = (int)attacker.attackAttribute;
  //  Battler target = GetBattlerByTargetId(targetId);

  //  Skill skillUsed = attacker.DecideWhichSkillToUse();

  //  Debug.LogWarning("Skill decided to use: " + skillUsed.name);

  //  int effectiveAttackDamage = CalculateAttackDamage(attacker, skillUsed, target);

  //  Actor2D targetActor = TheaterController.instance.GetActorByIdInParty(targetId);
  //  bool causeDamage = (attacker.attackPower != 0);


  //  yield return ShowBattleDescription(VsnSaveSystem.GetStringVariable("pre_attack"));


  //  TheaterController.instance.FocusActors(new Actor2D[] { targetActor, TheaterController.instance.enemyActor });
  //  yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);


  //  VsnAudioManager.instance.PlaySfx(attacker.attackSfxName);
  //  TheaterController.instance.enemyActor.EnemyAttackAnim();
  //  yield return new WaitForSeconds(attackAnimationTime + 0.8f);

  //  targetActor.ShineRed();

  //  // cause damage
  //  if(causeDamage) {
  //    if(selectedActionType[targetId] == TurnActionType.defend) {
  //      targetActor.ShowDefendHitParticle();
  //    }

  //    float effectivity = target.DamageTakenMultiplier(attacker.attackAttribute);

  //    if(target.IsDefending()) {
  //      VsnAudioManager.instance.PlaySfx("damage_block");
  //    } else if(effectivity > 1f) {
  //      VsnAudioManager.instance.PlaySfx("damage_effective");
  //    } else if(effectivity < 1f) {
  //      VsnAudioManager.instance.PlaySfx("damage_ineffective");
  //    } else {
  //      VsnAudioManager.instance.PlaySfx("damage_default");
  //    }

  //    TheaterController.instance.Screenshake(1f);
  //    targetActor.ShowDamageParticle(effectiveAttackDamage, effectivity);
  //    yield return new WaitForSeconds(1f);

  //    target.TakeDamage(effectiveAttackDamage);
  //    yield return new WaitForSeconds(1f);
  //  }

  //  // chance to receive status condition
  //  int effectiveStatusConditionChance = attacker.giveStatusConditionChance;
  //  if(selectedActionType[targetId] == TurnActionType.defend) {
  //    effectiveStatusConditionChance -= Mathf.Min(effectiveStatusConditionChance/2, 30);
  //  }
  //  if(Random.Range(0, 100) < effectiveStatusConditionChance) {
  //    foreach(string statusConditionName in attacker.givesConditionNames) {
  //      StatusCondition statusCondition = GetStatusConditionByName(statusConditionName);
  //      bool receivedNewStatus = partyMembers[targetId].ReceiveStatusCondition(statusCondition);

  //      if(receivedNewStatus) {
  //        yield return ShowGetStatusConditionMessage(targetActor.battler.name, statusCondition.GetPrintableName());
  //      }
  //    }
  //  }
  //  yield return new WaitForSeconds(0.5f);


  //  UIController.instance.HideHelpMessagePanel();
  //  TheaterController.instance.UnfocusActors();
  //  yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);

  //  VsnController.instance.state = ExecutionState.PLAYING;
  //}

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
    newParticle.GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
  }



  public void DamageEnemyHp(int dmg) {
    int initialHp = GetCurrentEnemy().hp;
    GetCurrentEnemy().hp -= dmg;
    GetCurrentEnemy().hp = Mathf.Max(GetCurrentEnemy().hp, 0);
    UIController.instance.AnimateEnemyHpChange(initialHp, GetCurrentEnemy().hp);
  }

  public void DamagePartyHp(int dmg) {
    int initialHp = hp;
    hp -= dmg;
    hp = Mathf.Max(hp, 0);
    UIController.instance.AnimatePartyHpChange(initialHp, hp);
  }


  public void EndTurn() {
    foreach(Person p in partyMembers) {
      p.EndTurn();
    }
    GetCurrentEnemy().EndTurn();
  }

  public void SetDateLengthAndLocation() {
    switch(currentDateId) {
      case 1:
        dateLength = 2;
        dateEnemies = new Enemy[] {allEnemies[0], allEnemies[9]};
        currentDateLocation = DateLocation.park;
        break;
      case 2:
        dateLength = 3;
        currentDateLocation = DateLocation.shopping;
        GenerateDateEnemies();
        break;
      case 3:
        dateLength = 4;
        currentDateLocation = DateLocation.park;
        GenerateDateEnemies();
        break;
      default:
        dateLength = 1;
        currentDateLocation = DateLocation.park;
        GenerateDateEnemies();
        break;
    }
  }

  public void SetCustomBattle(int enemyId) {
    currentDateLocation = DateLocation.park;
    dateLength = 1;
    dateEnemies = new Enemy[] {allEnemies[enemyId]};
  }

  public void GenerateDateEnemies() {
    List<int> selectedEnemies = new List<int>();

    dateEnemies = new Enemy[dateLength];
    for(int i = 0; i < dateLength; i++) {
      int selectedId = GetNewEnemy(selectedEnemies);
      dateEnemies[i] = allEnemies[selectedId];
      selectedEnemies.Add(selectedId);
    }
    System.Array.Sort(dateEnemies, new System.Comparison<Enemy>(
                      (event1, event2) => event1.stage.CompareTo(event2.stage)));
    RecoverEnemiesHp();
  }

  public void SetupDateLocation(){
    TheaterController.instance.SetLocation(currentDateLocation.ToString());
    UIController.instance.dateTitleText.text = Lean.Localization.LeanLocalization.GetTranslationText("location/" + currentDateLocation.ToString());
  }

  public int GetNewEnemy(List<int> selectedEvents) {
    //return 2;
    //return 4;
    //return 6;
    //return 7;
    //return 11;
    //return Random.Range(5, 9);
    //return Random.Range(0, 12);

    int selectedEnemyId;
    do {
      //selectedId = Random.Range(0, allDateEvents.Count);
      selectedEnemyId = Random.Range(0, 9);

      Debug.LogWarning("selected location: " + allEnemies[selectedEnemyId].location);
      Debug.LogWarning("date location: " + currentDateLocation.ToString());

    } while(selectedEvents.Contains(selectedEnemyId) ||
            (string.Compare(allEnemies[selectedEnemyId].location, currentDateLocation.ToString()) != 0 &&
             string.Compare(allEnemies[selectedEnemyId].location, DateLocation.generic.ToString()) != 0));

    return selectedEnemyId;
  }

  public int CurrentPlayerTargetId() {
    return VsnSaveSystem.GetIntVariable("currentPlayerTurn");
  }

  public Person GetCurrentPlayer() {
    int currentPlayer = CurrentPlayerTargetId();
    if(currentPlayer < partyMembers.Length) {
      return partyMembers[currentPlayer];
    }
    return null;
  }

  public Person GetCurrentTarget() {
    int currentPlayer = CurrentPlayerTargetId();
    if(currentPlayer < partyMembers.Length) {
      int target = selectedTargetPartyId[currentPlayer];
      if(target < partyMembers.Length) {
        return partyMembers[target];
      }
    }
    return null;
  }

  public Battler GetBattlerByTargetId(int targetId) {
    if(targetId < 3) {
      return partyMembers[targetId];
    }
    return GetCurrentEnemy();
  }


  public void RecoverEnemiesHp() {
    for(int i = 0; i < dateLength; i++) {
      dateEnemies[i].hp = dateEnemies[i].maxHp;
    }
  }

  public void FleeDateSegment(int positionId) {
    List<int> currentUsedEvents = new List<int>();
    foreach(Enemy d in dateEnemies) {
      currentUsedEvents.Add(d.id);
    }

    Debug.LogWarning("currentUsedEvents: ");
    foreach(int i in currentUsedEvents) {
      Debug.Log("i: " + i);
    }

    int selectedId = GetNewEnemy(currentUsedEvents);
    //selectedId = 1;
    dateEnemies[positionId] = allEnemies[selectedId];
    currentUsedEvents.Clear();

    RecoverEnemiesHp();
  }



  public void LoadAllEnemies() {
    allEnemies = new List<Enemy>();

    float guts, intelligence, charisma;
    string[] loadedTags;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(enemiesFile, 1);
    foreach(Dictionary<string, string> dic in spreadsheetData.data) {
      Debug.LogWarning("Loading enemy: " + dic["name"]);

      Enemy loadedEnemy = JsonUtility.FromJson<Enemy>("{\"activeSkillLogics\" :" + dic["active skills logic"] +
                                                      ", \"customEvents\" :" + dic["custom events"] + "}");
      guts = GetEffectivityByName(dic["guts effectivity"]);
      intelligence = GetEffectivityByName(dic["intelligence effectivity"]);
      charisma = GetEffectivityByName(dic["charisma effectivity"]);
      loadedTags = Utils.SeparateTags(dic["tags"]);


      allEnemies.Add(new Enemy {
        id = int.Parse(dic["id"]),
        name = dic["name"],
        scriptName = dic["script name"],
        level = int.Parse(dic["level"]),
        maxHp = int.Parse(dic["max hp"]),
        attributeEffectivity = new float[] { guts, intelligence, charisma },
        spriteName = dic["sprite name"],
        attackSfxName = dic["attack sfx"],
        appearSfxName = dic["appear sfx"],
        stage = int.Parse(dic["stage"]),
        location = dic["location"],
        attributes = new int[]{int.Parse(dic["guts"]), int.Parse(dic["intelligence"]),
          int.Parse(dic["charisma"]), int.Parse(dic["endurance"])},
        passiveSkills = Utils.SeparateInts(dic["passive skills"]),
        activeSkillLogics = loadedEnemy.activeSkillLogics,
        customEvents = loadedEnemy.customEvents,
        tags = loadedTags
      }); ;
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
      Debug.LogWarning("Importing skill: " + entry["name"]);

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
      if(!string.IsNullOrEmpty(entry["sp cost"])) {
        newSkill.spCost = int.Parse(entry["sp cost"]);
      }

      newSkill.specialEffect = GetSkillEffectByString(entry["special effect"]);
      if(!string.IsNullOrEmpty(entry["effect power"])) {
        newSkill.effectPower = float.Parse(entry["effect power"]);
      }

      newSkill.damageAttribute = Utils.GetAttributeByString(entry["damage attribute"]);
      if(!string.IsNullOrEmpty(entry["damage power"])) {
        newSkill.damagePower = int.Parse(entry["damage power"]);
      }

      newSkill.healsConditionNames = ItemDatabase.GetStatusConditionNamesByString(entry["heals status conditions"]);
      newSkill.givesConditionNames = ItemDatabase.GetStatusConditionNamesByString(entry["gives status conditions"]);

      if(!string.IsNullOrEmpty(entry["passive trigger"])) {
        newSkill.activationTrigger = GetPassiveSkillTriggerByString(entry["passive trigger"]);
      } else {
        newSkill.activationTrigger = PassiveSkillActivationTrigger.none;
      }
      if(!string.IsNullOrEmpty(entry["trigger conditions"])) {
        newSkill.triggerConditions = Utils.SeparateTags(entry["trigger conditions"]);
      }
      if(!string.IsNullOrEmpty(entry["trigger chance"])) {
        newSkill.triggerChance = float.Parse(entry["trigger chance"]);
      }

      if(newSkill.type == SkillType.attack) {
        newSkill.sprite = ResourcesManager.instance.attributeSprites[(int)newSkill.damageAttribute];
      } else {
        newSkill.sprite = Resources.Load<Sprite>("Icons/" + entry["sprite"]);
      }
      newSkill.animation = GetSkillAnimationByString(entry["animation"]);

      newSkill.tags = Utils.SeparateTags(entry["tags"]);

      if(!string.IsNullOrEmpty(entry["heal hp"])) {
        newSkill.healHp = int.Parse(entry["heal hp"]);
      }
      if(!string.IsNullOrEmpty(entry["heal hp percent"])) {
        newSkill.healHpPercent = float.Parse(entry["heal hp percent"]);
      }
      if(!string.IsNullOrEmpty(entry["heal sp"])) {
        newSkill.healSp = int.Parse(entry["heal sp"]);
      }
      if(!string.IsNullOrEmpty(entry["give status chance"])) {
        newSkill.giveStatusChance = int.Parse(entry["give status chance"]);
      }
      if(!string.IsNullOrEmpty(entry["duration"])) {
        newSkill.duration = int.Parse(entry["duration"]);
      }

      allSkills.Add(newSkill);
    }
  }

  public SkillSpecialEffect GetSkillEffectByString(string skillEffect) {
    for(int i = 0; i <= (int)SkillSpecialEffect.none; i++) {
      if(((SkillSpecialEffect)i).ToString() == skillEffect) {
        return (SkillSpecialEffect)i;
      }
    }
    return SkillSpecialEffect.none;
  }

  public PassiveSkillActivationTrigger GetPassiveSkillTriggerByString(string passiveTrigger) {
    for(int i = 0; i <= (int)PassiveSkillActivationTrigger.none; i++) {
      if(((PassiveSkillActivationTrigger)i).ToString() == passiveTrigger) {
        return (PassiveSkillActivationTrigger)i;
      }
    }
    return PassiveSkillActivationTrigger.none;
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

  public SkillAnimation GetSkillAnimationByString(string skillAnimation) {
    for(int i = 0; i <= (int)SkillAnimation.none; i++) {
      if(((SkillAnimation)i).ToString() == skillAnimation) {
        return (SkillAnimation)i;
      }
    }
    return SkillAnimation.none;
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


  public void LoadAllActionSkins() {
    SpreadsheetData data = SpreadsheetReader.ReadTabSeparatedFile(actionSkinsFile, 1);

    allActionSkins = new List<ActionSkin>();
    foreach(Dictionary<string, string> entry in data.data) {
      ActionSkin newSkin = new ActionSkin() {
        id = entry["id"],
        name = entry["name"],
        buttonName = entry["button name"],
        sfxName = entry["sfx"]
      };

      allActionSkins.Add(newSkin);
    }
  }

  public ActionSkin GetActionSkinByName(string name) {
    foreach(ActionSkin currentActionSkin in allActionSkins) {
      if(currentActionSkin.name == name) {
        return currentActionSkin;
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
      newStatusCondition.stackable = int.Parse(entry["stackable"]);//  (entry["stackable"] == "TRUE" ? true : false);

      newStatusCondition.sprite = Resources.Load<Sprite>("Icons/" + entry["sprite"]);

      List<StatusConditionEffect> effects = new List<StatusConditionEffect>();
      List<float> effectsPower = new List<float>();
      Debug.LogWarning("Status Condition: " + newStatusCondition.name);
      for(int i=1; i<=3; i++) {
        if(!string.IsNullOrEmpty(entry["effect "+i])) {
          effects.Add(GetStatusConditionEffectByString(entry["effect " + i]));
          if(!string.IsNullOrEmpty(entry["effect " + i + " power"])) {
            effectsPower.Add(float.Parse(entry["effect " + i + " power"]));
          } else {
            effectsPower.Add(1f);
          }
        }
      }
      newStatusCondition.statusEffect = effects.ToArray();
      newStatusCondition.statusEffectPower = effectsPower.ToArray();

      allStatusConditions.Add(newStatusCondition);
    }
  }

  public StatusConditionEffect GetStatusConditionEffectByString(string effect) {
    Debug.LogWarning("effect: " + effect);
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
