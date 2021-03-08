using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Esta clase se  encarga de parsear el json a una estructura manejable
/// por el sistema y retornarla.
/// En caso de error, retorna null.
/// </summary>
public class JsonParser
{
  private string path;

  public JsonParser(string jsonPath) {
    //path = Application.dataPath + "/StreamingAssets/JsonChallenge.json";
    path = jsonPath;
  }


  public Table Parse() {
    var exist = File.Exists(path);
    if (!exist) {
      Debug.LogError("Archivo no existe");
      return null;
    }
    string json = File.ReadAllText(path);

    JSONNode result = JSON.Parse(json);
    if (result == null) {
      Debug.LogError("Formato de Json incorrecto");
      return null;
    }

    var title = result["Title"];
    var headers = result["ColumnHeaders"].AsArray;
    var rows = result["Data"].AsArray;

    var table = new Table();

    table.title = title;

    for (int i = 0; i < headers.Count; i++) {
      table.tableContent[headers[i]] = new string[rows.Count];
    }

    for (int i = 0; i < rows.Count; i++) {
      for (int j = 0; j < headers.Count; j++) {
        var data = rows[i][j];
        var header = headers[j];
        table.tableContent[header][i] = data;
      }
    }
    return table;
  }
}

public class Table
{
  public string title;
  public Dictionary<string, string[]> tableContent;

  public Table() {
    title = "";
    tableContent = new Dictionary<string, string[]>();
  }
}