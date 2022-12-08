using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 遊戲資料(需儲存)
public class GameData
{
    public List<Vector2Int> unlockListKey;  // 解鎖清單-索引
    public List<bool> unlockListValue;      // 解鎖清單-值
    public float BGMVolime;         // 音樂音量
    public float SEVolime;          // 音效音量
    public SystemLanguage language; // 語言
    
    /** 建構子 */
    public GameData() {
        unlockListKey = new List<Vector2Int>();
        unlockListValue = new List<bool>();
        BGMVolime = 1.0f;
        SEVolime = 1.0f;

        switch(Application.systemLanguage) {
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified: {
                language = SystemLanguage.ChineseTraditional;
            } break;
            case SystemLanguage.Japanese: {
                language = SystemLanguage.Japanese;
            } break;
            default: {
                language = SystemLanguage.English;
            } break;
        }
    }
}

/**
    跨場景用，共用資料(單例)
*/

// 資料管理器
public class DataManager: Singleton<DataManager>
{
    public int episodeId = 0;       // 章節編號
    public int levelId = 0;         // 關卡編號
    public int puzzleGridX = 0;     // 謎題格數X
    public int puzzleGridY = 0;     // 謎題格數Y
    private GameData gameData;      // 遊戲資料(用func呼叫+自動存檔)

    /** 建構子 */
    public DataManager() {
        gameData = (GameData)SaveLoad.instance.loadData(typeof(GameData));
        if (gameData == null) {
            gameData = new GameData();
        }

        episodeId = 1;
        levelId = 1;
        puzzleGridX = 3;
        puzzleGridY = 3;
    }

    /** 設定音樂大小 */
    public void setBGMVolime(float value) {
        gameData.BGMVolime = value;
        SaveLoad.instance.saveData(gameData);
    }

    /** 取得音樂大小 */
    public float getBGMVolime() {
        return gameData.BGMVolime;
    }

    /** 設定音效大小 */
    public void setSEVolime(float value) {
        gameData.SEVolime = value;
        SaveLoad.instance.saveData(gameData);
    }

    /** 取得音效大小 */
    public float getSEVolime() {
        return gameData.SEVolime;
    }

    /** 設定語言 */
    public void setLanguage(SystemLanguage ID) {
        gameData.language = ID;
        SaveLoad.instance.saveData(gameData);
    }

    /** 取得語言 */
    public SystemLanguage getLanguage() {
        return gameData.language;
    }
    
    /** 取得語言字符 */
    public string getLanguageName() {
        string language = "en";
        switch(gameData.language) {
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified: {
                language = "zh";
            } break;
            case SystemLanguage.Japanese: {
                language = "jp";
            } break;
            default: {
                language = "en";
            } break;
        }
        return language;
    }

    /** 解鎖關卡 */
    public void unlockLevel(int episode, int level) {
        setUnlock(episode, level, true);
        SaveLoad.instance.saveData(gameData);
    }

    public void unlockLevel(int level) {
        unlockLevel(episodeId, level);
    }

    /** 取得是否解鎖 */
    public bool isUnlockLevel(int episode, int level) {
        return getUnlock(episode, level);
    }

    public bool isUnlockLevel(int level) {
        return isUnlockLevel(episodeId, level);
    }

    /** 取得已解鎖level清單 */
    public List<int> getUnlockLevelList(int episode) {
        List<int> list = new List<int>();
        int index = 0;
        foreach(Vector2Int item in gameData.unlockListKey) {
            if (item.x != episode) {
                continue;
            }
            if (gameData.unlockListValue[index]) {
                list.Add(item.y);
            }
            index++;
        }
        return list;
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 設定解鎖值 */
    private void setUnlock(int episode, int level, bool value) {
        Vector2Int key = new Vector2Int(episode, level);
        int index = gameData.unlockListKey.IndexOf(key);
        if (index == -1) {
            gameData.unlockListKey.Add(key);
            gameData.unlockListValue.Add(value);
        }
        else {
            gameData.unlockListValue[index] = value;
        }
    }

    /** 取得解鎖值 */
    private bool getUnlock(int episode, int level) {
        Vector2Int key = new Vector2Int(episode, level);
        int index = gameData.unlockListKey.IndexOf(key);
        if (index != -1) {
            return gameData.unlockListValue[index];
        }
        return false;
    }
}

