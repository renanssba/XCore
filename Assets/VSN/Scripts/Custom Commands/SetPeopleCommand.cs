using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_people")]
  public class SetPeopleCommand : VsnCommand {

    public override void Execute() {

      GameController.instance.personCards[0].gameObject.SetActive(true);
      GameController.instance.personCards[1].gameObject.SetActive(true);
      switch (args[0].GetStringValue()){
        case "hide":
          GameController.instance.personCards[0].gameObject.SetActive(false);
          GameController.instance.personCards[1].gameObject.SetActive(false);
          break;
        case "show":
          GameController.instance.personCards[0].gameObject.SetActive(true);
          GameController.instance.personCards[1].gameObject.SetActive(true);
          GameController.instance.personCards[0].SetEquipableItems(true);
          GameController.instance.personCards[1].SetEquipableItems(true);
          break;
        case "date":
          GameController.instance.personCards[0].gameObject.SetActive(true);
          GameController.instance.personCards[1].gameObject.SetActive(true);
          GameController.instance.personCards[0].SetEquipableItems(false);
          GameController.instance.personCards[1].SetEquipableItems(false);
          break;
        case "event":
          GameController.instance.personCards[0].SetEquipableItems(false);
          GameController.instance.personCards[1].SetEquipableItems(false);
          switch (GameController.instance.GetCurrentEvent().interactionType) {
            case EventInteractionType.male:
              GameController.instance.personCards[0].gameObject.SetActive(true);
              GameController.instance.personCards[1].gameObject.SetActive(true);
              GameController.instance.personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
              GameController.instance.personCards[1].GetComponent<CanvasGroup>().alpha = 0.65f;
              break;
            case EventInteractionType.female:
              GameController.instance.personCards[0].gameObject.SetActive(true);
              GameController.instance.personCards[1].gameObject.SetActive(true);
              GameController.instance.personCards[0].GetComponent<CanvasGroup>().alpha = 0.65f;
              GameController.instance.personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
              break;
            case EventInteractionType.couple:
              GameController.instance.personCards[0].gameObject.SetActive(true);
              GameController.instance.personCards[1].gameObject.SetActive(true);
              GameController.instance.personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
              GameController.instance.personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
              break;
          }
          break;
      }
      
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
