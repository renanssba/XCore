using System;
using UnityEngine;

public class VsnString : VsnArgument{

  protected string stringValue;

  public VsnString(string text){
		stringValue = text;
  }

  public override string GetStringValue(){
    return SpecialCodes.InterpretStrings(LocalizedString());
  }

  public string LocalizedString(){
    string localizedString = Lean.Localization.LeanLocalization.GetTranslationText(stringValue);
    if(localizedString == null) {
      //Debug.Log("Couldn't localize string");
      localizedString = stringValue;
    }
    //Debug.Log("Localized string: " + localizedString);

    return localizedString;
  }
}

