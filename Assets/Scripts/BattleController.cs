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

  [Header("- Enemies -")]
  public List<Enemy> allEnemies;
  public TextAsset enemiesFile;

  [Header("- Skills -")]
  public List<Skill> allSkills;
  public TextAsset skillsFile;

  public List<ActionSkin> playerActionSkins;
  public TextAsset actionSkinsFile;

  [Header("- Status Conditions -")]
  public List<StatusCondition> allStatusConditions;
  public TextAsset statusConditionsFile;

  [Header("- HP -")]
  public int maxHp = 10;
  public int hp = 10;

  [Header("- Battle Members -")]
  public Person[] partyMembers;
  public Enemy[] enemyMembers;

  [Header("- Player Actions -")]
  public TurnActionType[] selectedActionType;
  public Skill[] selectedSkills;
  public Item[] selectedItems;
  public SkillTarget[] selectedTargetPartyId;
  public int dateLength;

  public DateLocation currentDateLocation;

  public GameObject defaultEnemyPrefab;

  public GameObject damageParticlePrefab;
  public GameObject itemParticlePrefab;
  public GameObject defenseActionParticlePrefab;
  public GameObject defendHitParticlePrefab;
  public GameObject detectParticlePrefab;

  const float attackAnimationTime = 0.15f;

  public Color greenColor;
  public Color redColor;

  public float damageShineTime;
  public float damageShineAlpha;

  public Inventory usedCelestialItems;


  public void Awake() {
    instance = this;
    partyMembers = new Person[0];
    selectedTargetPartyId = new SkillTarget[0];
    LoadAllEnemies();
    LoadAllSkills();
    LoadAllActionSkins();
    LoadAllStatusConditions();
    dateLength = 3;
  }


  public void SetupBattleStart() {
    Combat combat = GameController.instance.combatHappening;
    List<Person> heroesParty = new List<Person>();
    List<Enemy> enemiesParty = new List<Enemy>();
    string enemyName = "fly";

    foreach(Character c in combat.characters) {
      if(c.id <= CharacterId.maya) {
        heroesParty.Add(GlobalData.instance.people[(int)c.id]);
      } else {
        enemiesParty.Add( GetEnemyByString(c.id.ToString()) );
        enemyName = c.id.ToString();
      }
    }
    partyMembers = heroesParty.ToArray();

    selectedSkills = new Skill[partyMembers.Length];
    selectedItems = new Item[partyMembers.Length];
    selectedActionType = new TurnActionType[partyMembers.Length];
    for(int i=0; i< selectedActionType.Length; i++) {
      selectedActionType[i] = TurnActionType.idle;
    }
    selectedTargetPartyId = new SkillTarget[partyMembers.Length];
    usedCelestialItems = new Inventory();

    maxHp = GlobalData.instance.GetCurrentRelationship().GetMaxHp();

    FullHealParty();
    FullHealEnemies();

    ClearSkillsUsageRegistry();
    //UIController.instance.ShowPartyPeopleCards();
    UIController.instance.UpdateBattleUI();

    VsnSaveSystem.SetVariable("battle_is_happening", true);

    SetDateEnemiesAndLocation(enemyName);
    VsnSaveSystem.SetVariable("currentDateEvent", 0);
    TheaterController.instance.InitializeEnemyLevelAndHp();
  }

  public bool IsBattleHappening() {
    return VsnSaveSystem.GetBoolVariable("battle_is_happening");
  }
  
  public void EndBattle() {
    VsnSaveSystem.SetVariable("battle_is_happening", false);

    // fully heal the party
    //FullHealParty();

    // remove bg effect
    TheaterController.instance.ApplyBgEffect(BgEffect.pulsingEffect, 0);

    // recover used celestial items
    foreach(ItemListing itemToRecharge in usedCelestialItems.itemListings) {
      Inventory ivt = GlobalData.instance.people[0].inventory;
      ivt.AddItem(itemToRecharge.id, itemToRecharge.amount);
    }

    GameController.instance.EndBattle();
  }


  public void FullHealParty() {
    HealPartyHp(maxHp);
    RemovePartyStatusConditions();
    for(int i=0; i < partyMembers.Length; i++) {
      partyMembers[i].HealSp(partyMembers[i].GetMaxSp(GlobalData.instance.GetCurrentRelationship().id) );
    }
  }

  public void FullHealEnemies() {
    foreach(Enemy currentEnemy in enemyMembers) {
      currentEnemy.hp = currentEnemy.maxHp;
      currentEnemy.RemoveAllStatusConditions();
      currentEnemy.ClearAllSkillsUsage();
    }
  }

  public void ClearSkillsUsageRegistry() {
    foreach(Person p in partyMembers) {
      p.ClearAllSkillsUsage();
    }
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



  public Enemy GetCurrentEnemy() {
    int currentDateEvent = VsnSaveSystem.GetIntVariable("currentDateEvent");
    if(enemyMembers.Length <= currentDateEvent) {
      return null;
    }
    return enemyMembers[currentDateEvent];
  }

  public SkillTarget GetPartyMemberPosition(Battler character) {
    if(partyMembers[0] == character) {
      return SkillTarget.partyMember1;
    } else if(partyMembers.Length >= 2 && partyMembers[1] == character) {
      return SkillTarget.partyMember2;
    } else if(partyMembers.Length >= 3 && partyMembers[2] == character) {
      return SkillTarget.partyMember3;
    } else if(GetCurrentEnemy() == character) {
      return SkillTarget.enemy1;
    }
    return SkillTarget.none;
  }



  public void FinishSelectingCharacterAction() {
    int currentPlayerTurn = CurrentPlayerId();

    switch(selectedActionType[currentPlayerTurn]) {
      case TurnActionType.useItem:
        WaitToSelectTarget(selectedActionType[currentPlayerTurn], selectedItems[currentPlayerTurn].range, currentPlayerTurn);
        return;
      case TurnActionType.useSkill:
        WaitToSelectTarget(selectedActionType[currentPlayerTurn], selectedSkills[currentPlayerTurn].range, currentPlayerTurn);
        return;
      case TurnActionType.idle:
        //RecoverStealth(stealthRecoveredWhenIdle);
        break;
    }

    HideActionButtons();
    TheaterController.instance.SetCharacterChoosingAction(-1);
    VsnController.instance.GotCustomInput();
  }

  public void HideActionButtons() {
    UIController.instance.actionsPanel.EndActionSelect();
    UIController.instance.CleanHelpMessagePanel();
  }

  public void WaitToSelectTarget(TurnActionType actionType, ActionRange range, int currentPlayerTurn) {
    UIController.instance.actionsPanel.EndActionSelect();
    if(actionType == TurnActionType.useItem) {
      UIController.instance.SetHelpMessageText(Lean.Localization.LeanLocalization.GetTranslationText("action/target/item"));
    } else if(actionType == TurnActionType.useSkill) {
      UIController.instance.SetHelpMessageText(Lean.Localization.LeanLocalization.GetTranslationText("action/target/skill"));
    }

    UIController.instance.selectTargetPanel.SetActive(true);
    switch(range) {
      case ActionRange.self:
        SetAllTargetSelections(false);
        UIController.instance.selectTargets[currentPlayerTurn].SetActive(true);
        break;
      case ActionRange.one_ally:
        SetAllTargetSelections(true);
        UIController.instance.selectTargets[2].SetActive(false);
        break;
      case ActionRange.other_ally:
        SetAllTargetSelections(true);
        UIController.instance.selectTargets[2].SetActive(false);
        for(int i = 0; i < 2; i++) {
          if(i == currentPlayerTurn) {
            UIController.instance.selectTargets[i].SetActive(false);
          }
        }
        break;
      case ActionRange.one_enemy:
        SetAllTargetSelections(false);
        UIController.instance.selectTargets[2].SetActive(true);
        break;
      case ActionRange.anyone:
        SetAllTargetSelections(true);
        break;
      case ActionRange.random_enemy:
        UIController.instance.selectTargetPanel.SetActive(false);
        selectedTargetPartyId[currentPlayerTurn] = SkillTarget.enemy1;
        HideActionButtons();
        TheaterController.instance.SetCharacterChoosingAction(-1);
        VsnController.instance.GotCustomInput();
        return;
    }

    for(int i = 0; i < UIController.instance.selectTargets.Length; i++) {
      if(UIController.instance.selectTargets[i].activeSelf) {
        Utils.SelectUiElement(UIController.instance.selectTargets[i]);
        return;
      }
    }
  }

  public void SetAllTargetSelections(bool value) {
    for(int i = 0; i < UIController.instance.selectTargets.Length; i++) {
      UIController.instance.selectTargets[i].SetActive(value);
    }
  }

  public void CharacterTurn(SkillTarget partyMemberId) {
    VsnController.instance.state = ExecutionState.WAITING;

    TurnActionType action = selectedActionType[(int)partyMemberId];
    Battler currentCharacter = GetBattlerByTargetId(partyMemberId);


    switch(action) {
      case TurnActionType.useSkill:
        CharacterUseSkill(partyMemberId);
        break;
      case TurnActionType.useItem:
        StartCoroutine(ExecuteUseItem(partyMemberId, selectedTargetPartyId[(int)partyMemberId], selectedItems[(int)partyMemberId]));
        AchievementsController.StatProgress("ITEMS_USED", 1);
        break;
      case TurnActionType.defend:
        StartCoroutine(ExecuteDefend(partyMemberId));
        break;
      case TurnActionType.idle:
        StartCoroutine(ExecuteIdle(partyMemberId));
        break;
    }
  }

  public void CharacterUseSkill(SkillTarget partyMemberId) {
    // spend SP
    GetBattlerByTargetId(partyMemberId).SpendSp(selectedSkills[(int)partyMemberId].spCost);

    switch(selectedSkills[(int)partyMemberId].type) {
      case SkillType.attack:
      case SkillType.active:
        StartCoroutine(ExecuteBattlerSkill(partyMemberId, selectedTargetPartyId[(int)partyMemberId], selectedSkills[(int)partyMemberId]));
        break;
      case SkillType.passive:
        Debug.LogError("Passive skill " + selectedSkills[(int)partyMemberId].name + " used actively.");
        break;
    }
  }



  public int CalculateAttackDamage(Battler attacker, Skill usedSkill, Battler defender) {
    float damage;

    Debug.Log("used skill: " +usedSkill.damagePower+", attr: "+usedSkill.damageAttribute);
    Debug.Log("attacker: " + attacker.GetName());
    Debug.Log("defender: " + defender.GetName());

    damage = attacker.AttributeValue((int)usedSkill.damageAttribute) * usedSkill.damagePower;
    //damage = (3f*attacker.AttributeValue((int)usedSkill.damageAttribute) / Mathf.Max(2f * defender.AttributeValue((int)Attributes.endurance) + defender.AttributeValue((int)usedSkill.damageAttribute), 1f));

    Debug.LogWarning(attacker.GetName() + " Hits!");

    // skill power
    //damage *= usedSkill.damagePower * Random.Range(0.9f, 1.1f);

    // attacker damage multiplier
    damage *= attacker.DamageMultiplier();

    // defender damage taken multiplier
    damage *= defender.DamageTakenMultiplier(usedSkill.damageAttribute);

    // level damage scaling
    //damage *= LevelModifier(attacker.Level(), defender.Level());

    Debug.LogWarning("LEVEL MODIFIER: " + LevelModifier(attacker.Level(), defender.Level()));

    // defend?
    damage /= (defender.IsDefending() ? 2f : 1f);

    Debug.Log("Final damage: " + damage);

    return Mathf.Max(1, Mathf.RoundToInt(damage));
  }

  public float LevelModifier(int attackerLevel, int defenderLevel) {
    int levelDiff = attackerLevel - defenderLevel;
    levelDiff = Mathf.Min(levelDiff, 4);
    levelDiff = Mathf.Max(levelDiff, -4);

    if(levelDiff >= 0) {
      return 1f + (levelDiff * levelDiff) / 10f;
    } else {
      return 1f - (levelDiff * levelDiff) / 18f;
    }
  }



  public IEnumerator ShowGetStatusConditionMessage(string targetName, string statusConditionName) {
    VsnSaveSystem.SetVariable("target_name", targetName);
    VsnSaveSystem.SetVariable("status_condition", statusConditionName);

    string code = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("status_condition/receive"));
    yield return new WaitForSeconds(0.5f);
  }

  public void ShowTakeStatusConditionDescription(string targetName, StatusConditionEffect statusEffect) {    
    VsnSaveSystem.SetVariable("target_name", targetName);
    string code = SpecialCodes.InterpretStrings(Lean.Localization.LeanLocalization.GetTranslationText("status_condition/effect/"+statusEffect.ToString()));
    StartCoroutine(ShowStatusConditionDamage(code));
  }

  public IEnumerator ShowStatusConditionDamage(string code) {
    VsnController.instance.state = ExecutionState.WAITING;

    yield return new WaitForSeconds(1f);

    UIController.instance.CleanHelpMessagePanel();
    VsnController.instance.state = ExecutionState.PLAYING;
  }



  public ActionSkin GetActionSkin(Person user, Skill usedSkill) {
    string sexModifier = (user.isMale ? "_boy" : "_girl");
    string actionSkinName = SpecialCodes.InterpretStrings("\\vsn(" + usedSkill.damageAttribute.ToString() + "_action" + sexModifier + "_name)");
    return GetActionSkinByName(actionSkinName);
  }


  public IEnumerator ExecuteBattlerSkill(SkillTarget skillUserId, SkillTarget targetId, Skill usedSkill) {
    Actor2D skillUserActor = TheaterController.instance.GetActorByIdInParty(skillUserId);
    Battler skillUser = skillUserActor.battler;
    Actor2D[] targetActors;

    if(targetId == SkillTarget.allEnemies) {
      targetActors = TheaterController.instance.GetAllEnemiesActors();
    } else if(targetId == SkillTarget.allHeroes) {
      targetActors = TheaterController.instance.GetAllHeroesActors();
    } else {
      targetActors = new Actor2D[] { TheaterController.instance.GetActorByIdInParty(targetId) };
    }



    /// register skill usage
    skillUser.RegisterUsedSkill(usedSkill.id);

    if(usedSkill.type == SkillType.attack) {
      string attackName = "pre_attack";

      switch(skillUserId) {
        case SkillTarget.enemy1:
        case SkillTarget.enemy2:
        case SkillTarget.enemy3:
          if(((Person)targetActors[0].battler).isMale) {
            attackName = "enemy_attacks_boy";
          } else {
            attackName = "enemy_attacks_girl";
          }
          break;
        case SkillTarget.partyMember1:
          break;
      }
    }

    // focus actors
    if(targetId != SkillTarget.allEnemies && targetId != SkillTarget.allHeroes) {
      TheaterController.instance.FocusActors(new Actor2D[] { skillUserActor, targetActors[0] });
      yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);
    }


    // skill cast animation
    switch(usedSkill.animationSkin.animation) {
      case SkillAnimation.skinnable:
        ActionSkin actionSkin;
        if(skillUserId == SkillTarget.partyMember1 || skillUserId == SkillTarget.partyMember2) {
          actionSkin = GetActionSkin(partyMembers[(int)skillUserId], usedSkill);
        } else {
          actionSkin = skillUserActor.enemy.baseAttackSkin;
        }
        yield return skillUserActor.CharacterAttackAnim(actionSkin, targetActors);
        break;

      default:
        yield return skillUserActor.CharacterAttackAnim(usedSkill.animationSkin, targetActors);
        break;

      case SkillAnimation.none:
      case SkillAnimation.passive:
        /// show nothing
        break;
    }


    // skill effects for every target
    foreach(Actor2D targetActor in targetActors) {
      // skill receive animation
      if(usedSkill.animationSkin.animation != SkillAnimation.none &&
         usedSkill.animationSkin.animation != SkillAnimation.passive) {
        if(targetActor.enemy != null && targetActor.enemy.HasTag("ally")) {
          // special animation for hitting allies
          targetActor.ShineGreen();
        } else {
          switch(usedSkill.animationSkin.animation) {
            case SkillAnimation.active_support:
            case SkillAnimation.long_charge:
              targetActor.ShineGreen();
              break;
            case SkillAnimation.attack:
            case SkillAnimation.charged_attack:
            case SkillAnimation.active_offensive:
            case SkillAnimation.run_over:
            case SkillAnimation.throw_object:
            case SkillAnimation.multi_throw:
            default:
              targetActor.ShineRed();
              break;
          }
        }
      }


      // cause damage
      if(usedSkill.damagePower != 0) {
        int effectiveAttackDamage = CalculateAttackDamage(skillUser, usedSkill, targetActor.battler);

        float effectivity = targetActor.battler.DamageTakenMultiplier(usedSkill.damageAttribute);

        if(targetActor.enemy != null && targetActor.enemy.HasTag("ally")) {
          VsnAudioManager.instance.PlaySfx("heal_default");
        }

        else if(targetActor.battler.IsDefending()) {
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


        if(targetActor.enemy == null || !targetActor.enemy.HasTag("ally")) {
          targetActor.ShowDamageParticle(effectiveAttackDamage, effectivity);
        } else {
          targetActor.ShowDamageParticle(effectiveAttackDamage, -1f);
        }
        //yield return new WaitForSeconds(1f);

        targetActor.battler.TakeDamage(effectiveAttackDamage);
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
          VsnSaveSystem.SetVariable("enemyAttackTargetId", (int)skillUserId);
          break;
        case SkillSpecialEffect.divertEnemyTarget:
          VsnSaveSystem.SetVariable("enemyAttackTargetId", (int)OtherPartyMemberId(skillUserId));
          break;
        case SkillSpecialEffect.reflectEnemyTarget:
          selectedTargetPartyId[CurrentPlayerId()] = (SkillTarget)CurrentPlayerId();
          break;
      }


      // heal status condition
      if(usedSkill.healsConditionNames.Length > 0) {
        targetActor.battler.RemoveStatusConditionBySkill(usedSkill);
      }


      // heal HP and SP
      if(usedSkill.healHp > 0 || usedSkill.healHpPercent > 0f || usedSkill.healSp > 0) {
        VsnAudioManager.instance.PlaySfx("heal_default");
        int effectiveHeal;

        if(usedSkill.healHp > 0) {
          effectiveHeal = (int)(usedSkill.healHp * skillUser.HealingSkillMultiplier());
          targetActor.battler.HealHP(effectiveHeal);
          targetActor.ShowHealHpParticle(effectiveHeal);
          yield return new WaitForSeconds(1f);
        }
        if(usedSkill.healHpPercent > 0) {
          effectiveHeal = (int)(usedSkill.healHpPercent * skillUser.HealingSkillMultiplier() * targetActor.battler.MaxHP());
          targetActor.battler.HealHP(effectiveHeal);
          targetActor.ShowHealHpParticle(effectiveHeal);
          yield return new WaitForSeconds(1f);
        }
        if(usedSkill.healSp > 0) {
          targetActor.battler.HealSp(usedSkill.healSp);
          targetActor.ShowHealSpParticle(usedSkill.healSp);
          yield return new WaitForSeconds(1f);
        }
      }


      // chance to give status condition
      for(int i = 0; i < usedSkill.givesConditionNames.Length; i++) {
        int effectiveStatusConditionChance = usedSkill.giveStatusChance;
        string[] statusConditionOptions = usedSkill.givesConditionNames[i].Split('|');
        string selectedStatusCondition = statusConditionOptions[Random.Range(0, statusConditionOptions.Length)];

        if(targetActor.battler.IsDefending() && targetActor.battler.FightingSide() != skillUser.FightingSide()) {
          effectiveStatusConditionChance -= Mathf.Min(effectiveStatusConditionChance / 2, 30);
        }
        effectiveStatusConditionChance -= targetActor.battler.StatusResistance(selectedStatusCondition);

        Debug.LogWarning("effective Status Condition Chance: " + effectiveStatusConditionChance);

        if(effectiveStatusConditionChance <= 0) {
          targetActor.ShowImmuneConditionParticle();
          yield return new WaitForSeconds(1f);
        } else if(Random.Range(0, 100) < effectiveStatusConditionChance) {
          VsnAudioManager.instance.PlaySfx("skill_activate_bad");

          targetActor.battler.ReceiveStatusConditionBySkill(usedSkill, selectedStatusCondition);
          StatusCondition statusCondition = GetStatusConditionByName(selectedStatusCondition);
          yield return ShowGetStatusConditionMessage(targetActor.battler.GetName(), statusCondition.GetPrintableName());
        } else {
          targetActor.ShowResistConditionParticle();
          yield return new WaitForSeconds(1f);
        }
      }
    }


    // wait then end
    yield return new WaitForSeconds(0.5f);


    TheaterController.instance.UnfocusActors();
    UIController.instance.CleanHelpMessagePanel();
    yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);

    VsnController.instance.state = ExecutionState.PLAYING;
  }

  public SkillTarget OtherPartyMemberId(SkillTarget id) {
    switch(id) {
      case SkillTarget.partyMember1:
        return SkillTarget.partyMember2;
      case SkillTarget.partyMember2:
        return SkillTarget.partyMember1;
    }
    return SkillTarget.partyMember1;
  }


  public IEnumerator ExecuteUseItem(SkillTarget itemUserId, SkillTarget targetId, Item usedItem) {
    Actor2D userActor = TheaterController.instance.GetActorByIdInParty(itemUserId);
    Actor2D targetActor = TheaterController.instance.GetActorByIdInParty(targetId);
    Battler target = targetActor.battler;

    //yield return ShowUseItemMessage(userActor.battler.GetName(), usedItem.GetPrintableName());

    TheaterController.instance.FocusActors(new Actor2D[] { userActor, targetActor });
    yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);

    Enemy currentEvent = GetCurrentEnemy();

    VsnAudioManager.instance.PlaySfx("challenge_default");
    yield return userActor.UseItemAnimation(targetActor, usedItem);

    // spend item
    Inventory ivt = GlobalData.instance.people[0].inventory;
    if(usedItem.HasTag("celestial")) {
      usedCelestialItems.AddItem(usedItem.id, 1);
    }
    ivt.ConsumeItem(usedItem.id, 1);


    // sensor effect
    if(usedItem.nameKey == "sensor") {
      VsnAudioManager.instance.PlaySfx("skill_activate_good");
      targetActor.ShineRed();
      targetActor.ShowWeaknessCard(true);
      yield return new WaitForSeconds(1f);
    } else {
      VsnAudioManager.instance.PlaySfx("item_use");
      targetActor.ShineGreen();
    }

    // heal status condition
    if(usedItem.HealsStatusCondition()) {
      foreach(string condName in usedItem.healsConditionNames) {
        target.RemoveStatusCondition(condName);
      }
    }

    // heal HP and SP
    if(usedItem.healHp > 0) {
      int healingpower = (int)(usedItem.healHp * GlobalData.instance.GetCurrentRelationship().HealingItemMultiplier());
      target.HealHP(healingpower);
      targetActor.ShowHealHpParticle(healingpower);
      yield return new WaitForSeconds(1f);
    }
    if(usedItem.healSp > 0) {
      target.HealSp(usedItem.healSp);
      targetActor.ShowHealSpParticle(usedItem.healSp);
      yield return new WaitForSeconds(1f);
    }

    // give status condition
    if(usedItem.GivesStatusCondition()) {
      target.ReceiveStatusConditionByItem(usedItem);
      bool receivedNewStatus = true;

      if(receivedNewStatus) {
        StatusCondition statusCondition = GetStatusConditionByName(usedItem.givesConditionNames[0]);
        yield return ShowGetStatusConditionMessage(targetActor.battler.GetName(), statusCondition.GetPrintableName());
      }
    }
    yield return new WaitForSeconds(0.5f);

    TheaterController.instance.UnfocusActors();
    UIController.instance.CleanHelpMessagePanel();
    yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);

    // spend stealth bar
    //RemoveStealth(stealthLostWhenUsedItem);
    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public IEnumerator ExecuteDefend(SkillTarget partyMemberId) {
    Actor2D defendingActor = TheaterController.instance.GetActorByIdInParty(partyMemberId);
    Battler defender = defendingActor.battler;

    defendingActor.DefendActionAnimation();
    //yield return ShowDefendMessage();

    if(defender.CurrentSP() < defender.MaxSP()) {
      yield return new WaitForSeconds(0.2f);
      defender.HealSp(1);
      defendingActor.ShowHealSpParticle(1);
      yield return new WaitForSeconds(0.5f);
    } else {
      yield return new WaitForSeconds(1.5f);
    }

    UIController.instance.CleanHelpMessagePanel();
    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public IEnumerator ExecuteIdle(SkillTarget partyMemberId) {
    Actor2D idleActor = TheaterController.instance.GetActorByIdInParty(partyMemberId);
    Battler defender = idleActor.battler;
    VsnSaveSystem.SetVariable("target_name", idleActor.battler.GetName());

    //yield return ShowDistractedMessage();
    idleActor.DistractedAnimation();

    yield return new WaitForSeconds(0.5f);

    UIController.instance.CleanHelpMessagePanel();
    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public void EnemyAttack() {
    SkillTarget targetId = (SkillTarget)VsnSaveSystem.GetIntVariable("enemyAttackTargetId");
    VsnController.instance.state = ExecutionState.WAITING;

    Skill skillUsed = GetCurrentEnemy().DecideWhichSkillToUse();

    if(skillUsed.range == ActionRange.all_allies) {
      targetId = SkillTarget.allEnemies;
    } else if(skillUsed.range == ActionRange.all_enemies) {
      targetId = SkillTarget.allHeroes;
    } else if(skillUsed.range == ActionRange.self || skillUsed.range == ActionRange.one_ally) {
      targetId = SkillTarget.enemy1;
    } else {
      // change target if the other party member is using Guardian
      if(partyMembers.Length > 1) {
        if(!partyMembers[(int)targetId].IsPreferredTarget() && partyMembers[(int)OtherPartyMemberId(targetId)].IsPreferredTarget()) {
          targetId = OtherPartyMemberId(targetId);
        }
      }
    }

    StartCoroutine(ExecuteBattlerSkill(SkillTarget.enemy1, targetId, skillUsed));
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

  public void SetDateEnemiesAndLocation(string enemyName) {
    currentDateLocation = DateLocation.desert;
    dateLength = 1;
    enemyMembers = new Enemy[] { GetEnemyByString(enemyName) };
  }

  public void SetCustomBattle(int enemyId) {
    currentDateLocation = DateLocation.desert;
    dateLength = 1;
    enemyMembers = new Enemy[] {allEnemies[enemyId]};
    FullHealParty();
  }

  public void SetupDateLocation(){
    TheaterController.instance.SetLocation(currentDateLocation.ToString());
  }

  public Enemy GetEnemyByString(string nameKey) {
    foreach(Enemy enemy in allEnemies) {
      if(enemy.nameKey == nameKey) {
        return enemy;
      }
    }
    return null;
  }

  public int CurrentPlayerId() {
    return VsnSaveSystem.GetIntVariable("currentPlayerTurn");
  }

  public Person GetCurrentPlayer() {
    int currentPlayer = CurrentPlayerId();
    if(currentPlayer < partyMembers.Length && currentPlayer >= 0) {
      return partyMembers[currentPlayer];
    }
    return null;
  }

  public Person GetCurrentTarget() {
    int currentPlayer = CurrentPlayerId();
    if(currentPlayer < partyMembers.Length) {
      int target = (int)selectedTargetPartyId[currentPlayer];
      if(target < partyMembers.Length) {
        return partyMembers[target];
      }
    }
    return null;
  }

  public Battler GetBattlerByTargetId(SkillTarget targetId) {
    switch(targetId) {
      case SkillTarget.partyMember1:
        return partyMembers[0];
      case SkillTarget.partyMember2:
        return partyMembers[1];
      case SkillTarget.partyMember3:
        return partyMembers[2];
      case SkillTarget.enemy1:
      default:
        return GetCurrentEnemy();
    }
    return null;
  }

  public Battler GetBattlerByString(string targetName) {
    switch(targetName) {
      case "main":
        return GetBattlerByTargetId(SkillTarget.partyMember1);
      case "support":
        return GetBattlerByTargetId(SkillTarget.partyMember2);
      case "angel":
        return GetBattlerByTargetId(SkillTarget.partyMember3);
      case "enemy":
        return GetBattlerByTargetId(SkillTarget.enemy1);
    }
    return null;
  }


  public void RecoverEnemiesHp() {
    for(int i = 0; i < dateLength; i++) {
      enemyMembers[i].hp = enemyMembers[i].maxHp;
    }
  }



  public void LoadAllEnemies() {
    allEnemies = new List<Enemy>();

    string[] loadedTags, loadedImmunities;
    ActionSkin actionSkin;

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(enemiesFile, 1);
    foreach(Dictionary<string, string> dic in spreadsheetData.data) {
      Debug.LogWarning("Loading enemy: " + dic["name key"]);

      Enemy loadedEnemy = JsonUtility.FromJson<Enemy>("{\"activeSkillLogics\" :" + dic["active skills logic"] + "}");
      loadedTags = Utils.SeparateTags(dic["tags"]);
      loadedImmunities = Utils.SeparateTags(dic["status immunities"]);


      actionSkin = new ActionSkin();
      actionSkin.sfxName = dic["base attack sfx"];
      actionSkin.animation = GetSkillAnimationByString(dic["base attack animation"]);
      actionSkin.animationArgument = Utils.GetStringArgument(dic["base attack animation"]);

      loadedEnemy.id = int.Parse(dic["id"]);
      loadedEnemy.nameKey = dic["name key"];
      loadedEnemy.level = int.Parse(dic["level"]);
      loadedEnemy.maxHp = int.Parse(dic["maxHp"]);
      loadedEnemy.spriteName = dic["sprite name"];
      loadedEnemy.baseAttackSkin = actionSkin;
      loadedEnemy.appearSfxName = dic["appear sfx"];
      loadedEnemy.expReward = int.Parse(dic["exp reward"]);
      loadedEnemy.moneyReward = int.Parse(dic["money reward"]);
      loadedEnemy.attributes = new int[]{
        int.Parse(dic["maxHp"]),
        int.Parse(dic["movementRange"]),
        int.Parse(dic["attack"]),
        int.Parse(dic["agility"]),
        int.Parse(dic["dodgeRate"])};
      loadedEnemy.passiveSkills = Utils.SeparateInts(dic["passive skills"]);
      loadedEnemy.statusImmunities = loadedImmunities;
      loadedEnemy.tags = loadedTags;

      allEnemies.Add(loadedEnemy);
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
      //Debug.LogWarning("Importing skill: " + entry["name"]);

      Skill newSkill = new Skill();

      newSkill.id = int.Parse(entry["id"]);

      newSkill.name = entry["name"];
      newSkill.type = GetSkillTypeByString(entry["type"]);
      newSkill.range = GetActionRangeByString(entry["range"]);

      if(!string.IsNullOrEmpty(entry["sp cost"])) {
        newSkill.spCost = int.Parse(entry["sp cost"]);
      }

      newSkill.specialEffect = GetSkillEffectByString(entry["special effect"]);
      if(!string.IsNullOrEmpty(entry["effect power"])) {
        newSkill.effectPower = float.Parse(entry["effect power"]);
      }

      newSkill.damageAttribute = (Attributes)System.Enum.Parse(typeof(Attributes), entry["damage attribute"]);
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

      if(newSkill.id < 3) {
        newSkill.sprite = null;
      } else {
        newSkill.sprite = Resources.Load<Sprite>("Icons/" + entry["sprite"]);
      }
      newSkill.animationSkin = new ActionSkin();
      newSkill.animationSkin.animation = GetSkillAnimationByString(entry["animation"]);
      newSkill.animationSkin.animationArgument = Utils.GetStringArgument(entry["animation"]);
      newSkill.animationSkin.sfxName = entry["animation sfx"];

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

  public static ActionRange GetActionRangeByString(string actionRange) {
    for(int i = 0; i <= (int)ActionRange.none; i++) {
      if(((ActionRange)i).ToString() == actionRange) {
        return (ActionRange)i;
      }
    }
    return ActionRange.anyone;
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

  public SkillTarget GetSkillTargetByString(string targetName) {
    for(int i = 0; i <= (int)SkillTarget.none; i++) {
      if(((SkillTarget)i).ToString() == targetName) {
        return (SkillTarget)i;
      }
    }
    return SkillTarget.none;
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
      if(skillAnimation.StartsWith( ((SkillAnimation)i).ToString() ) ) {
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

    playerActionSkins = new List<ActionSkin>();
    foreach(Dictionary<string, string> entry in data.data) {
      ActionSkin newSkin = new ActionSkin() {
        name = entry["name"],
        sfxName = entry["sfx"],
        animation = GetSkillAnimationByString(entry["animation"]),
        animationArgument = Utils.GetStringArgument(entry["animation"])
    };

      playerActionSkins.Add(newSkin);
    }
  }

  public ActionSkin GetActionSkinByName(string name) {
    foreach(ActionSkin currentActionSkin in playerActionSkins) {
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
      newStatusCondition.stackable = int.Parse(entry["stackable"]);

      newStatusCondition.sprite = Resources.Load<Sprite>("Icons/" + entry["sprite"]);

      List<StatusConditionEffect> effects = new List<StatusConditionEffect>();
      List<float> effectsPower = new List<float>();
      //Debug.LogWarning("Status Condition: " + newStatusCondition.name);
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
    //Debug.LogWarning("effect: " + effect);
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
