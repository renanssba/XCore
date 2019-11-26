using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "resolve_action")]
  public class ResolveActionCommand : VsnCommand {

    public override void Execute() {
      if(args[0].GetStringValue() == "date"){
        ResolveAction();
      }
    }


    public void ResolveAction(){
      DateEvent currentEvent = BattleController.instance.GetCurrentDateEvent();
      int attributeToUse;
      //int result = 0;

      attributeToUse = (int)args[0].GetNumberValue();

      //result = attributeToUse;
      //if(VsnSaveSystem.GetIntVariable("attribute_effective_level") * currentEvent.attributeEffectivity[attributeToUse] < currentEvent.difficulty) {
      //  result += 3;
      //}
      VsnSaveSystem.SetVariable("resolution", attributeToUse);
      GameController.instance.EndTurn();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
