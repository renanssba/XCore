using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "event_resolution")]
  public class EventResolutionCommand : VsnCommand {

    public override void Execute() {
      DateEvent currentEvent = GameController.instance.GetCurrentEvent();
      int currentEventId = VsnSaveSystem.GetIntVariable("currentDateEvent");
      Person p = null;
      int attributeToUse;
      int result = 0;

      switch(currentEvent.interactionType){
        case EventInteractionType.male:
          p = GlobalData.instance.GetCurrentBoy();
          break;
        case EventInteractionType.female:
          p = GlobalData.instance.GetCurrentGirl();
          break;
      }

      if (args.Length > 0) {
        attributeToUse = (int)args[0].GetNumberValue();
      } else {
        attributeToUse = (int)p.AttributetoUse();
      }

      result = attributeToUse;
      if (p.AttributeValue(attributeToUse) < currentEvent.difficultyForAttribute[attributeToUse]){
        result += 3;
      }
      VsnSaveSystem.SetVariable("resolution", result);
      if(result < 3){
        VsnSaveSystem.SetVariable("date_event_result_" + currentEventId, 1);
      }else{
        VsnSaveSystem.SetVariable("date_event_result_" + currentEventId, 2);
      }

      GameController.instance.UpdateUI();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
      signatures.Add(new VsnArgType[] { 
        VsnArgType.numberArg
      });
    }
  }
}
