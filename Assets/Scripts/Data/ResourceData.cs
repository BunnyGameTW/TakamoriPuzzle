using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    資源清單，用於Resources.Load
*/

// 資源資料
static class RES_DATA
{
    static private string spritePath = "";
    static public Dictionary<Vector2Int, string> SPRITE_PUZZLE_IMAGE = new Dictionary<Vector2Int, string>();

    static RES_DATA() {
        SPRITE_PUZZLE_IMAGE.Add(new Vector2Int(1, 1), spritePath + "xmas_1");
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
        return RES_DATA.SPRITE_PUZZLE_IMAGE[new Vector2Int(episodeId, levelId)];
    }
}
