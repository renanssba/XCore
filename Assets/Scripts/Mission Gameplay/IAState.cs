using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState {
  public CharacterToken unit;
  public int hp;
  public Vector2Int pos;
  public List<StatusCondition> statusConditions;

  public static readonly List<StatusConditionId> goodStatus = new List<StatusConditionId> { StatusConditionId.inspired };
  public static readonly List<StatusConditionId> badStatus = new List<StatusConditionId>();

  public UnitState(CharacterToken character) {
    unit = character;
    hp = character.battler.hp;
    pos = character.BoardGridPosition();
    statusConditions = new List<StatusCondition>();
    statusConditions.AddRange(character.battler.statusConditions);
  }


  public int DistanceToClosestOpponent(List<UnitState> allUnitStates) {
    CombatTeam opponentTeam = unit.OpponentCombatTeam();
    int closest = int.MaxValue;

    foreach(UnitState opponentUnitState in allUnitStates) {
      if(opponentUnitState.unit.combatTeam == opponentTeam) {
        int distance = BoardController.instance.pathfinding.DistanceToPosition(pos, opponentUnitState.pos);
        if(closest > distance) {
          closest = distance;
        }
      }
    }
    if(closest == int.MaxValue) {
      return 0;
    }
    return closest;
  }

  public float StatusEffectHeuristic() {
    float sum = 0;
    foreach(StatusCondition sc in statusConditions) {
      if(goodStatus.Contains(sc.id)) {
        sum += 3;
      }
      if(badStatus.Contains(sc.id)) {
        sum -= 5;
      }
    }
    return sum;
  }

  public StatusCondition GetStatusCondition(StatusConditionId id) {
    foreach(StatusCondition sc in statusConditions) {
      if(sc.id == id) {
        return sc;
      }
    }
    return null;
  }


  public void ReceiveStatusCondition(StatusConditionId statusId, int statusDuration) {
    StatusCondition newCondition = new StatusCondition {
      id = statusId,
      duration = statusDuration
    };

    foreach(StatusCondition sc in statusConditions) {
      if(sc.id == statusId) {
        sc.duration = Mathf.Max(sc.duration, statusDuration);
        return;
      }
    }
    statusConditions.Add(newCondition);
  }

  public void ReduceStatusConditionDuration(StatusConditionId statusCondition, int durationReduction) {
    for(int i = statusConditions.Count - 1; i >= 0; i--) {
      if(statusConditions[i].id == statusCondition) {
        if(statusConditions[i].duration > 0) {
          statusConditions[i].duration -= durationReduction;
        }
        if(statusConditions[i].duration == 0) {
          statusConditions.RemoveAt(i);
        }
      }
    }
  }

  public bool ContainsStatusCondition(StatusConditionId statusId) {
    foreach(StatusCondition sc in statusConditions) {
      if(sc.id == statusId) {
        return true;
      }
    }
    return false;
  }

  public void RemoveStatusCondition(StatusConditionId statusToRemove) {
    for(int i = statusConditions.Count - 1; i >= 0; i--) {
      if(statusConditions[i].id == statusToRemove) {
        statusConditions.RemoveAt(i);
      }
    }
  }


  public void TakeDamage(int damage) {
    hp -= damage;
    hp = Mathf.Min(hp, unit.battler.GetAttributeValue(Attributes.maxHp));
    hp = Mathf.Max(0, hp);
  }

  public bool IsDead() {
    return hp <= 0;
  }


  //public void ReceiveSkillEffect(SkillData skillReceived) {
  //  TakeDamage(skillReceived.baseDamage);

  //  if(skillReceived.receiveStatusEffect != StatusConditionId.none) {
  //    ReceiveStatusCondition(skillReceived.receiveStatusEffect, skillReceived.statusEffectDuration);
  //  }
  //  RemoveStatusCondition(skillReceived.removeStatusEffect);
  //}
}


[System.Serializable]
public class IAState {
  public Vector2Int tileInput;

  public List<UnitState> allUnitStates;
  public float heuristic;
  public bool advancesTurn = false;

  public CharacterToken engagedUnit = null;


  public IAState(Vector2Int tileChosen) {
    allUnitStates = new List<UnitState>();
    tileInput = tileChosen;
    engagedUnit = null;

    InitializeUnitStates();
  }


  public void InitializeUnitStates() {
    foreach(CharacterToken character in GameController.instance.allCharacters) {
      allUnitStates.Add(new UnitState(character));
    }
    advancesTurn = false;
  }

