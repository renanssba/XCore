using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "show_engagement")]
  public class ShowEngagementCommand : VsnCommand {

    public override void Execute() {
      int babies = Relationship.childrenNumber[GlobalData.instance.currentRelationshipId];
      //int babies = Random.Range(4, 9);
      //babies = 6;
      GameController.instance.ShowEngagementScreen(babies);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}