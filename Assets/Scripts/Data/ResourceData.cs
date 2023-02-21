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

    // 讀取內部音檔AudioClip
    static public AudioClip loadAudioClip(string path) {
        return Resources.Load<AudioClip>(path);
    }

    // 非同步讀內部圖片Sprite
    static public IEnumerator asyncLoadSprite(string path, System.Action<Sprite> callback = null) {
        ResourceRequest resRequest = Resources.LoadAsync<Sprite>(path);
        while(!resRequest.isDone) {
            yield return null;
        }
        if (callback != null) {
            Sprite sprite = resRequest.asset as Sprite;
            callback(sprite);
        }
    }

    // 非同步讀內部音檔AudioClip
    static public IEnumerator asyncLoadAudioClip(string path, System.Action<AudioClip> callback = null) {
        ResourceRequest resRequest = Resources.LoadAsync<AudioClip>(path);
        while(!resRequest.isDone) {
            yield return null;
        }
        if (callback != null) {
            AudioClip audioClip = resRequest.asset as AudioClip;
            callback(audioClip);
        }
    }

    /** 取得謎題圖片 */
    static public string getPuzzleImagePath(int episodeId, int levelId) {
        string path =  RES_PATH.SPRITE_PUZZLE_IMAGE;
        path = path + episodeId.ToString() + "_" + levelId.ToString();
        return path;
    }
}
