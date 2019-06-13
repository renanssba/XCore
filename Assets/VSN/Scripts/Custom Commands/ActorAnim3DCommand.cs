using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "actor_anim3d")]
  public class ActorAnim3DCommand : VsnCommand {

    public override void Execute() {
      TheaterController.instance.ActorAnimation();
      //if(args[0].GetReference() != "null") {

      //} else {
      //  TheaterController.instance.ResetBgSprite();
      //}
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}