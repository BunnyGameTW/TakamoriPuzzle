using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlidingPuzzleTile : BasePuzzleGridTile
{
    private Vector2Int goalGridPos = new Vector2Int();  // 目標格子位置
	private Vector2Int nowGridPos = new Vector2Int();   // 目前格子位置
    Sequence tweener = null;    // 補間事件
    
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

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 檢查格子是否正確 */
    public override bool checkTileCorrect() {
        return (nowGridPos == goalGridPos);
    }

    /** 設定目前格子位置 */
    public void setNowGridPos(Vector2Int pos) {
        nowGridPos = pos;
    }

    /** 取得目前格子位置 */
    public Vector2Int getNowGridPos() {
        return nowGridPos;
    }

    /** 設定目標格子位置 */
    public void setGoalGridPos(Vector2Int pos) {
        goalGridPos = pos;
    }

    /** 取得目標格子位置 */
    public Vector2Int getGoalGridPos() {
        return goalGridPos;
    }

    /** 執行補間位移 */
    public void runMoveEffect(Vector3 targetPos, bool extraEffect = false, System.Action callback = null) {
        if (tweener != null) {
            this.transform.localScale = baseScale;
            this.GetComponent<SpriteRenderer>().material.color = baseColor;
            tweener.Kill();
        }
        handleMoveEffect(targetPos, extraEffect, callback);
    }

    /** 執行淡入 */
    public void runFadeInEffect(System.Action callback = null) {
        if (tweener != null) {
            this.transform.localScale = baseScale;
            this.GetComponent<SpriteRenderer>().material.color = baseColor;
            tweener.Kill();
        }
        handleFadeInEffect(callback);
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

    /** 處理位移動畫 */
    private void handleMoveEffect(Vector3 targetPos, bool extraEffect = false, System.Action callback = null) {
        tweener = DOTween.Sequence();
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale * 1.05f,
            0.1f
        ).SetEase(Ease.Linear));
        tweener.Append(DOTween.To(
            () => { return this.transform.localPosition; },
            (value) => { this.transform.localPosition = value; },
            targetPos,
            0.2f
        ).SetEase(Ease.OutCubic));
        if (extraEffect == true) {
            tweener = addTileCorrectTween(tweener);
        }
        else {
            tweener.Append(DOTween.To(
                () => { return this.transform.localScale; },
                (value) => { this.transform.localScale = value; },
                baseScale,
                0.1f
            ).SetEase(Ease.Linear));
        }
        tweener.AppendCallback(() => {
            this.transform.localScale = baseScale;
            this.GetComponent<SpriteRenderer>().material.color = baseColor;
            this.transform.localPosition = targetPos;
            tweener = null;
            if (callback != null) {
                callback();
            }
        });
        tweener.Play();
    }

    /** 處理淡入動畫 */
    private void handleFadeInEffect(System.Action callback = null) {
        this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        tweener = DOTween.Sequence();
        tweener.AppendInterval(0.2f);
        tweener.AppendCallback(() => {
            this.gameObject.SetActive(true);
        });
        tweener.Append(DOTween.To(
            () => { return this.GetComponent<SpriteRenderer>().color; },
            (value) => { this.GetComponent<SpriteRenderer>().color = value; },
            new Color(1, 1, 1, 1),
            0.5f
        ).SetEase(Ease.InOutQuart));
        tweener = addTileCorrectTween(tweener);
        tweener.AppendCallback(() => {
            SoundManager.instance.playSE(
                    SoundManager.instance.SE_puzzles[Random.Range(0, SoundManager.instance.SE_puzzles.Length)]);
        });
        tweener.AppendInterval(0.66f);
        tweener.AppendCallback(() => {
            tweener = null;
            if (callback != null) {
                callback();
            }
        });
        tweener.Play();
    }

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
