using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPuzzleTile : MonoBehaviour
{
    private Vector2Int goalGridPos = new Vector2Int();    // 目標格子位置
	private Vector2Int nowGridPos = new Vector2Int();     // 目前格子位置
    private bool isCorrectGrid = false;             // 是否在正確的位置
    private bool isGameActive = false;              // 是否正在遊戲中
    
    // 生命週期 --------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        isCorrectGrid = false;
        isGameActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameActive) {
            return;
        }
        isCorrectGrid = checkGridCorrect();
    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 設定目標格子位置 */
    public void setGoalGridPos(Vector2Int pos) {
        goalGridPos = pos;
    }

    /** 設定目前格子位置 */
    public void setNowGridPos(Vector2Int pos) {
        nowGridPos = pos;
    }

    /** 取得目前格子位置 */
    public Vector2Int getNowGridPos() {
        return nowGridPos;
    }

    /** 設定正在遊戲中 */
    public void setGameActive(bool isActive) {
        isGameActive = isActive;
    }

    /** 取得正在遊戲中 */
    public bool getGameActive() {
        return isGameActive;
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 檢查格子是否在正確的位置 */
    private bool checkGridCorrect() {
        return (nowGridPos == goalGridPos);
    }
}
