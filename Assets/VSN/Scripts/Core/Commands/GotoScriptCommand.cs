using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "goto_script")]
  public class GotoScriptCommand : VsnCommand {

    public override void Execute() {
      VsnArgument[] newArgs = new VsnArgument[args.Length-1];
      for(int i=1; i<args.Length; i++){
        newArgs[i-1] = args[i];
      }
      StaticExecute(args[0].GetStringValue(), newArgs);
    }


    public static void StaticExecute(string scriptToRead, VsnArgument[] args) {
      VsnController.instance.GotoVSNScript(scriptToRead, args);
    }


    public override void AddSupportedSignatures(){}

    public override bool CheckSyntax(){
      /// only validates first arg
      if(args.Length > 0 && ArgumentMatchesType(args[0], VsnArgType.stringArg)){
        return true;
      }
      return false;
    }
  }
}