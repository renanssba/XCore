using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "event_resolution")]
  public class EventResolutionCommand : VsnCommand {

    public override void Execute() {
      if(args[0].GetStringValue() == "date"){
        ResolveDate();
      }
    }


    public void ResolveDate(){
      DateEvent currentEvent = BattleController.instance.GetCurrentDateEvent();
      int currentEventId = VsnSaveSystem.GetIntVariable("currentDateEvent");
      Person p = null;
      int attributeToUse;
      int result = 0;

      switch (currentEvent.interactionType) {
        case DateEventInteractionType.male:
          p = GlobalData.instance.CurrentBoy();
          break;
        case DateEventInteractionType.female:
          p = GlobalData.instance.CurrentGirl();
          break;
      }

      attributeToUse = (int)args[1].GetNumberValue();

      result = attributeToUse;
      if(VsnSaveSystem.GetIntVariable("attribute_effective_level") * currentEvent.attributeEffectivity[attributeToUse] < currentEvent.difficulty) {
        result += 3;
      }
      VsnSaveSystem.SetVariable("resolution", result);
      GameController.instance.EndTurn();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}
