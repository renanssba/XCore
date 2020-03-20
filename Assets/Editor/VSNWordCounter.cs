using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using System.Text.RegularExpressions;
public class VSNWordCounter
{


  private static TextAsset[] vsnScripts;


  private static void LoadVSNFiles() {
    vsnScripts = Resources.LoadAll<TextAsset>("VSN Scripts");

  }
  [MenuItem("VSN/Word Counter")]
  private static void PrintWords() {
    LoadVSNFiles();
    int wordCount = 0;
    foreach (TextAsset t in vsnScripts)
    {
      wordCount += CountWordsInTextAsset(t);
    }

    Debug.Log("Words: " + wordCount);

  }

  private static int CountWordsInTextAsset( TextAsset t) {
    string[] lines = t.text.Split('\n');
    int wordCount = 0;
    foreach(string line in lines)
    {
      if(line.StartsWith("say"))
      {
        char[] delimiters = new char[] { '\"'};
        string[] content = line.Substring(3).Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        foreach(string c in content)
        {
          string s = c.Trim();
          if(!s.Equals("")){
            wordCount += CountWords(s);
          }
        }
      }
    }

    return wordCount;
  }

  private static int  CountWords(string s) {

    return s.Split(' ').Length;
  }
}
