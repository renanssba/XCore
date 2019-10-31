using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "item_choice")]
  public class ItemChoiceCommand : VsnCommand {

    public override void Execute() {
      ItemSelectorScreen itemScreen = ItemSelectorScreen.instance;

      itemScreen.OpenGiftSelect();
      VsnUIManager.instance.choicesPanel.SetActive(false);
      VsnController.instance.WaitForCustomInput();
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]{});
    }
  }
}