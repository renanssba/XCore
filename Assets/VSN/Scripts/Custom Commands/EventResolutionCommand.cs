using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "event_resolution")]
  public class EventResolutionCommand : VsnCommand {

    public override void Execute() {
      Event currentEvent = GameController.instance.GetCurrentEvent();
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
      attributeToUse = (int)p.AttributetoUse();

      result = attributeToUse;
      if (p.attributes[attributeToUse] < currentEvent.difficultyForAttribute[attributeToUse]){
        result += 3;
      }
      VsnSaveSystem.SetVariable("resolution", result);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
