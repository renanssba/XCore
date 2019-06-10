using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "bg3d")]
  public class Bg3DCommand : VsnCommand {

    public override void Execute() {
      if(args[0].GetReference() != "null") {
        Sprite backgroundSprite = Resources.Load<Sprite>("Bg/" + args[0].GetStringValue());
        if(backgroundSprite == null) {
          Debug.LogError("Error loading " + args[0].GetStringValue() + " Background sprite. Please check its path");
          return;
        }
        TheaterController.instance.SetBgSprite(backgroundSprite);
      } else {
        TheaterController.instance.ResetBgSprite();
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}