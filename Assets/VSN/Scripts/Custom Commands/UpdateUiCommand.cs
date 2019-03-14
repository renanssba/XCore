using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "update_ui")]
  public class UpdateUiCommand : VsnCommand {

    public override void Execute() {
      GameController.instance.UpdateUI();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]{});
    }
  }
}