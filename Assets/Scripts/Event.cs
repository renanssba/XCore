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
public class Event {

  public string scriptName;
  public int[] difficultyForAttribute;
  public EventInteractionType interactionType;

  public RewardType rewardType;
  public int rewardId;
}
