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


public enum DateLocation {
  desert,
  generic
}


public class BattleController : MonoBehaviour {
  public static BattleController instance;

  [Header("- Battle Members -")]
  public Pilot[] partyMembers;
  public Enemy[] enemyMembers;

  [Header("- Player Actions -")]
  public TurnActionType[] selectedActionType;
  public Skill[] selectedSkills;
  public Item[] selectedItems;
  public SkillTarget[] selectedTargetPartyId;

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

  [Header("- Date Location -")]
  public DateLocation currentDateLocation;

  [Header("- Actor Prefabs -")]
  public GameObject defaultEnemyPrefab;

  [Header("- Particle Prefabs -")]
  public GameObject damageParticlePrefab;
  public GameObject itemParticlePrefab;
  public GameObject defenseActionParticlePrefab;
  public GameObject defendHitParticlePrefab;

  [Header("- Attack Animation -")]
  public Color greenColor;
  public Color redColor;
  public float damageShineTime;
  public float damageShineAlpha;


  public int CurrentBattlerId {
    get{ return VsnSaveSystem.GetIntVariable("currentBattlerTurn"); }
  }

  public Battler CurrentBattler {
    get { return GameController.instance.currentCombat.characters[CurrentBattlerId].battler; }
  }

  public Actor2D CurrentBattlerActor {
    get { return TheaterController.instance.GetActorByBattler(CurrentBattler); }
  }

  public SkillTarget CurrentPartyPos {
    get {
      for(int i=0; i<partyMembers.Length; i++) {
        if(partyMembers[i] == CurrentBattler) {
          return SkillTarget.partyMember1+i;
        }
      }
      for(int i = 0; i < enemyMembers.Length; i++) {
        if(enemyMembers[i] == CurrentBattler) {
          return SkillTarget.enemy1 + i;
        }
      }
      return SkillTarget.none;
    }
  }


  public void Awake() {
    instance = this;
    partyMembers = new Pilot[0];
    selectedTargetPartyId = new SkillTarget[0];
    LoadAllEnemies();
    LoadAllSkills();
    LoadAllActionSkins();
    LoadAllStatusConditions();
  }


  public void SetupBattleStart() {
    Combat combat = GameController.instance.currentCombat;
    List<Pilot> heroesParty = new List<Pilot>();
    List<Enemy> enemiesParty = new List<Enemy>();

    foreach(CharacterToken characterToken in combat.characters) {
      if(characterToken.id <= CharacterId.maya) {
        heroesParty.Add((Pilot)characterToken.battler);
      } else {
        enemiesParty.Add((Enemy)characterToken.battler);
      }
    }
    partyMembers = heroesParty.ToArray();
    enemyMembers = enemiesParty.ToArray();

    /// set location
    currentDateLocation = DateLocation.desert;

    selectedActionType = new TurnActionType[GameController.instance.currentCombat.characters.Count];
    selectedSkills = new Skill[GameController.instance.currentCombat.characters.Count];
    selectedItems = new Item[GameController.instance.currentCombat.characters.Count];
    for(int i=0; i< selectedActionType.Length; i++) {
      selectedActionType[i] = TurnActionType.idle;
    }
    selectedTargetPartyId = new SkillTarget[GameController.instance.currentCombat.characters.Count];

    ClearSkillsUsageRegistry();

    VsnSaveSystem.SetVariable("battle_is_happening", true);

    /// Show Battle UI, with HP set instantaneously
    UIController.instance.UpdateBattleUI();
    UIController.instance.SkipHpBarAnimations();
  }

  public bool IsBattleHappening() {
    return VsnSaveSystem.GetBoolVariable("battle_is_happening");
  }


  public void FullHealParty() {
    for(int i=0; i < partyMembers.Length; i++) {
      partyMembers[i].RemoveAllStatusConditions();
      partyMembers[i].HealHP(partyMembers[i].AttributeValue(Attributes.maxHp));
      partyMembers[i].HealSp(partyMembers[i].GetMaxSp());
    }
  }

  public void ClearSkillsUsageRegistry() {
    foreach(Pilot p in partyMembers) {
      p.ClearAllSkillsUsage();
    }
  }



  public SkillTarget GetPartyMemberPosition(Battler character) {
    if(partyMembers[0] == character) {
      return SkillTarget.partyMember1;
    } else if(partyMembers.Length >= 2 && partyMembers[1] == character) {
      return SkillTarget.partyMember2;
    } else if(partyMembers.Length >= 3 && partyMembers[2] == character) {
      return SkillTarget.partyMember3;
    } else if(enemyMembers[0] == character) {
      return SkillTarget.enemy1;
    } else if(enemyMembers.Length >= 2 && enemyMembers[1] == character) {
      return SkillTarget.enemy2;
    } else if(enemyMembers.Length >= 3 && enemyMembers[2] == character) {
      return SkillTarget.enemy3;
    }
    return SkillTarget.none;
  }



