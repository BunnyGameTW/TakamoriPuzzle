using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    跨場景用，共用資料(單例)
*/

public class DataManager: Singleton<DataManager>
{
    public string language = "";    // 語言
    public int episodeId = 0;       // 章節編號
    public int levelId = 0;         // 關卡編號
    public int puzzleGridX = 0;     // 謎題格數X
    public int puzzleGridY = 0;     // 謎題格數Y

    /** 建構子 */
    public DataManager() {
        language = "zh";
        episodeId = 1;
        levelId = 1;
        puzzleGridX = 3;
        puzzleGridY = 3;
    }
}

