using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DateEventInteractionType{
  male,
  female,
  couple,
  compatibility
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
  public int difficulty;
  public int stage;
  public string location;
  public string spriteName;
  public DateEventInteractionType interactionType;

  public RewardType rewardType;
  public int rewardId;
}



public enum ObservationEventType{
  femaleInTrouble = 0,
  maleInTrouble = 1,
  attributeTraining = 2,
  bet = 3,
  itemOnSale = 4,
  homeStalking = 5
}


[System.Serializable]
public class ObservationEvent{
  public int id;
  public ObservationEventType eventType;
  public string scriptName;
  public string location;

  public Item itemInEvent;

  public ItemCategory itemCategory;
  public Attributes challengedAttribute;
  public int challengeDifficulty;
  public int discountPercent;
}