  public void CharacterMoves(CharacterToken unit, Vector2Int moveTo) {
    UnitState us = FindUnitState(unit);
    us.pos = moveTo;
  }

  public void EngageWith(CharacterToken unitEngaged) {
    /// TODO: implement this and use to calculate heuristics
  }

  public void CharacterReceivesStatus(CharacterToken unit, StatusConditionId statusId, int statusDuration) {
    UnitState us = FindUnitState(unit);
    if(us != null) {
      us.ReceiveStatusCondition(statusId, statusDuration);
    }
  }

  public void RemoveDeadUnits() {
    for(int i = allUnitStates.Count - 1; i >= 0; i--) {
      if(allUnitStates[i].IsDead()) {
        allUnitStates.RemoveAt(i);
      }
    }
  }

  public void ActionEndsTurn() {
    advancesTurn = true;
  }

  public UnitState FindUnitState(CharacterToken unitToFind) {
    foreach(UnitState us in allUnitStates) {
      if(us.unit == unitToFind) {
        return us;
      }
    }
    return null;
  }

  public UnitState FindUnitStateByPosition(Vector2Int pos) {
    foreach(UnitState us in allUnitStates) {
      if(us.pos == pos) {
        return us;
      }
    }
    return null;
  }




  public float CalculateHeuristic() {
    float sum = 0;
    CombatTeam myTeam = GameController.instance.CurrentCharacter.combatTeam;
    CombatTeam enemyTeam = GameController.instance.CurrentCharacter.OpponentCombatTeam();

    // enemy units alive are bad. my units alive are good
    sum += -10000f * CountCharactersByTeam(enemyTeam);
    sum += 10000f * CountCharactersByTeam(myTeam);

    // enemy units with more hp are bad. my units with more hp are good
    sum += -100f * CountHpByTeam(enemyTeam);
    sum += 100f * CountHpByTeam(myTeam);

    // having low HP on player team is good. on my team is bad
    sum += -80f * LowestHpByTeam(enemyTeam);
    sum += 80f * LowestHpByTeam(myTeam);

    // enemy units with positive status effects are bad. my units with positive status effects are good
    sum += -5f * SumStatusEffects(enemyTeam);
    sum += 5f * SumStatusEffects(myTeam);

    // being far from enemy units is bad. this is to incentivize enemies to get close and attack
    // TODO: improve this. should be something more related to being in position to hit enemies with skills
    sum += -1f * SumDistanceToEnemyUnits(myTeam);

    /// ending the turn is not desired
    sum += advancesTurn ? -5f : 0f;

    heuristic = sum;
    return sum;
  }

  public int CountCharactersByTeam(CombatTeam team) {
    int count = 0;
    foreach(UnitState unitState in allUnitStates) {
      if(unitState.unit.combatTeam == team) {
        count++;
      }
    }
    return count;
  }

  public int CountHpByTeam(CombatTeam team) {
    int count = 0;
    foreach(UnitState unitState in allUnitStates) {
      if(unitState.unit.combatTeam == team) {
        count += unitState.hp;
      }
    }
    return count;
  }

  public int LowestHpByTeam(CombatTeam team) {
    int lowest = int.MaxValue;

    foreach(UnitState unitState in allUnitStates) {
      if(unitState.unit.combatTeam == team && unitState.hp < lowest) {
        lowest = unitState.hp;
      }
    }
    if(lowest == int.MaxValue) {
      return 0;
    }
    return lowest;
  }

  public int SumDistanceToEnemyUnits(CombatTeam team) {
    int sum = 0;
    foreach(UnitState unitState in allUnitStates) {
      if(unitState.unit.combatTeam == team) {
        sum += unitState.DistanceToClosestOpponent(allUnitStates);
      }
    }
    return sum;
  }

  //public int DistanceToClosestOpponent(UnitState pcUnit) {
  //  CombatTeam opponentTeam = ((Character)pcUnit.unit).OpponentCombatTeam();
  //  int closest = 10000;

  //  foreach(UnitState opponentUnitState in allUnitStates) {
  //    if(opponentUnitState.unit.combatTeam == opponentTeam) {
  //      int distance = BoardController.DistanceBetweenTiles(pcUnit.pos, opponentUnitState.pos);
  //      if(closest > distance) {
  //        closest = distance;
  //      }
  //    }
  //  }
  //  return closest;
  //}

  public float SumStatusEffects(CombatTeam team) {
    float count = 0;
    foreach(UnitState unitState in allUnitStates) {
      if(unitState.unit.combatTeam == team) {
        count += unitState.StatusEffectHeuristic();
      }
    }
    return count;
  }
}
