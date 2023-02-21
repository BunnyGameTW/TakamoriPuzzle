using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 謎題種類
public enum PuzzleType: int {
    SlidingPuzzle = 1,  // 九宮格滑動拼圖
    RotatingPuzzle = 2, // 旋轉拼圖
}

// 情境種類
public enum StoryType: int {
    XMas = 1,   // 聖誕節
}

// 語言code值
static class LANGUAGE_CODE {
    public const string ENGLISH = "en";
    public const string JAPANESE = "jp";
    public const string CHINESE = "zh";
}

// 配置
public class Config: Singleton<Config>{
    // 章節資料
    public struct EpisodeData {
        public int episodeId;           // 章節ID
        public AudioClip bgm;           // BGM
        public StoryType storyType;     // 情境種類
        public PuzzleType puzzleType;   // 謎題種類
        public Vector2Int puzzleGrid;   // 謎題格子數
    }
    readonly public Dictionary<int, EpisodeData> episodeList = new Dictionary<int, EpisodeData>();

    /** 建構子 */
    public Config() {
        episodeList[1] = new EpisodeData() {
            episodeId = 1,
            bgm = SoundManager.instance.BGM_xmas,
            storyType = StoryType.XMas,
            puzzleType = PuzzleType.SlidingPuzzle,
            puzzleGrid = new Vector2Int(3, 3),
        };
        episodeList[2] = new EpisodeData() {
            episodeId = 2,
            bgm = SoundManager.instance.BGM_xmas,
            storyType = StoryType.XMas,
            puzzleType = PuzzleType.RotatingPuzzle,
            puzzleGrid = new Vector2Int(3, 3),
        };
    }
}

// Excel相關
// static class EXCEL {
//     // 表單
//     static public class SHEET {
//         public const string content = "content";
//         public const string all = "all";
//         public const string episodeTitle = "episodeTitle";
//         public const string uiText = "uiText";
//     }
//     // content表單
//     static public class CONTENT {
//         public const string id = "id";
//         public const string _story = "_story";
//         public const string _chioce_ = "_chioce_";
//     }
//     // all表單
//     static public class ALL {
//         public const string episodeId = "episodeId";
//         public const string levelId = "levelId";
//         public const string contentId = "contentId";
//         public const string unlockLevelId = "unlockLevelId";
//         public const string chioceId_ = "chioceId_";
//     }
//     // episodeTitle表單
//     static public class EPISODE_TITLE {
//         public const string id = "id";
//         public const string _title = "_title";
//         public const string sortId = "sortId";
//         public const string isLock = "isLock";
//     }
//     // uiText表單
//     static public class UI_TEXT {
//         public const string id = "id";
//         public const string name = "name";
//         public const string story = "story";
//         public const string bgm = "bgm";
//         public const string se = "se";
//         public const string setting = "setting";
//         public const string language = "language";
//         public const string credit = "credit";
//         public const string programming = "programming";
//         public const string art = "art";
//         public const string translation = "translation";
//         public const string skipPuzzle = "skipPuzzle";
//         public const string storyAuto = "storyAuto";
//         public const string reset = "reset";
//         public const string unlockCG = "unlockCG";
//     }
// }
