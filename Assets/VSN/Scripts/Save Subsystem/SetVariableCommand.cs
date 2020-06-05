using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "set_var")]
  public class SetVariableCommand : VsnCommand {

    public override void Execute() {
      float fvalue = args[1].GetNumberValue();
      string svalue = args[1].GetStringValue();

      //Debug.Log("SET VAR FLOAT: "+fvalue+  ", STRING: " + svalue);

      switch(args[1].GetVsnValueType()) {
        case VsnArgType.booleanArg:
          VsnSaveSystem.SetVariable(args[0].GetReference(), args[1].GetBooleanValue());
          return;
        case VsnArgType.stringArg:
          VsnSaveSystem.SetVariable(args[0].GetReference(), args[1].GetStringValue());
          return;
        case VsnArgType.numberArg:
          VsnSaveSystem.SetVariable(args[0].GetReference(), args[1].GetNumberValue());
          return;
      }

      Debug.LogWarning(args[0].GetReference() + " var value: " + args[1].GetPrintableValue());
    }

    public override void AddSupportedSignatures(){
      signatures.Add( new VsnArgType[]{
        VsnArgType.referenceArg,
        VsnArgType.numberArg
      } );

      signatures.Add( new VsnArgType[]{
        VsnArgType.referenceArg,
        VsnArgType.stringArg
      } );

      signatures.Add(new VsnArgType[]{
        VsnArgType.referenceArg,
        VsnArgType.booleanArg
      });
    }
  }
}