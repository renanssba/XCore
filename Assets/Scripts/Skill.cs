using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillEffect {
  sensor,
  raiseAttribute,
  flee,
  gluttony,
  bondSkill,
  none
}

public enum SkillType {
  attack,
  active,
  passive
}

[System.Serializable]
public class Skill {
  public string name;
  public int id;

  public string description;
  public Sprite sprite;
  public SkillType type;

  public Attributes attribute;
  public float multiplier;

  public SkillEffect skillEffect;
  public int duration;
  public int spCost;
}


