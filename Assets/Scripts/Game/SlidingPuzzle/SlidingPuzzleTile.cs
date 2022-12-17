using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlidingPuzzleTile : MonoBehaviour
{
    private Vector2Int goalGridPos = new Vector2Int();  // 目標格子位置
	private Vector2Int nowGridPos = new Vector2Int();   // 目前格子位置
    Sequence tweener = null;    // 補間事件
    Vector3 baseScale;
    Color baseColor;
    
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
        baseScale = this.transform.localScale;
        baseColor = this.GetComponent<SpriteRenderer>().material.color;
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

    /** 取得目標格子位置 */
    public Vector2Int getGoalGridPos() {
        return goalGridPos;
    }

    /** 檢查格子是否在正確的位置 */
    public bool checkGridCorrect() {
        return (nowGridPos == goalGridPos);
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

    /** 設定目標格子位置 */
    private void setGoalGridPos(Vector2Int pos) {
        goalGridPos = pos;
    }

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
            tweener.Append(DOTween.To(
                () => { return this.transform.localScale; },
                (value) => { this.transform.localScale = value; },
                baseScale * 1.15f,
                0.1f
            ).SetEase(Ease.Linear));
            tweener.Join(DOTween.To(
                () => { return this.GetComponent<SpriteRenderer>().material.color; },
                (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
                baseColor * 1.1f,
                0.1f
            ).SetEase(Ease.InQuart));
            tweener.Append(DOTween.To(
                () => { return this.transform.localScale; },
                (value) => { this.transform.localScale = value; },
                baseScale * 0.85f,
                0.1f
            ).SetEase(Ease.InQuart));
            tweener.AppendInterval(0.05f);
            tweener.Join(DOTween.To(
                () => { return this.GetComponent<SpriteRenderer>().material.color; },
                (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
                baseColor,
                0.05f
            ).SetEase(Ease.Linear));
            tweener.Append(DOTween.To(
                () => { return this.transform.localScale; },
                (value) => { this.transform.localScale = value; },
                baseScale,
                0.05f
            ).SetEase(Ease.Linear));
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
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale * 1.15f,
            0.1f
        ).SetEase(Ease.Linear));
        tweener.Join(DOTween.To(
            () => { return this.GetComponent<SpriteRenderer>().material.color; },
            (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
            baseColor * 1.1f,
            0.1f
        ).SetEase(Ease.InQuart));
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale * 0.85f,
            0.1f
        ).SetEase(Ease.InQuart));
        tweener.AppendInterval(0.05f);
        tweener.Join(DOTween.To(
            () => { return this.GetComponent<SpriteRenderer>().material.color; },
            (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
            baseColor,
            0.05f
        ).SetEase(Ease.Linear));
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale,
            0.05f
        ).SetEase(Ease.Linear));
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
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale * 1.05f,
            0.05f
        ).SetEase(Ease.Linear));
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale * 0.95f,
            0.1f
        ).SetEase(Ease.InQuart));
        tweener.Join(DOTween.To(
            () => { return this.GetComponent<SpriteRenderer>().material.color; },
            (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
            baseColor * 1.1f,
            0.1f
        ).SetEase(Ease.Linear));
        tweener.AppendInterval(0.1f);
        tweener.Join(DOTween.To(
            () => { return this.GetComponent<SpriteRenderer>().material.color; },
            (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
            baseColor,
            0.1f
        ).SetEase(Ease.Linear));
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale,
            0.05f
        ).SetEase(Ease.Linear));
        tweener.AppendCallback(() => {
            tweener = null;
            if (callback != null) {
                callback();
            }
        });
        tweener.Play();
}
}
