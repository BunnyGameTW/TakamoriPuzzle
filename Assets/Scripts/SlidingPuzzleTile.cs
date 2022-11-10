using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPuzzleTile : MonoBehaviour
{
    private Vector2Int goalGridPos = new Vector2Int();    // 目標格子位置
	private Vector2Int nowGridPos = new Vector2Int();     // 目前格子位置
    
    // 生命週期 --------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 初始化
    public void init(Vector2Int gridPos, Sprite sprite, Vector2 boxColliderSize) {
        SpriteRenderer tmepSpriteRenderer = this.GetComponent<SpriteRenderer>();
        BoxCollider2D tmepBoxCollider2D = this.GetComponent<BoxCollider2D>();

        tmepSpriteRenderer.sprite = sprite;
        tmepBoxCollider2D.size = boxColliderSize;
        setGoalGridPos(gridPos);
        setNowGridPos(gridPos);
    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 設定目前格子位置 */
    public void setNowGridPos(Vector2Int pos) {
        nowGridPos = pos;
    }

    /** 取得目前格子位置 */
    public Vector2Int getNowGridPos() {
        return nowGridPos;
    }

    /** 檢查格子是否在正確的位置 */
    public bool checkGridCorrect() {
        return (nowGridPos == goalGridPos);
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 設定目標格子位置 */
    private void setGoalGridPos(Vector2Int pos) {
        goalGridPos = pos;
    }
}