  public void FinishSelectingCharacterAction() {
    switch(selectedActionType[CurrentBattlerId]) {
      case TurnActionType.useItem:
        WaitToSelectTarget(selectedItems[CurrentBattlerId].range, CurrentBattlerId);
        return;
      case TurnActionType.useSkill:
        WaitToSelectTarget(selectedSkills[CurrentBattlerId].range, CurrentBattlerId);
        return;
    }

    HideActionButtons();
    TheaterController.instance.SetCharacterChoosingAction(-1);
    VsnController.instance.GotCustomInput();
  }

  public void HideActionButtons() {
    UIController.instance.actionsPanel.EndActionSelect();
  }

  public void WaitToSelectTarget(ActionRange range, int currentBattler) {
    UIController.instance.actionsPanel.EndActionSelect();
    
    UIController.instance.selectTargetPanel.SetActive(true);
    switch(range) {
      case ActionRange.self:
        SetHeroTargetSelections(false);
        SetEnemyTargetSelections(false);
        UIController.instance.selectTargets[currentBattler].SetActive(true);
        break;
      case ActionRange.one_ally:
        SetHeroTargetSelections(true);
        SetEnemyTargetSelections(false);
        break;
      case ActionRange.other_ally:
        SetHeroTargetSelections(true);
        SetEnemyTargetSelections(false);
        UIController.instance.selectTargets[currentBattler].SetActive(false);
        break;
      case ActionRange.one_enemy:
        SetHeroTargetSelections(false);
        SetEnemyTargetSelections(true);
        break;
      case ActionRange.anyone:
        SetHeroTargetSelections(true);
        SetEnemyTargetSelections(true);
        break;
      case ActionRange.random_enemy:
        UIController.instance.selectTargetPanel.SetActive(false);
        selectedTargetPartyId[currentBattler] = SkillTarget.enemy1;
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

  public void SetHeroTargetSelections(bool value) {
    for(int i = 0; i < 3; i++) {
      if(partyMembers.Length > i) {
        UIController.instance.selectTargets[i].SetActive(value);
      } else {
        UIController.instance.selectTargets[i].SetActive(false);
      }      
    }
  }
  public void SetEnemyTargetSelections(bool value) {
    for(int i = 3; i < 6; i++) {
      if(enemyMembers.Length > (i-3)) {
        UIController.instance.selectTargets[i].SetActive(value);
      } else {
        UIController.instance.selectTargets[i].SetActive(false);
      }
    }
  }



  public void CharacterTurn() {
    VsnController.instance.state = ExecutionState.WAITING;
    TurnActionType action = selectedActionType[CurrentBattlerId];

    switch(action) {
      case TurnActionType.useSkill:
        CharacterUseSkill();
        break;
      case TurnActionType.useItem:
        StartCoroutine(ExecuteUseItem((SkillTarget)CurrentBattlerId, selectedTargetPartyId[CurrentBattlerId], selectedItems[CurrentBattlerId]));
        AchievementsController.StatProgress("ITEMS_USED", 1);
        break;
      case TurnActionType.defend:
        StartCoroutine(ExecuteDefend((SkillTarget)CurrentBattlerId));
        break;
      case TurnActionType.idle:
        StartCoroutine(ExecuteIdle((SkillTarget)CurrentBattlerId));
        break;
    }
  }

  public void CharacterUseSkill() {
    Skill usedSkill = selectedSkills[CurrentBattlerId];
    // spend SP
    CurrentBattler.SpendSp(usedSkill.spCost);

    switch(usedSkill.type) {
      case SkillType.attack:
      case SkillType.active:
        StartCoroutine(ExecuteBattlerSkill(selectedTargetPartyId[CurrentBattlerId], usedSkill));
        break;
      case SkillType.passive:
        Debug.LogError("Passive skill " + selectedSkills[CurrentBattlerId].name + " used actively.");
        break;
    }
  }



  public int CalculateAttackDamage(Battler attacker, Skill usedSkill, Battler defender) {
    float damage;

    Debug.Log("used skill: " +usedSkill.damagePower+", attr: "+usedSkill.damageAttribute);
    Debug.Log("attacker: " + attacker.GetName());
    Debug.Log("defender: " + defender.GetName());

    damage = attacker.AttributeValue(usedSkill.damageAttribute) * usedSkill.damagePower;
    //damage = (3f*attacker.AttributeValue((int)usedSkill.damageAttribute) / Mathf.Max(2f * defender.AttributeValue((int)Attributes.endurance) + defender.AttributeValue((int)usedSkill.damageAttribute), 1f));

    Debug.LogWarning(attacker.GetName() + " Hits!");

    // skill power
    //damage *= usedSkill.damagePower * Random.Range(0.9f, 1.1f);

    // attacker damage multiplier
    damage *= attacker.DamageMultiplier();

    // defender damage taken multiplier
    damage *= defender.DamageTakenMultiplier(usedSkill.damageAttribute);

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

    VsnController.instance.state = ExecutionState.PLAYING;
  }



  public ActionSkin GetActionSkin(Pilot user, Skill usedSkill) {
    string sexModifier = (user.isMale ? "_boy" : "_girl");
    string actionSkinName = SpecialCodes.InterpretStrings("\\vsn(" + usedSkill.damageAttribute.ToString() + "_action" + sexModifier + "_name)");
    Debug.LogWarning("actionSkinName: "+ actionSkinName);
    return GetActionSkinByName(actionSkinName);
  }


  public IEnumerator ExecuteBattlerSkill(SkillTarget targetId, Skill usedSkill) {
    Battler skillUser = CurrentBattler;
    Actor2D skillUserActor = TheaterController.instance.GetActorByBattler(skillUser);
    Actor2D[] targetActors;

    if(targetId == SkillTarget.allEnemies) {
      targetActors = TheaterController.instance.GetAllEnemiesActors();
    } else if(targetId == SkillTarget.allHeroes) {
      targetActors = TheaterController.instance.GetAllHeroesActors();
    } else {
      targetActors = new Actor2D[] { TheaterController.instance.GetActorByPartyId(targetId) };
    }


    /// register skill usage
    skillUser.RegisterUsedSkill(usedSkill.id);


    // focus actors
    if(targetId <= SkillTarget.allEnemies) {
      TheaterController.instance.FocusActors(new Actor2D[] { skillUserActor, targetActors[0] });
      yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);
    }


    // skill cast animation
    switch(usedSkill.animationSkin.animation) {
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
        switch(usedSkill.animationSkin.animation) {
          case SkillAnimation.active_support:
          case SkillAnimation.long_charge:
            targetActor.ShineGreen();
            break;
          case SkillAnimation.attack:
          case SkillAnimation.charged_attack:
          case SkillAnimation.active_offensive:
          case SkillAnimation.throw_object:
          case SkillAnimation.multi_throw:
          default:
            targetActor.ShineRed();
            break;
        }
      }


      // cause damage
      if(usedSkill.damagePower != 0) {
        int effectiveAttackDamage = CalculateAttackDamage(skillUser, usedSkill, targetActor.battler);

        float effectivity = targetActor.battler.DamageTakenMultiplier(usedSkill.damageAttribute);

        if(targetActor.battler.IsDefending()) {
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
        //yield return new WaitForSeconds(1f);

        targetActor.battler.TakeDamage(effectiveAttackDamage);
        yield return new WaitForSeconds(1f);
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
    Actor2D userActor = TheaterController.instance.GetActorByPartyId(itemUserId);
    Actor2D targetActor = TheaterController.instance.GetActorByPartyId(targetId);
    Battler target = targetActor.battler;

    TheaterController.instance.FocusActors(new Actor2D[] { userActor, targetActor });
    yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);

    VsnAudioManager.instance.PlaySfx("challenge_default");
    yield return userActor.UseItemAnimation(targetActor, usedItem);

    // spend item
    Inventory ivt = GlobalData.instance.pilots[0].inventory;
    ivt.ConsumeItem(usedItem.id, 1);

    // sound effect and shine
    VsnAudioManager.instance.PlaySfx("item_use");
    targetActor.ShineGreen();

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
    yield return new WaitForSeconds(TheaterController.instance.focusAnimationDuration);

    // spend stealth bar
    //RemoveStealth(stealthLostWhenUsedItem);
    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public IEnumerator ExecuteDefend(SkillTarget partyMemberId) {
    Actor2D defendingActor = TheaterController.instance.GetActorByPartyId(partyMemberId);
    Battler defender = defendingActor.battler;

    defendingActor.DefendActionAnimation();
    //yield return ShowDefendMessage();

    yield return new WaitForSeconds(1.5f);
    //if(defender.CurrentSP() < defender.MaxSP()) {
    //  yield return new WaitForSeconds(0.2f);
    //  defender.HealSp(1);
    //  defendingActor.ShowHealSpParticle(1);
    //  yield return new WaitForSeconds(0.5f);
    //} else {
    //  yield return new WaitForSeconds(1.5f);
    //}

    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public IEnumerator ExecuteIdle(SkillTarget partyMemberId) {
    Actor2D idleActor = TheaterController.instance.GetActorByPartyId(partyMemberId);
    Battler defender = idleActor.battler;
    VsnSaveSystem.SetVariable("target_name", idleActor.battler.GetName());

    yield return new WaitForSeconds(0.5f);

    VsnController.instance.state = ExecutionState.PLAYING;
  }


  public void SelectEnemyTarget() {
    int targetId = Random.Range(0, partyMembers.Length);
    selectedTargetPartyId[CurrentBattlerId] = (SkillTarget)targetId;
  }

  public void EnemyAttack() {
    SkillTarget targetId = selectedTargetPartyId[CurrentBattlerId];
    VsnController.instance.state = ExecutionState.WAITING;
    Skill skillUsed = ((Enemy)CurrentBattler).DecideWhichSkillToUse();

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

    StartCoroutine(ExecuteBattlerSkill(targetId, skillUsed));
  }



  public void EndTurn() {
    foreach(Pilot p in partyMembers) {
      p.EndTurn();
    }
    foreach(Enemy en in enemyMembers) {
      en.EndTurn();
    }
  }

  public void SetCustomBattle(int enemyId) {
    currentDateLocation = DateLocation.desert;
    enemyMembers = new Enemy[] { GetEnemyById(enemyId) };
  }

  public void SetupDateLocation(){
    TheaterController.instance.SetLocation(currentDateLocation.ToString());
  }

  public Enemy GetEnemyById(int id) {
    foreach(Enemy enemy in allEnemies) {
      if(enemy.id == id) {
        return enemy;
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
        return enemyMembers[0];
    }
    return null;
  }



  public void LoadAllEnemies() {
    allEnemies = new List<Enemy>();

    ActionSkin actionSkin;
    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(enemiesFile, 1);

    foreach(Dictionary<string, string> dic in spreadsheetData.data) {
      Debug.LogWarning("Loading enemy: " + dic["name key"]);

      Enemy loadedEnemy = JsonUtility.FromJson<Enemy>("{\"activeSkillLogics\" :" + dic["active skills logic"] + "}");

      loadedEnemy.id = int.Parse(dic["id"]);
      loadedEnemy.nameKey = dic["name key"];

      loadedEnemy.attributes = new int[]{
        int.Parse(dic["maxHp"]),
        int.Parse(dic["movementRange"]),
        int.Parse(dic["attack"]),
        int.Parse(dic["agility"]),
        int.Parse(dic["dodgeRate"]) };

      loadedEnemy.statusImmunities = Utils.SeparateTags(dic["status immunities"]);
      loadedEnemy.passiveSkills = Utils.SeparateInts(dic["passive skills"]);
      loadedEnemy.expReward = int.Parse(dic["exp reward"]);
      loadedEnemy.moneyReward = int.Parse(dic["money reward"]);

      loadedEnemy.spriteName = dic["sprite name"];

      actionSkin = new ActionSkin();
      actionSkin.sfxName = dic["base attack sfx"];
      actionSkin.animation = GetSkillAnimationByString(dic["base attack animation"]);
      actionSkin.animationArgument = Utils.GetStringArgument(dic["base attack animation"]);
      loadedEnemy.baseAttackSkin = actionSkin;

      loadedEnemy.appearSfxName = dic["appear sfx"];
      loadedEnemy.tags = Utils.SeparateTags(dic["tags"]);

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
      newSkill.type = (SkillType)System.Enum.Parse(typeof(SkillType), entry["type"]);
      newSkill.range = (ActionRange)System.Enum.Parse(typeof(ActionRange), entry["range"]);

      if(!string.IsNullOrEmpty(entry["sp cost"])) {
        newSkill.spCost = int.Parse(entry["sp cost"]);
      } else {
        newSkill.spCost = 0;
      }

      if(!string.IsNullOrEmpty(entry["sp cost"])) {
        newSkill.specialEffect = (SkillSpecialEffect)System.Enum.Parse(typeof(SkillSpecialEffect), entry["special effect"]);
      } else {
        newSkill.specialEffect = SkillSpecialEffect.none;
      }

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
        newSkill.activationTrigger = (PassiveSkillActivationTrigger)System.Enum.Parse(typeof(PassiveSkillActivationTrigger), entry["passive trigger"]);
      } else {
        newSkill.activationTrigger = PassiveSkillActivationTrigger.none;
      }
      if(!string.IsNullOrEmpty(entry["trigger conditions"])) {
        newSkill.triggerConditions = Utils.SeparateTags(entry["trigger conditions"]);
      }
      if(!string.IsNullOrEmpty(entry["trigger chance"])) {
        newSkill.triggerChance = float.Parse(entry["trigger chance"]);
      }

      newSkill.sprite = Resources.Load<Sprite>("Icons/" + entry["sprite"]);
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
