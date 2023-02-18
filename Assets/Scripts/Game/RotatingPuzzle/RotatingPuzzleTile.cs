using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotatingPuzzleTile : BasePuzzleGridTile
{
    private float goalGridAngle = 0;  // 目標格子角度
	private float nowGridAngle = 0;   // 目前格子角度
    Sequence tweener = null;        // 補間事件
    
    // 生命週期 --------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnDestroy() {
        if (tweener != null) {
            tweener.Kill();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 初始化
    public override void init(Sprite sprite, Vector2 boxColliderSize) {
        base.init(sprite, boxColliderSize);
        goalGridAngle = 0;
        nowGridAngle = 0;
    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 檢查格子是否正確 */
    public override bool checkTileCorrect() {
        return ((int)goalGridAngle == (int)nowGridAngle);
    }

    /** 設定角度 */
    public void setTileAngle(float angle) {
        nowGridAngle = angle % 360;
        this.transform.localRotation = Quaternion.Euler(0, 0, nowGridAngle);
    }

    /** 取得角度 */
    public float getTileAngle() {
        return nowGridAngle;
    }

    /** 執行旋轉90度 */
    public void runRotateTile(System.Action callback = null) {
        setTileAngle(getTileAngle() + 90);
        // if (tweener != null) {
        // }
        if (callback != null) {
            callback();
        }
    }

    /** 執行閃耀效果 */
    public void runShineEffect(float delay, System.Action callback = null) {
        if (tweener != null) {
            this.transform.localScale = baseScale;
            this.GetComponent<SpriteRenderer>().material.color = baseColor;
            tweener.Kill();
        }
        handleShineEffect(delay, callback);
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 處理閃耀動畫 */
    private void handleShineEffect(float delay = 0f, System.Action callback = null) {
        tweener = DOTween.Sequence();
        tweener.AppendInterval(delay);
        tweener = addPuzzleFinishTween(tweener);
        tweener.AppendCallback(() => {
            tweener = null;
            if (callback != null) {
                callback();
            }
        });
        tweener.Play();
    }
}
