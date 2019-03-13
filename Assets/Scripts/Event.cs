using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventInteractionType{
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
  public EventInteractionType interactionType;

  public RewardType rewardType;
  public int rewardId;

  public DateEvent(int newid, string name, int difGuts, int difInt, int difCha, EventInteractionType interType){
    id = newid;
    scriptName = name;
    difficultyForAttribute = new int[3];
    difficultyForAttribute[0] = difGuts;
    difficultyForAttribute[1] = difInt;
    difficultyForAttribute[2] = difCha;
    interactionType = interType;
  }
}
