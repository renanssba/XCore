using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "while")]
  public class WhileCommand : VsnCommand {

    public override void Execute() {
      bool comparisonResult = ((VsnOperator)args[1]).EvaluateComparison(args[0], args[2]);
      if(comparisonResult == false) {
        VsnController.instance.CurrentScriptReader().GotoNextEndwhile();
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[]{
        VsnArgType.numberArg,
        VsnArgType.operatorArg,
        VsnArgType.numberArg
      });

      signatures.Add(new VsnArgType[]{
        VsnArgType.stringArg,
        VsnArgType.operatorArg,
        VsnArgType.stringArg
      });

      signatures.Add(new VsnArgType[]{
        VsnArgType.booleanArg,
        VsnArgType.operatorArg,
        VsnArgType.booleanArg
      });
    }
  }
}