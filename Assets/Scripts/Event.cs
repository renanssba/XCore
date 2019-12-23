using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DateEventInteractionType{
  male,
  female,
  couple
}


public enum RewardType{
  none,
  item,
  money
}


public enum DateLocation{
  parque,
  shopping,
  generico
}


[System.Serializable]
public class DateEvent {

  public string scriptName;
  public int id;
  public float[] attributeEffectivity;
  public int level;
  public int stage;
  public string location;
  public string spriteName;
  public DateEventInteractionType interactionType;

  public int maxHp;
  public int hp;

  public Attributes attackAttribute;
  public int attackDamage;
  public string[] givesConditionNames;
  public int giveStatusConditionChance;

  public RewardType rewardType;
  public int rewardId;

  public Attributes[] GetWeaknesses() {
    List<Attributes> att = new List<Attributes>();
    for(int i=0; i<4; i++) {
      if(attributeEffectivity[i] > 1f) {
        att.Add((Attributes)i);
      }
    }
    return att.ToArray();
  }

  public Attributes[] GetResistances() {
    List<Attributes> att = new List<Attributes>();
    for(int i = 0; i < 4; i++) {
      if(attributeEffectivity[i] < 1f) {
        att.Add((Attributes)i);
      }
    }
    return att.ToArray();
  }
}
