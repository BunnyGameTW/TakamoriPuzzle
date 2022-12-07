using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    資源清單，用於Resources.Load
*/

// 資源路徑
static class RES_PATH
{

    static public string PUZZLE_EXCEL = "excel/TAKAMORIPuzzleScripts.xlsx";
    static private string spritePath = "puzzle/";
    static public string SPRITE_PUZZLE_IMAGE = spritePath + "puzzle_";

    static RES_PATH() {
    }
}

// 資源管理器
static class ResManager
{
    // 讀取內部圖片Sprite
    static public Sprite loadSprite(string path) {
        return Resources.Load<Sprite>(path);
    }

    // TODO: 讀取內部音檔AudioClip

    /** 取得謎題圖片 */
    static public string getPuzzleImagePath(int episodeId, int levelId) {
        string path =  RES_PATH.SPRITE_PUZZLE_IMAGE;
        path = path + episodeId.ToString() + "_" + levelId.ToString();
        return path;
    }
}