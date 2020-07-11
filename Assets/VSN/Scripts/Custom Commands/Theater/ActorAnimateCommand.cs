using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "actor_animate")]
  public class ActorAnimateCommand : VsnCommand {

    public override void Execute() {
      Actor2D actor = TheaterController.instance.GetActorByString(args[0].GetReference());

      ActionSkin skin = new ActionSkin();

      switch(args[1].GetStringValue()) {
        case "attack":
          skin.animation = SkillAnimation.attack;
          actor.StartCoroutine(actor.CharacterAttackAnim(skin));
          break;
        case "defend":
          actor.DefendActionAnimation();
          break;
        case "run over":
          skin.animation = SkillAnimation.run_over;
          actor.StartCoroutine(actor.CharacterAttackAnim(skin));
          break;
        case "shine red":
          actor.ShineRed();
          break;
        case "shine green":
          actor.ShineGreen();
          break;
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg
      });
    }
  }
}