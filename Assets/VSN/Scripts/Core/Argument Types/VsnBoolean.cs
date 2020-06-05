using System;

public class VsnBoolean : VsnArgument {

  protected bool boolValue;

  public VsnBoolean(bool value) {
    boolValue = value;
  }

  public override bool GetBooleanValue() {
    return boolValue;
  }

  public override VsnArgType GetVsnValueType() {
    return VsnArgType.booleanArg;
  }
}

