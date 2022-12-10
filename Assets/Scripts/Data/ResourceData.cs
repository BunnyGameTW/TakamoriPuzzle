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

    static private string bgmPath = "Music/BGM/";
    static private string sePath = "Music/SE/";
    static public class BGM {
        static public string TITLE = bgmPath + "bgm_title";
        static public string XMAS = bgmPath + "bgm_xmas";
    }
    static public class SE {
        static public string FINISH = sePath + "finish";
        static public string PAGE = sePath + "page";
        static public string PAGE_2 = sePath + "page_2";
        static public string PUZZLE_1 = sePath + "puzzle_1";
        static public string PUZZLE_2 = sePath + "puzzle_2";
        static public string PUZZLE_3 = sePath + "puzzle_3";
        static public string PUZZLE_4 = sePath + "puzzle_4";
        static public string TRANSITION_IN = sePath + "transition_in";
        static public string TRANSITION_OUT = sePath + "transition_out";
    }

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

    // 讀取內部音檔AudioClip
    static public AudioClip loadAudioClip(string path) {
        return Resources.Load<AudioClip>(path);
    }

    /** 取得謎題圖片 */
    static public string getPuzzleImagePath(int episodeId, int levelId) {
        string path =  RES_PATH.SPRITE_PUZZLE_IMAGE;
        path = path + episodeId.ToString() + "_" + levelId.ToString();
        return path;
    }
}
