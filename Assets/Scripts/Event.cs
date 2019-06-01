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
  public int[] difficultyForAttribute;
  public int stage;
  public string location;
  public DateEventInteractionType interactionType;

  public RewardType rewardType;
  public int rewardId;

  public DateEvent(int newid, string name, int difGuts, int difInt, int difCha, int stage, string location, DateEventInteractionType interType){
    id = newid;
    scriptName = name;
    this.stage = stage;
    this.location = location;
    difficultyForAttribute = new int[3];
    difficultyForAttribute[0] = difGuts;
    difficultyForAttribute[1] = difInt;
    difficultyForAttribute[2] = difCha;
    interactionType = interType;
  }
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

  public Person personInEvent;
  public Item itemInEvent;

  public ItemCategory itemCategory;
  public Attributes challengedAttribute;
  public int challengeDifficulty;
  public int discountPercent;
}