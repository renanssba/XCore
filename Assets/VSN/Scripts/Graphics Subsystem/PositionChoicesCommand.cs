using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "position_choices")]
  public class PositionChoicesCommand : VsnCommand {

    public override void Execute() {
      Vector2 pos = Vector2.zero;

      switch(args[0].GetStringValue()) {
        case "date_event":
          switch(GameController.instance.GetCurrentDateEvent().interactionType) {
            case DateEventInteractionType.male:
            case DateEventInteractionType.couple:
              pos = new Vector2(-130f, 731f);
              break;
            case DateEventInteractionType.female:
              pos = new Vector2(48f, 636f);
              break;
          }
          break;
        case "base":
          pos = new Vector2(685, 425);
          break;
      }
      VsnUIManager.instance.choicesPanel.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = pos;
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }

  }
}