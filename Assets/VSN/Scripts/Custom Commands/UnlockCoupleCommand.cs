using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "unlock_couple")]
  public class UnlockCoupleCommand : VsnCommand {

    public override void Execute() {
      Debug.LogWarning(GlobalData.instance.CurrentCoupleName() + " agora são um casal viável!");
      GlobalData.instance.UnlockDateableCouple(GlobalData.instance.CurrentBoy(),
                                               GlobalData.instance.CurrentGirl());
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}