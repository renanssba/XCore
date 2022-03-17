using UnityEngine;
using Lean.Common;
using System.Collections.Generic;
using NaughtyAttributes;

namespace Lean.Localization {
  /// <summary>This component will load localizations from a CSV file. By default they should be in the format:
  /// Phrase Name Here = Translation Here // Optional Comment Here
  /// </summary>
  [HelpURL(LeanLocalization.HelpUrlPrefix + "LeanLanguageCSV")]
  [AddComponentMenu(LeanLocalization.ComponentPathPrefix + "Language CSV")]
  public class LeanLanguageCSVMulti : MonoBehaviour {
    /// <summary>The text asset that contains all the translations.</summary>
    public TextAsset Source;

    /// <summary>The string separating the phrase name from the translation.</summary>
    public char separator = ',';

    /// <summary>The string denoting a new line within a translation.</summary>
    public string NewLine = "\\n";

    /// <summary>The characters used to separate each translation.</summary>
    private static readonly char[] newlineCharacters = new char[] { '\r', '\n' };

    protected virtual void Start() {
      LoadFromSource();
    }

    [ContextMenu("Load From Source")]
    [Button]
    public void LoadFromSource() {
      if(Source != null) {
        // Split file into lines, and loop through them all
        var lines = Source.text.Split(newlineCharacters, System.StringSplitOptions.RemoveEmptyEntries);

        string[] langs = GetPartsFromLine(lines[0]);

        for(var i = 1; i < lines.Length; i++) {
          string line = lines[i];
          var equalsIndex = line.IndexOf(separator);

          // Only consider lines with the Separator character
          if(equalsIndex != -1) {
            string[] parts = GetPartsFromLine(line, langs.Length);

            for(int j = 0; j < langs.Length-1; j++) {
              // Find or add the translation for this phrase
              LeanLocalization.AddTranslationToFirst(parts[0], langs[j+1], parts[j+1]);
            }
          }
        }

        LeanLocalization.UpdateTranslations();
      }
    }

    public string[] GetPartsFromLine(string line, int minParts = -1) {
      List<string> parts = new List<string>();

      int startingPos =0;
      bool commaOpened = false;

      //Debug.Log("Line: " + line);

      for(int i=0; i<line.Length; i++) {
        //Debug.Log("i: " + i + ", current char: " + line[i]);

        // end quotes segment
        if(line[i]=='"' && commaOpened /**/ && IsCharCommaOrEndline(line, i+1)/**/) {
          //Debug.Log("// end quotes segment. i: " + i + ", startingPos: " + startingPos);
          if(i - startingPos - 1 <= 0) {
            parts.Add("");
          } else {
            parts.Add(line.Substring(startingPos + 1, i - startingPos - 1));
          }          
          i++;
          commaOpened = !commaOpened;
          startingPos = i + 1;
          continue;
        }

        // toggle comma opened
        if(line[i] == '"') {
          commaOpened = !commaOpened;
          //Debug.Log("// toggle comma being set to " + commaOpened + ". i: " + i + ", startingPos: " + startingPos);
          continue;
        }

        // end part if found separator outside comma
        if(line[i] == separator && !commaOpened) {
          //Debug.Log("// end part if found separator outside comma. i: " + i + ", startingPos: " + startingPos);
          if(i - startingPos <= 0) {
            parts.Add("");
          } else {
            parts.Add(line.Substring(startingPos, i - startingPos));
          }
          startingPos = i + 1;
          continue;
        }

        // end part because line ended
        if(i == line.Length - 1) {
          //Debug.Log("// end part because line ended. i: " + i + ", startingPos: " + startingPos);

          if(i - startingPos + 1 <= 0) {
            parts.Add("");
            //if(line[i] == separator) {
            //  parts.Add("");
            //}
          } else {
            parts.Add(line.Substring(startingPos, i - startingPos + 1));
          }
          continue;
        }

      }

      //Debug.LogWarning("Parts");
      for(int i = 0; i < parts.Count; i++) {
        parts[i] = parts[i].Replace("\"\"", "\"");

        // Replace newline markers with actual newlines
        if(string.IsNullOrEmpty(NewLine) == false) {
          parts[i] = parts[i].Replace(NewLine, System.Environment.NewLine);
        }
        //Debug.LogWarning("part: " + parts[i]);
      }

      while(parts.Count < minParts && minParts != -1) {
        parts.Add("");
      }

      return parts.ToArray();
    }

    public bool IsCharCommaOrEndline(string line, int id) {
      if(id == line.Length) {
        return true;
      }
      if(id > line.Length) {
        return false;
      }
      if(line[id] == ',' || line[id] == '\n' || line[id] == '\r') {
        return true;
      }
      return false;
    }
  }
}