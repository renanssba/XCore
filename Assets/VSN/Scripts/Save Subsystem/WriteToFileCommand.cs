using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "write_to_file")]
  public class WriteToFileCommand : VsnCommand {

    public override void Execute() {
      string fileName = args[0].GetStringValue();
      string contentToWrite = args[1].GetStringValue();

      //string fileName = Application.dataPath + "/../version.txt";

      if(!File.Exists(fileName)) {
        File.Create(fileName).Close();
        File.WriteAllText(fileName, "");
      }
      File.AppendAllText(fileName, contentToWrite+ "\n");
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.stringArg
      });
    }
  }
}