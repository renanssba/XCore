using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "event_resolution")]
  public class EventResolutionCommand : VsnCommand {

    public override void Execute() {
      if(args[0].GetStringValue() == "date"){
        ResolveDate();
      }else{
        //ResolveObservation();
      }
    }


    public void ResolveDate(){
      DateEvent currentEvent = GameController.instance.GetCurrentDateEvent();
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

      if (args.Length > 0) {
        attributeToUse = (int)args[1].GetNumberValue();
      } else {
        attributeToUse = (int)p.AttributetoUse();
      }

      result = attributeToUse;
      if(VsnSaveSystem.GetIntVariable("attribute_effective_level") * currentEvent.attributeEffectivity[attributeToUse] < currentEvent.difficulty) {
        result += 3;
      }
      VsnSaveSystem.SetVariable("resolution", result);
      //if (result < 3) {
      //  VsnSaveSystem.SetVariable("date_event_result_" + currentEventId, 1);
      //} else {
      //  VsnSaveSystem.SetVariable("date_event_result_" + currentEventId, 2);
      //}
      //GameController.instance.UpdateUI();
      GameController.instance.EndTurn();
    }


    //public void ResolveObservation() {
    //  ObservationEvent currentEvent = GameController.instance.GetCurrentObservationEvent();
    //  Person person = GlobalData.instance.ObservedPerson();
    //  int attributeToUse = (int)currentEvent.challengedAttribute;
    //  int result = 0;

    //  if (person.AttributeValue(attributeToUse) >= currentEvent.challengeDifficulty) {
    //    result = 0;
    //  }else{
    //    result = 1;
    //  }
    //  VsnSaveSystem.SetVariable("resolution", result);
    //}


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
