using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "character_action_description")]
  public class CharacterActionDescriptionCommand : VsnCommand {

    public override void Execute() {
      VsnArgument[] newArgs = new VsnArgument[1];
      int partyMemberId = (int)args[0].GetNumberValue();
      string scriptToLoadPath = "action_descriptions";

      switch(BattleController.instance.selectedActionType[partyMemberId]) {
        case TurnActionType.useSkill:
          Skill usedSkill = BattleController.instance.selectedSkills[partyMemberId];
          if(usedSkill.type == SkillType.attack) {
            if(usedSkill.attribute != Attributes.endurance) {
              scriptToLoadPath = "date enemies/" + BattleController.instance.GetCurrentDateEventName();
              newArgs[0] = new VsnString(usedSkill.attribute.ToString() + "_action"+(BattleController.instance.partyMembers[partyMemberId].isMale ? "_boy":"_girl") );
            } else {
              scriptToLoadPath = "action_descriptions";
              newArgs[0] = new VsnString("magic_arrow_action");
            }
          } else {
            newArgs[0] = new VsnString(usedSkill.name + "_skill");
          }
          break;
        case TurnActionType.useItem:
          Item usedItem = BattleController.instance.selectedItems[partyMemberId];
          VsnSaveSystem.SetVariable("selected_item_name", usedItem.GetPrintableName());
          newArgs[0] = new VsnString("item");
      break;
        case TurnActionType.flee:
          newArgs[0] = new VsnString("flee");
          break;
        case TurnActionType.defend:
          newArgs[0] = new VsnString("defend");
          break;
      }
      GotoScriptCommand.StaticExecute(scriptToLoadPath, newArgs);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}