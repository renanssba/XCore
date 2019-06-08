using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "reveal_person")]
  public class RevealPersonCommand : VsnCommand {

    public override void Execute() {
      if(GlobalData.instance.EncounterPerson().state == PersonState.unrevealed) {
        GlobalData.instance.EncounterPerson().state = PersonState.available;
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}