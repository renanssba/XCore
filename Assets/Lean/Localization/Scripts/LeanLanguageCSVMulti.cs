using UnityEngine;
using Lean.Common;
using System.Collections.Generic;

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
            string[] parts = GetPartsFromLine(line);

            for(int j = 0; j < langs.Length-1; j++) {
              // Find or add the translation for this phrase
              LeanLocalization.AddTranslationToFirst(parts[0], langs[j+1], parts[j+1]);
            }
          }
        }

        LeanLocalization.UpdateTranslations();
      }
    }

    public string[] GetPartsFromLine(string line) {
      List<string> parts = new List<string>();

      int startingPos =0;
      bool commaOpened = false;

      for(int i=0; i<line.Length; i++) {
        // end quotes segment
        if(line[i]=='"' && commaOpened && IsCharComma(line, i+1)) {
          parts.Add(line.Substring(startingPos+1, i - startingPos-1));
          i++;
          startingPos = i + 1;
          continue;
        }

        // toggle comma opened
        if(line[i] == '"') {
          commaOpened = !commaOpened;
          continue;
        }

        // end part because line ended
        if(i == line.Length-1) {
          parts.Add(line.Substring(startingPos, i - startingPos+1));
          continue;
        }

        // end part if found separator outside comma
        if(line[i] == separator && !commaOpened) {
          parts.Add(line.Substring(startingPos, i - startingPos));
          startingPos = i + 1;
          continue;
        }
      }

      for(int i = 0; i < parts.Count; i++) {
        parts[i] = parts[i].Replace("\"\"", "\"");

        // Replace newline markers with actual newlines
        if(string.IsNullOrEmpty(NewLine) == false) {
          parts[i] = parts[i].Replace(NewLine, System.Environment.NewLine);
        }
        //Debug.LogWarning("part: " + parts[i]);
      }

      return parts.ToArray();
    }

    public bool IsCharComma(string line, int id) {
      if(line[id] == ',') {
        return true;
      }
      return false;
    }
  }
}