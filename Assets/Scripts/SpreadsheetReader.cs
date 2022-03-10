using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SpreadsheetData {
  public Dictionary<string, string>[] data;
}


public class SpreadsheetReader : MonoBehaviour {
  private static char separator = '\t';

  public static SpreadsheetData ReadTabSeparatedText(string txt, int header_lines) {
    string[] lines = txt.Split('\n');
    int line_count = lines.Length;

    if(line_count < header_lines + 1) {
      Debug.LogError("Invalid File! Not enough lines!");
      return null;
    }

    string[] keys = lines[header_lines - 1].Split(separator);
    SpreadsheetData spreadsheet = new SpreadsheetData();
    spreadsheet.data = new Dictionary<string, string>[line_count - header_lines];

    //Debug.LogError("KEYS COUNT: " + keys.Length + ", LINES (ENTRIES) COUNT: " + spreadsheet.data.Length);

    for(int i = 0; i < spreadsheet.data.Length; i++) {
      spreadsheet.data[i] = new Dictionary<string, string>();
      string[] data_line = lines[i + header_lines].Split(separator);

      if(data_line.Length != keys.Length) {
        Debug.LogError("Invalid File! Field count is wrong!");
        return null;
      }

      for(int j = 0; j < keys.Length; j++) {
        if(spreadsheet.data[i].ContainsKey(keys[j])) {
          Debug.LogError("Key " + keys[j] + " already exists! Duplicated!");
        } else {
          //Debug.Log("Adding key '" + keys[j]+"', value: '"+ data_line[j]+"'");
          spreadsheet.data[i].Add(CleanValue(keys[j]), CleanValue(data_line[j]));
        }
      }
    }

    return spreadsheet;
  }

  public static string CleanValue(string key) {
    return key.Replace("\r", "").Replace("\n", "");
  }

  public static SpreadsheetData ReadTabSeparatedFile(TextAsset spreadsheet, int header_lines) {
    if(null == spreadsheet) {
      Debug.LogError("Spreadsheet not set!");
      return null;
    }
    return ReadTabSeparatedText(spreadsheet.text, header_lines);
  }

  public static SpreadsheetData ReadSpreadsheet(string name, int headerLines) {
    TextAsset text = Resources.Load(name) as TextAsset;
    return ReadTabSeparatedText(text.text, 1);
  }
}
