using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "actor_emote")]
  public class ActorEmoteCommand : VsnCommand {

    public override void Execute() {
      Actor2D actor = TheaterController.instance.GetActorByString(args[0].GetReference());

      switch(args[1].GetStringValue()) {
        case "surprised":
          actor.DetectAnimation();
          WaitForSeconds(actor, 0.5f);
          break;
      }
    }

    public void WaitForSeconds(Actor2D actor, float time) {
      VsnController.instance.state = ExecutionState.WAITING;
      actor.StartCoroutine(WaitToResume(time));
    }

    public IEnumerator WaitToResume(float time) {
      yield return new WaitForSeconds(time);
      VsnController.instance.state = ExecutionState.PLAYING;
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg
      });
    }
  }
}
