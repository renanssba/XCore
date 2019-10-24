﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "improve_attribute")]
  public class ImproveAttributeCommand : VsnCommand {

    public override void Execute() {
      Person p = GlobalData.instance.ObservedPerson();
      int improve_value = (int)args[1].GetNumberValue();

      switch(args[0].GetStringValue()){
        case "guts":
          p.attributes[(int)Attributes.guts] += improve_value;
          break;
        case "intelligence":
          p.attributes[(int)Attributes.intelligence] += improve_value;
          break;
        case "charisma":
          p.attributes[(int)Attributes.charisma] += improve_value;
          break;
      }

      GameController.instance.UpdateUI();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });
    }
  }
}