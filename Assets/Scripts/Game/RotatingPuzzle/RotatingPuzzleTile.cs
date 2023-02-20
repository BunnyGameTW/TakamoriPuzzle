using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotatingPuzzleTile : BasePuzzleGridTile
{
    private float goalGridAngle = 0;    // 目標格子角度
	private float nowGridAngle = 0;     // 目前格子角度
    Sequence tweener = null;            // 補間事件
    
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

    /** 清除動作 */
    public void clearTweener() {
        if (tweener != null) {
            this.transform.localScale = baseScale;
            setTileAngle(nowGridAngle);
            this.GetComponent<SpriteRenderer>().material.color = baseColor;
            tweener.Kill();
        }
    }

    /** 執行旋轉90度 */
    public void runRotateTile(float angle, System.Action callback = null) {
        bool isTweener = false;
        if (tweener != null) {
            tweener.Kill();
            isTweener = true;
        }
        nowGridAngle = nowGridAngle + angle;
        tweener = DOTween.Sequence();
        if (isTweener) {
            this.transform.localScale = baseScale * 1.1f;
            this.GetComponent<SpriteRenderer>().material.color = baseColor * 0.95f;
        }
        else {
            tweener.Append(DOTween.To(
                () => { return this.transform.localScale; },
                (value) => { this.transform.localScale = value; },
                baseScale * 1.1f,
                0.1f
            ).SetEase(Ease.Linear));
            tweener.Join(DOTween.To(
                () => { return this.GetComponent<SpriteRenderer>().material.color; },
                (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
                baseColor * 0.95f,
                0.1f
            ).SetEase(Ease.Linear));
        }
        tweener.Append(DOTween.To(
            () => { return this.transform.localRotation; },
            (value) => { this.transform.localRotation = value; },
            new Vector3(0, 0, nowGridAngle),
            0.2f
        ).SetEase(Ease.OutCubic));
        tweener.AppendCallback(() => {
            tweener = null;
            setTileAngle(getTileAngle());
            if (callback != null) {
                callback();
            }
        });
        tweener.Play();
    }

    /** 執行格子正確效果 */
    public void runCorrectEffect(System.Action callback = null) {
        if (tweener != null) {
            this.transform.localScale = baseScale;
            this.transform.localRotation = baseRotation;
            this.GetComponent<SpriteRenderer>().material.color = baseColor;
            tweener.Kill();
        }
        tweener = DOTween.Sequence();
        tweener = addTileCorrectTween(tweener);
        tweener.AppendCallback(() => {
            tweener = null;
            if (callback != null) {
                callback();
            }
        });
        tweener.Play();
    }

    /** 執行格子錯誤效果 */
    public void runMistakeEffect(System.Action callback = null) {
        if (tweener != null) {
            this.transform.localScale = baseScale;
            this.transform.localRotation = baseRotation;
            this.GetComponent<SpriteRenderer>().material.color = baseColor;
            tweener.Kill();
        }
        tweener = DOTween.Sequence();
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale,
            0.1f
        ).SetEase(Ease.Linear));
        tweener.Join(DOTween.To(
            () => { return this.GetComponent<SpriteRenderer>().material.color; },
            (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
            baseColor,
            0.1f
        ).SetEase(Ease.Linear));
        tweener.AppendCallback(() => {
            tweener = null;
            if (callback != null) {
                callback();
            }
        });
        tweener.Play();
    }

    /** 執行洗牌效果 */
    public void runJuggleEffect(System.Action callback = null) {
        int JUGGLE_COUNT = 4;
        if (tweener != null) {
            tweener.Kill();
        }
        tweener = DOTween.Sequence();
        tweener.AppendInterval(0.02f * UnityEngine.Random.Range(0, 7));
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale * 1.1f,
            0.1f
        ).SetEase(Ease.Linear));
        tweener.Join(DOTween.To(
            () => { return this.GetComponent<SpriteRenderer>().material.color; },
            (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
            baseColor * 0.95f,
            0.1f
        ).SetEase(Ease.Linear));
        for(int i = 0; i < JUGGLE_COUNT * 4; i++) {
            tweener.Append(DOTween.To(
                () => { return this.transform.localRotation; },
                (value) => { this.transform.localRotation = value; },
                new Vector3(0, 0, nowGridAngle + 90 * (i + 1)),
                0.05f
            ).SetEase(Ease.Linear));
        }
        tweener.Append(DOTween.To(
            () => { return this.transform.localScale; },
            (value) => { this.transform.localScale = value; },
            baseScale,
            0.1f
        ).SetEase(Ease.Linear));
        tweener.Join(DOTween.To(
            () => { return this.GetComponent<SpriteRenderer>().material.color; },
            (value) => { this.GetComponent<SpriteRenderer>().material.color = value; },
            baseColor,
            0.1f
        ).SetEase(Ease.Linear));
        tweener.AppendCallback(() => {
            tweener = null;
            if (callback != null) {
                callback();
            }
        });
        tweener.Play();
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
