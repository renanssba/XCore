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
      }
      
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
