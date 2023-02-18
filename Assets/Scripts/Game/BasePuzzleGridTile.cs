using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class BasePuzzleGridTile : MonoBehaviour
{
    protected Vector3 baseScale;        // 原始尺寸
    protected Vector3 basePosition;     // 原始座標
    protected Quaternion baseRotation;  // 原始角度
    protected Color baseColor;          // 原始顏色
    
    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // 初始化
    public virtual void init(Sprite sprite, Vector2 boxColliderSize) {
        SpriteRenderer tmepSpriteRenderer = this.GetComponent<SpriteRenderer>();
        BoxCollider2D tmepBoxCollider2D = this.GetComponent<BoxCollider2D>();

        tmepSpriteRenderer.sprite = sprite;
        tmepBoxCollider2D.size = boxColliderSize;
        baseScale = this.transform.localScale;
        basePosition = this.transform.localPosition;
        baseRotation = this.transform.localRotation;
        baseColor = this.GetComponent<SpriteRenderer>().material.color;
    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 檢查格子是否正確 */
    public abstract bool checkTileCorrect();

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 增加解謎正確效果 */
    protected Sequence addTileCorrectTween(Sequence tweener) {
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
        return tweener;
    }

    /** 增加解謎完成效果 */
    protected Sequence addPuzzleFinishTween(Sequence tweener) {
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
        return tweener;
    }
}
