using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

// 語言code值
static class LANGUAGE_CODE {
    public const string ENGLISH = "en";
    public const string JAPANESE = "jp";
    public const string CHINESE = "zh";
}

// 遊戲資料(需儲存)
public class GameData
{
    public int episodeId;           // 最後章節編號
    public int levelId;             // 最後關卡編號
    public List<Vector2Int> unlockListKey;  // 解鎖清單-索引
    public List<bool> unlockListValue;      // 解鎖清單-值
    public float BGMVolime;         // 音樂音量
    public float SEVolime;          // 音效音量
    public SystemLanguage language; // 語言
    public bool skipPassPuzzle;     // 跳過已解謎題
    public bool autoPlayDialog;     // 自動播放對話
    
    /** 建構子 */
    public GameData() {
        episodeId = 0;
        levelId = 0;
        unlockListKey = new List<Vector2Int>();
        unlockListValue = new List<bool>();
        BGMVolime = 1.0f;
        SEVolime = 1.0f;
        skipPassPuzzle = false;
        autoPlayDialog = false;

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
    public int puzzleGridX = 0;     // 謎題格數X
    public int puzzleGridY = 0;     // 謎題格數Y
    private GameData gameData;      // 遊戲資料(用func呼叫+自動存檔)
    public event System.EventHandler<string> LanguageChanged;

    /** 建構子 */
    public DataManager() {
        gameData = (GameData)SaveLoad.instance.loadData(typeof(GameData));
        if (gameData == null) {
            gameData = new GameData();
        }

        puzzleGridX = 3;
        puzzleGridY = 3;

        renewLocalization(getLanguageName());
    }

    /** 最後選擇的章節 */
    public int episodeId {
        get { return gameData.episodeId; }
        set {
            gameData.episodeId = value;
            SaveLoad.instance.saveData(gameData);
        }
    }

    /** 最後選擇的關卡 */
    public int levelId {
        get { return gameData.levelId; }
        set {
            gameData.levelId = value;
            SaveLoad.instance.saveData(gameData);
        }
    }

    /** 音樂音量 */
    public float BGMVolime {
        get { return gameData.BGMVolime; }
        set {
            gameData.BGMVolime = value;
            SaveLoad.instance.saveData(gameData);
        }
    }

    /** 音效音量 */
    public float SEVolime {
        get { return gameData.SEVolime; }
        set {
            gameData.SEVolime = value;
            SaveLoad.instance.saveData(gameData);
        }
    }

    /** 語言 */
    public SystemLanguage language {
        get { return gameData.language; }
        set {
            gameData.language = value;
            SaveLoad.instance.saveData(gameData);
        }
    }

    /** 跳過已解謎題 */
    public bool skipPassPuzzle {
        get { return gameData.skipPassPuzzle; }
        set {
            gameData.skipPassPuzzle = value;
            SaveLoad.instance.saveData(gameData);
        }
    }

    /** 自動播放對話 */
    public bool autoPlayDialog {
        get { return gameData.autoPlayDialog; }
        set {
            gameData.autoPlayDialog = value;
            SaveLoad.instance.saveData(gameData);
        }
    }

    /** Localization語言處理 */
    public void renewLocalization(string languageCode) {
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (languageCode == locale.Identifier.Code) {
                LocalizationSettings.SelectedLocale = locale;
            }
        }
        LanguageChanged?.Invoke(this, languageCode);
    }

    /** 取得語言字符 */
    public string getLanguageName(SystemLanguage languageValue) {
        string language = LANGUAGE_CODE.ENGLISH;
        switch(languageValue) {
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified: {
                language = LANGUAGE_CODE.CHINESE;
            } break;
            case SystemLanguage.Japanese: {
                language = LANGUAGE_CODE.JAPANESE;
            } break;
            default: {
                language = LANGUAGE_CODE.ENGLISH;
            } break;
        }
        return language;
    }

    public string getLanguageName() {
        return getLanguageName(gameData.language);
    }

    /** 解鎖關卡 */
    public void unlockLevel(int episode, int level) {
        setUnlock(episode, level, true);
        SaveLoad.instance.saveData(gameData);
    }

    public void unlockLevel(int level) {
        unlockLevel(gameData.episodeId, level);
    }

    /** 取得是否解鎖 */
    public bool isUnlockLevel(int episode, int level) {
        return getUnlock(episode, level);
    }

    public bool isUnlockLevel(int level) {
        return isUnlockLevel(gameData.episodeId, level);
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

