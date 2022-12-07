using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelDataReader;
using System.IO;

/**
    跨場景用，共用讀取excel資料(單例)
*/

public class LoadExcel: Singleton<LoadExcel>
{
    const string arrayTag = "[]";   // 陣列標記
    private string firstSheet = ""; // 第一個表單頁簽名稱
    private Dictionary<string, Dictionary<string, Hashtable>> data = null;// excel原始資料

    /** 建構子 */
    public LoadExcel() {
    }
    
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 讀取excel檔案 */
    public void loadFile(string path) {
        path = Application.dataPath + "/Resources/" + path;
        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
        IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
        System.Data.DataSet result = reader.AsDataSet();
        if (result.Tables.Count <= 0) {
            Debug.LogError("Error: 表單讀取失敗");
            return;
        }
        parseExcelData(result);
        stream.Close();
        reader.Close();
    }

    /** 取得表單 */
    public Dictionary<string, Hashtable> getTable(string sheet) {
        return data[sheet];
    }

    /** 取得物件 */
    public Hashtable getObject(string sheet, string IDkey, string ID) {
        foreach(KeyValuePair<string, Hashtable> item in data[sheet]) {
            foreach(DictionaryEntry data in item.Value) {
                if ((string)data.Key == IDkey
                && (string)data.Value == ID) {
                    return item.Value;
                }
            }
        }
        return null;
    }

    public Hashtable getObject(string sheet, string IDkey, int ID) {
        return getObject(sheet, IDkey, ID.ToString());
    }

    public Hashtable getObject(string IDkey, string ID) {
        return getObject(firstSheet, IDkey, ID);
    }

    public Hashtable getObject(string IDkey, int ID) {
        return getObject(firstSheet, IDkey, ID);
    }

    /** 取得物件數值 */
    public string getObjectValue(string sheet, string IDkey, string ID, string key) {
        foreach(KeyValuePair<string, Hashtable> item in data[sheet]) {
            foreach(DictionaryEntry data in item.Value) {
                if ((string)data.Key == IDkey
                && (string)data.Value == ID) {
                    return (string)item.Value[key];
                }
            }
        }
        return null;
    }

    public string getObjectValue(string sheet, string IDkey, int ID, string key) {
        return getObjectValue(sheet, IDkey, ID.ToString(), key);
    }

    public string getObjectValue(string IDkey, string ID, string key) {
        return getObjectValue(firstSheet, IDkey, ID, key);
    }

    public string getObjectValue(string IDkey, int ID, string key) {
        return getObjectValue(firstSheet, IDkey, ID, key);
    }

    /** 取得物件清單 */
    public List<string> getObjectList(string sheet, string IDkey, string ID, string key) {
        foreach(KeyValuePair<string, Hashtable> item in data[sheet]) {
            foreach(DictionaryEntry data in item.Value) {
                if ((string)data.Key == IDkey
                && (string)data.Value == ID) {
                    List<string> list = new List<string>();
                    Hashtable keyList = (Hashtable)item.Value[key];
                    foreach(DictionaryEntry listItem in keyList) {
                        list.Add((string)listItem.Value);
                    }
                    return list;
                }
            }
        }
        return null;
    }
    
    public List<string> getObjectList(string sheet, string IDkey, int ID, string key) {
        return getObjectList(sheet, IDkey, ID.ToString(), key);
    }

    public List<string> getObjectList(string IDkey, string ID, string key) {
        return getObjectList(firstSheet, IDkey, ID, key);
    }

    public List<string> getObjectList(string IDkey, int ID, string key) {
        return getObjectList(firstSheet, IDkey, ID, key);
    }

    /** 取得某行的值 */
    public string getValue(string sheet, int index, string key) {
        return (string)data[sheet][index.ToString()][key];
    }

    public string getValue(int index, string key) {
        return getValue(firstSheet, index, key);
    }

    /** 取得某行的清單 */
    public List<string> getList(string sheet, int index, string key) {
        Hashtable keyList = (Hashtable)data[sheet][index.ToString()][key];
        List<string> list = new List<string>();
        foreach(DictionaryEntry item in keyList) {
            list.Add((string)item.Value);
        }
        return list;
    }

    public List<string> getList(int index, string key) {
        return getList(firstSheet, index, key);
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 解析excel檔案 */
    private void parseExcelData(System.Data.DataSet excelData) {
        data = new Dictionary<string, Dictionary<string, Hashtable>>();
        firstSheet = excelData.Tables[0].TableName;
        for(int i = 0; i < excelData.Tables.Count; i++) {
            var table = excelData.Tables[i];
            Dictionary<string, Hashtable> tableData = new Dictionary<string, Hashtable>();
            data.Add(table.TableName, tableData);
            for(int j = 1; j < table.Rows.Count; j++) {
                Hashtable contentData = new Hashtable();
                tableData.Add(j.ToString(), contentData);
                for(int k = 0; k < table.Columns.Count; k++) {
                    string key = table.Rows[0][k].ToString();
                    int index = key.IndexOf(arrayTag);
                    if (index > 0) {
                        key = key.Substring(0, index);
                        if (!contentData.ContainsKey(key)) {
                            Hashtable rowsList = new Hashtable();
                            contentData.Add(key, rowsList);
                        }
                        Hashtable keyList = (Hashtable)contentData[key];
                        keyList.Add(keyList.Count, table.Rows[j][k].ToString());
                    }
                    else if (!contentData.ContainsKey(key)) {
                        contentData.Add(key, table.Rows[j][k].ToString());
                    }
                    else {
                        Debug.LogError("Error: 表單" + table.TableName + ">" + key + "key重複");
                    }
                }
            }
        }
    }
}