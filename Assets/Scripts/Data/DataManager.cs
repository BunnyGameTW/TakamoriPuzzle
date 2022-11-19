using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    跨場景用，共用資料(單例)
*/

public class DataManager: Singleton<DataManager>
{
    public int episodeId = 0;   // 章節編號
    public int levelId = 0;     // 關卡編號

    /** 建構子 */
    public DataManager() {
        episodeId = 1;
        levelId = 1;
    }
}

