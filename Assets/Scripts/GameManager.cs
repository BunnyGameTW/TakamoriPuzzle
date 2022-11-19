using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SlidingPuzzle slidingPuzzle = null;
    
    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        int episodeId = DataManager.instance.episodeId;
        int levelId = DataManager.instance.levelId;
        string puzzleImagePath = ResManager.getPuzzleImagePath(episodeId, levelId);
        Sprite puzzleImage = ResManager.loadSprite(puzzleImagePath);

        slidingPuzzle.init(puzzleImage);
        slidingPuzzle.startPuzzle();
        slidingPuzzle.setFinishPuzzleCallback(handleFinishPuzzle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    private void handleFinishPuzzle() {
        // TODO: 完成謎題後的其他動作(顯示劇情+選項)
        Debug.Log("Show 劇本");
    }
}
