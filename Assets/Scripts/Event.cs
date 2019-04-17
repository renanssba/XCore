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


[System.Serializable]
public class DateEvent {

  public string scriptName;
  public int id;
  public int[] difficultyForAttribute;
  public DateEventInteractionType interactionType;

  public RewardType rewardType;
  public int rewardId;

  public DateEvent(int newid, string name, int difGuts, int difInt, int difCha, DateEventInteractionType interType){
    id = newid;
    scriptName = name;
    difficultyForAttribute = new int[3];
    difficultyForAttribute[0] = difGuts;
    difficultyForAttribute[1] = difInt;
    difficultyForAttribute[2] = difCha;
    interactionType = interType;
  }
}



public enum ObservationEventInteractionType{
  otherGenderPerson,
  sameGenderPerson,
  attributeTraining,
  itemOnSale,
  homeStalking
}


[System.Serializable]
public class ObservationEvent{
  public int id;
  public ObservationEventInteractionType eventType;
  public string eventScript;

  public Person personInEvent;
  public Item itemInEvent;

  public ItemCategory itemCategory;
  public Attributes challengedAttribute;
  public int challengeDifficulty;
  public int discountPercent;
}