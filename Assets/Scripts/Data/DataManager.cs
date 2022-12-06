using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    跨場景用，共用資料(單例)
*/

// 遊戲資料(需儲存)
public class GameData
{
    public List<Vector2Int> unlockListKey;  // 解鎖清單-索引
    public List<bool> unlockListValue;      // 解鎖清單-值
    
    /** 建構子 */
    public GameData() {
        unlockListKey = new List<Vector2Int>();
        unlockListValue = new List<bool>();
    }
}

// 資料管理器
public class DataManager: Singleton<DataManager>
{
    public string language = "";    // 語言
    public int episodeId = 0;       // 章節編號
    public int levelId = 0;         // 關卡編號
    public int puzzleGridX = 0;     // 謎題格數X
    public int puzzleGridY = 0;     // 謎題格數Y
    private GameData gameData;      // 遊戲資料(用func呼叫+自動存檔)

    /** 建構子 */
    public DataManager() {
        language = "zh";
        episodeId = 1;
        levelId = 1;
        puzzleGridX = 3;
        puzzleGridY = 3;

        gameData = (GameData)SaveLoad.instance.loadData(typeof(GameData));
        if (gameData == null) {
            gameData = new GameData();
        }
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

