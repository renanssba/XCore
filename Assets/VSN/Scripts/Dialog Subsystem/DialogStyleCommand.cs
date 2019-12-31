using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Command {

  [CommandAttribute(CommandString = "dialog_style")]
  public class DialogStyleCommand : VsnCommand {

    public override void Execute() {
      RectTransform rect = VsnUIManager.instance.vsnMessagePanel.GetComponent<RectTransform>();

      VsnUIManager.instance.SetTextBaseColor(Color.white);
      VsnUIManager.instance.SetDialogBoxInvisible(false);

      switch(args[0].GetStringValue()) {
        case "up":
        case "down":
        case "center":
          VsnUIManager.instance.SetDialogBoxPosition(args[0].GetStringValue());
          break;
        case "normal":
          VsnUIManager.instance.SetDialogBoxPosition("down");
          break;
        case "faded_screen_message":
          VsnUIManager.instance.SetDialogBoxPosition("center");
          VsnUIManager.instance.SetTextBaseColor(new Color(0.14f, 0.54f, 0.8f));
          VsnUIManager.instance.SetDialogBoxInvisible(true);
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