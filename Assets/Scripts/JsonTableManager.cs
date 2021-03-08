using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JsonTableManager : MonoBehaviour
{
  // Start is called before the first frame update
  Transform tableContainer;
  Transform firstColumnRef;
  Transform background;
  Transform title;
  float initTopHeight;

  const int Y_OFFSET = 30;
  const int WIDTH_INCREMENT = 75;
  const int HEIGHT_INCREMENT = 25;
  const int MAX_INITIAL_COLUMNS = 4;
  const int DEFAULT_WIDTH = 400;
  const int DEFAULT_HEIGHT = 350;

  private void Awake() {
    tableContainer = GameObject.Find("JsonTable").transform;
    firstColumnRef = GameObject.Find("FirstColumnRef").transform;
    background = GameObject.Find("Backgroung").transform;
    title = GameObject.Find("TitleText").transform;
    firstColumnRef.gameObject.SetActive(false);
  }

  void Start() {
    LoadTable();
  }

  // LLama al JsonParser para obtener una tabla
  void LoadTable() {
    string path = Application.dataPath + "/StreamingAssets/JsonChallenge.json";
    var parser = new JsonParser(path);
    var dataTable = parser.Parse();

    if (dataTable == null) {
      Debug.LogError("Error al parsear el archivo");
    }

    initTopHeight = background.GetComponent<RectTransform>().sizeDelta.y / 2;
    BuilTabledUi(dataTable);
  }

  // Resetea la UI para recargar la tabla
  public void ResetTable() {

    GameObject.Destroy(GameObject.Find("Columns"));
    var backgroundTransform = background.GetComponent<RectTransform>();
    backgroundTransform.sizeDelta = new Vector2(DEFAULT_WIDTH, DEFAULT_HEIGHT);
    backgroundTransform.anchoredPosition = new Vector2(0, 0);

    LoadTable();
  }

  // Construye los elementos que conforman la tabla a partir de una estructura
  // que la define.
  void BuilTabledUi(Table dataTable) {

    Font ArialFont = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

    float previousColumnWidth = 0;
    int i = 0;

    title.GetComponent<Text>().text = dataTable.title;
    ResiseBackgroundwidth(dataTable.tableContent.Count);

    var columns = new GameObject();
    columns.AddComponent<RectTransform>();
    columns.transform.SetParent(tableContainer.transform);
    columns.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    columns.name = "Columns";

    foreach (var entry in dataTable.tableContent) {

      var h = entry.Key;
      var values = entry.Value;

      var header = new GameObject();
      header.AddComponent<RectTransform>();
      header.transform.SetParent(columns.transform);

      var x = firstColumnRef.GetComponent<RectTransform>().anchoredPosition.x;
      var y = firstColumnRef.GetComponent<RectTransform>().anchoredPosition.y;
      var deltaX = x + (previousColumnWidth * i);
      var headerRectTransform = header.GetComponent<RectTransform>();
      headerRectTransform.anchoredPosition = new Vector2(deltaX, y);

      var headerText = header.AddComponent<Text>();
      headerText.name = h;
      headerText.color = Color.white;
      headerText.font = ArialFont;
      headerText.text = h;
      headerText.fontSize = 14;
      headerText.fontStyle = FontStyle.Bold;
      headerText.alignment = TextAnchor.MiddleCenter;

      int j = 1;
      foreach (var v in values) {
        var data = new GameObject();
        data.AddComponent<RectTransform>();
        data.transform.SetParent(header.transform);
        data.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Y_OFFSET * -j);

        var dataText = data.AddComponent<Text>();
        dataText.name = v;
        dataText.color = Color.white;
        dataText.font = ArialFont;
        dataText.text = v;
        dataText.alignment = TextAnchor.MiddleCenter;

        if (data.GetComponent<RectTransform>().anchoredPosition.y < -background.GetComponent<RectTransform>().rect.height / 2) {
          ResiseBackgroundHeight();
        }

        j++;
      }

      previousColumnWidth = header.GetComponent<RectTransform>().rect.width;
      i++;
    }
  }

  /// <summary>
  /// Redimensiona el ancho del background en funcion de las columnas
  /// </summary>
  /// <param name="initLeftX"></param>
  void ResiseBackgroundwidth(int columnCount) {
    if (columnCount <= MAX_INITIAL_COLUMNS) {
      return;
    }

    var backgroundTransform = background.GetComponent<RectTransform>();
    var initLeftWidth = backgroundTransform.sizeDelta.x / 2;
    var newWidth = backgroundTransform.rect.width + WIDTH_INCREMENT * columnCount;

    var posY = backgroundTransform.anchoredPosition.y;

    backgroundTransform.sizeDelta = new Vector2(newWidth, backgroundTransform.rect.height);
    backgroundTransform.anchoredPosition = new Vector2(backgroundTransform.sizeDelta.x / 2 - initLeftWidth, posY);
  }

  //Redimensiona la altura del background cuando las filas se salen del margen
  void ResiseBackgroundHeight() {
    var backgroundTransform = background.GetComponent<RectTransform>();
    var newHeith = backgroundTransform.rect.height + HEIGHT_INCREMENT;
    var posX = backgroundTransform.anchoredPosition.x;

    backgroundTransform.sizeDelta = new Vector2(backgroundTransform.rect.width, newHeith);
    backgroundTransform.anchoredPosition = new Vector2(posX, -backgroundTransform.sizeDelta.y / 2 + initTopHeight);
  }
}
