using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/*
<Application路徑相關說明>

*streamingAssetsPath: 存放影片的位置
    Windows & Mac: /專案路徑/Assets/StreamingAssets/
    Android: jar:file:///data/app/BundleId.apk/assets
    iOS: /private/var/mobile/Containers/Bundle/Application/隨機碼/AppName.app/Data/Raw

*persistentDataPath: 永久路徑
    Windows: /Users/UserName/AppData/LocalLow/CompanyName/AppName
    Mac: /Users/UserName/Library/Application Support/AppName/
    Android: data/data/BundleId/files

*temporaryCachePath: 快取路徑
    Windows: /Users/UserName/AppData/Local/Temp/CompanyName/AppName
    Mac: /var/folders/隨機路徑/CompanyName/AppName
    Android: /data/data/BundleId/cache
    iOS: /var/mobile/Containers/Data/Application/隨機碼/Library/Caches
*/

// 存檔種類
public enum SAVE_TYPE {
    STREAM_ASSETS,  // 專案資料夾 (跟專案同個位置)
    PERSISTENT,     // 永久資料夾 (系統不主動清掉)
    CACHE,          // 快取資料夾 (系統會主動清掉)
}

// 存讀檔物件
public class SaveLoad: Singleton<SaveLoad>
{
    private string folderPath = "/Save/";                   // 資料夾路徑
    private string fileName = "GameData.sav";               // 檔案
    private string filePath;                                // 完整路徑

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    /** 建構子 */
    public SaveLoad() {
        setPathType(SAVE_TYPE.PERSISTENT);
    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 設定存檔路徑種類 */
    public void setPathType(SAVE_TYPE state) {
        switch(state) {
            case SAVE_TYPE.STREAM_ASSETS: {
                folderPath = Application.streamingAssetsPath + folderPath;
            } break;
            case SAVE_TYPE.PERSISTENT: {
                folderPath = Application.persistentDataPath + folderPath;
            } break;
            case SAVE_TYPE.CACHE: {
                folderPath = Application.temporaryCachePath + folderPath;
            } break;
            default: {
                // folderPath = Application.dataPath + folderPath;
            } break;
        }
        filePath = folderPath + fileName;
    }

    /** 儲存檔案 */
    public void saveData(object content) {
        string content_string = serializeObject(content);
        createDirectory(folderPath);
        StreamWriter streamWriter = File.CreateText(filePath);
        streamWriter.Write(content_string);
        streamWriter.Close();
    }

    /** 讀取檔案 */
    public object loadData(Type dataType) {
        if (!File.Exists(filePath)) {
            return null;
        }
        StreamReader streamReader = File.OpenText(filePath);
        string data = streamReader.ReadToEnd();
        streamReader.Close();
        return deserializeObject(data, dataType);
    }
    
    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 創造資料夾 */
    private void createDirectory(string path) {
        if (File.Exists(path)) {
            return;
        }
        Directory.CreateDirectory(path);
    }

    /** 序列化物件(json) */
    private string serializeObject(object data) {
        return JsonUtility.ToJson(data);
    }

    /** 反序列化物件(json) */
    private static object deserializeObject(string data, Type dataType) {
        return JsonUtility.FromJson(data, dataType);
    }
}
