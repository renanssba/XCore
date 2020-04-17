using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "theater_setup")]
  public class TheaterSetupCommand : VsnCommand {

    public override void Execute() {
      string targetActor, newActorPrefabName;
      Vector3 positionToPut;
      Actor2D actor;

      TheaterController.instance.ClearTheater();
      for(int i = 0; i < args.Length / 3; i++) {
        targetActor = args[i*3].GetReference();
        newActorPrefabName = args[i*3+1].GetStringValue();
        positionToPut = TheaterController.instance.GetPositionByString(args[i*3+2].GetStringValue());

        TheaterController.instance.ChangeActor(targetActor, newActorPrefabName);
        actor = TheaterController.instance.GetActorByString(targetActor);
        actor.MoveToPosition(positionToPut, 0f);
      }
      actor = TheaterController.instance.GetActorByString("angel");
      actor.MoveToPosition(TheaterController.instance.GetPositionByString("angel"), 0f);
    }


    public override void AddSupportedSignatures() {
      VsnArgType[] argTypes;

      for(int i=1; i<=5; i++){
        argTypes = new VsnArgType[i*3];
        for(int j=0; j<i; j++){
          argTypes[j*3] = VsnArgType.referenceArg;
          argTypes[j*3+1] = VsnArgType.stringArg;
          argTypes[j*3+2] = VsnArgType.stringArg;
        }
        signatures.Add(argTypes);
      }
    }
  }
}
