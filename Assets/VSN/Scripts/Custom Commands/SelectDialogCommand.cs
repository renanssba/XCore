using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "select_dialog")]
  public class SelectDialogCommand : VsnCommand {

    public override void Execute() {
      List<string> acquaintances = new List<string>() { "acquaintances_1", "acquaintances_2", "acquaintances_3" };
      List<string> friends = new List<string>() { "friends_1", "friends_2", "friends_3" };
      List<string> lovers = new List<string>() { "lovers_1", "lovers_2", "lovers_3" };
      Relationship relationship = GlobalData.instance.GetCurrentRelationship();
      string selectedDialog = "fallback";

      for(int i = 0; i < relationship.talkedDialogs.Count; i++) {
        acquaintances.Remove(relationship.talkedDialogs[i]);
        friends.Remove(relationship.talkedDialogs[i]);
        lovers.Remove(relationship.talkedDialogs[i]);
      }


      if(acquaintances.Count > 0) {
        selectedDialog = acquaintances[Random.Range(0, acquaintances.Count)];
        relationship.talkedDialogs.Add(selectedDialog);
        goto end;
      }

      if(relationship.heartLocksOpened >= 1 && friends.Count > 0) {
        selectedDialog = friends[Random.Range(0, friends.Count)];
        relationship.talkedDialogs.Add(selectedDialog);
        goto end;
      }

      if(relationship.heartLocksOpened >= 2 && lovers.Count > 0) {
        selectedDialog = lovers[Random.Range(0, lovers.Count)];
        relationship.talkedDialogs.Add(selectedDialog);
        goto end;
      }

      end:
      GotoCommand.StaticExecute("dialog_" + selectedDialog);
      return;


      List<string> possibleDialogs = new List<string>();
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