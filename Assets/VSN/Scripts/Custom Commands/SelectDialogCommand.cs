using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "select_dialog")]
  public class SelectDialogCommand : VsnCommand {

    public override void Execute() {
      string[] acquaintances = new string[] { "acquaintances_1", "acquaintances_2", "acquaintances_3" };
      string[] friends = new string[] { "friends_1", "friends_2", "friends_3" };
      string[] lovers = new string[] { "lovers_1", "lovers_2", "lovers_3" };
      List<string> possibleDialogs = new List<string>();
      Relationship relationship = GlobalData.instance.GetCurrentRelationship();

      switch(relationship.heartLocksOpened) {
        case 2:
          possibleDialogs.AddRange(lovers);
          possibleDialogs.AddRange(friends);
          possibleDialogs.AddRange(acquaintances);
          break;
        case 1:
          possibleDialogs.AddRange(friends);
          possibleDialogs.AddRange(acquaintances);
          break;
        case 0:
          possibleDialogs.AddRange(acquaintances);
          break;
      }
      for(int i=0; i< relationship.talkedDialogs.Count; i++) {
        possibleDialogs.Remove(relationship.talkedDialogs[i]);
      }

      string selectedDialog;
      if(possibleDialogs.Count > 0) {
        selectedDialog = possibleDialogs[Random.Range(0, possibleDialogs.Count)];
        relationship.talkedDialogs.Add(selectedDialog);
      } else {
        selectedDialog = "fallback";
      }
      GotoCommand.StaticExecute("dialog_" + selectedDialog);
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}