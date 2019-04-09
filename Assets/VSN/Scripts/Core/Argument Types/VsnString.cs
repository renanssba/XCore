using System;

public class VsnString : VsnArgument{

  protected string stringValue;

  public VsnString(string text){
		stringValue = text;
  }

  public override string GetStringValue(){
    return SpecialCodes.InterpretStrings(stringValue.Replace('\'', '\"'));
  }
}

