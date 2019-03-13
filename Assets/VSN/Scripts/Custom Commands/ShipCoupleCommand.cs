using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "ship_couple")]
  public class ShipCoupleCommand : VsnCommand {

    public override void Execute() {
      GlobalData.instance.ShipCurrentCouple();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
