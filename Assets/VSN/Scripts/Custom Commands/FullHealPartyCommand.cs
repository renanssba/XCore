using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "full_heal_party")]
  public class FullHealPartyCommand : VsnCommand {

    public override void Execute() {
      BattleController.instance.FullHealParty();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}