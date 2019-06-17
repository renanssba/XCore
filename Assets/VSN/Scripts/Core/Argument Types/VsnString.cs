using System;
using UnityEngine;

public class VsnString : VsnArgument{

  protected string stringValue;

  public VsnString(string text){
		stringValue = text;
  }

  public override string GetStringValue(){
    return SpecialCodes.InterpretStrings(ReplaceSingleQuote());
  }

  public string ReplaceSingleQuote(){
    bool canReplace = false;
    string localizedString = Lean.Localization.LeanLocalization.GetTranslationText(stringValue);
    if(localizedString == null) {
      localizedString = stringValue;
    }

    char[] array = localizedString.ToCharArray();

    //Debug.Log("Text: " + new string(array));

    for (int i=0; i< localizedString.Length; i++){
      switch(localizedString[i]) {
        case '<':
          canReplace = true;
          break;
        case '>':
          canReplace = false;
          break;
        case '\'':
          if(canReplace){
            array[i] = '\"';
          }
          break;
      }
    }
    return new string(array);
  }
}

