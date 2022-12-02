using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SlidingPuzzle slidingPuzzle = null;
    public DialogBox dialogBox = null;
    
    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        int episodeId = DataManager.instance.episodeId;
        int levelId = DataManager.instance.levelId;
        int puzzleGridX = DataManager.instance.puzzleGridX;
        int puzzleGridY = DataManager.instance.puzzleGridY;
        string puzzleImagePath = ResManager.getPuzzleImagePath(episodeId, levelId);
        Sprite puzzleImage = ResManager.loadSprite(puzzleImagePath);

        slidingPuzzle.init(puzzleImage, puzzleGridX, puzzleGridY);
        slidingPuzzle.startPuzzle();
        slidingPuzzle.setFinishPuzzleCallback(handleFinishPuzzle);
        dialogBox.init();
        // ---------------------------------------
        // TODO: 測試用
        string testText = "";
        testText = testText + "聖誕前夜，到處洋溢著歡樂的氣氛，但對TAKAMORI來説卻是例外<block>";
        testText = testText + "Calli和Kiara之間充滿古怪與尷尬的氣氛，但她們之前並沒有吵過架<block>";
        testText = testText + "Kiara最近總是很晚才回家，而且常常嘆氣，但當Calli試著跟她說話時\n她的反應卻總是一切都好<block>";
        testText = testText + "Calli告訴她自己他們之間不能再這樣了，<block>";
        testText = testText + "她擔心是因為自己太傲嬌讓Kiara忍受不了，想要離開她<block>";
        testText = testText + "Calli: 我想讓她開心起來，但是我該怎麼做？";
        dialogBox.setMessageData(testText);
        dialogBox.addSelectData("選項A", 1);
        dialogBox.addSelectData("選項B", 2);
        dialogBox.setSelectCallback((ID) => {
            Debug.Log("選擇開啟關卡:" + ID);
        });
        // ---------------------------------------
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    private void handleFinishPuzzle() {
        dialogBox.playMessage();
    }
}
