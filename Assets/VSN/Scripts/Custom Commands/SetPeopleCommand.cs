using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_people")]
  public class SetPeopleCommand : VsnCommand {

    public override void Execute() {

      VsnSaveSystem.SetVariable("people_ui_state", args[0].GetStringValue());

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
          GameController.instance.personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
          GameController.instance.personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
          break;
        case "date":
          GameController.instance.personCards[0].gameObject.SetActive(true);
          GameController.instance.personCards[1].gameObject.SetActive(true);
          GameController.instance.personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
          GameController.instance.personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
          break;
        case "event":
          switch (GameController.instance.GetCurrentEvent().interactionType) {
            case DateEventInteractionType.male:
              GameController.instance.personCards[0].gameObject.SetActive(true);
              GameController.instance.personCards[1].gameObject.SetActive(true);
              GameController.instance.personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
              GameController.instance.personCards[1].GetComponent<CanvasGroup>().alpha = 0.65f;
              break;
            case DateEventInteractionType.female:
              GameController.instance.personCards[0].gameObject.SetActive(true);
              GameController.instance.personCards[1].gameObject.SetActive(true);
              GameController.instance.personCards[0].GetComponent<CanvasGroup>().alpha = 0.65f;
              GameController.instance.personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
              break;
            case DateEventInteractionType.couple:
              GameController.instance.personCards[0].gameObject.SetActive(true);
              GameController.instance.personCards[1].gameObject.SetActive(true);
              GameController.instance.personCards[0].GetComponent<CanvasGroup>().alpha = 1f;
              GameController.instance.personCards[1].GetComponent<CanvasGroup>().alpha = 1f;
              break;
          }
          break;
      }
      GameController.instance.UpdateUI();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
