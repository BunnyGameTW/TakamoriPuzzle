using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPuzzleTile : MonoBehaviour
{
    private Vector2Int goalGridPos = new Vector2Int();  // 目標格子位置
	private Vector2Int nowGridPos = new Vector2Int();   // 目前格子位置
    Coroutine tweenEvent = null;                        // 補間事件
    
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

    /** 執行捕間位移 */
    public void runPositionTween(Vector3 targetPos, float time, System.Action callback = null) {
        if (tweenEvent != null) {
            StopCoroutine(tweenEvent);
        }
        tweenEvent = StartCoroutine(updatePositionTween(targetPos, time, callback));
    }

    /** 執行淡入 */
    public void runFadeIn(float time, System.Action callback = null) {
        if (tweenEvent != null) {
            StopCoroutine(tweenEvent);
        }
        tweenEvent = StartCoroutine(updateFadeInTween(time, callback));
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 設定目標格子位置 */
    private void setGoalGridPos(Vector2Int pos) {
        goalGridPos = pos;
    }

    /** 更新捕間位移 */
    private IEnumerator updatePositionTween(Vector3 targetPos, float time, System.Action callback) {
        float tweenTime = 0;
        while(this.transform.localPosition != targetPos) {
			yield return null;
            tweenTime += Time.deltaTime;
			this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, targetPos, Easing.CubicOut(tweenTime/time));
		}
        tweenEvent = null;
        if (callback != null) {
            callback();
        }
        yield return null;
    }

    /** 更新捕間淡入 */
    private IEnumerator updateFadeInTween(float time, System.Action callback) {
        float tweenTime = 0;
        SpriteRenderer tmepSpriteRenderer = this.GetComponent<SpriteRenderer>();
        Color tempColor = tmepSpriteRenderer.color;
        Color targetColor = tmepSpriteRenderer.color;
        tempColor.a = 0;
        tmepSpriteRenderer.color = tempColor;
        while(tweenTime < time) {
			yield return null;
            tweenTime += Time.deltaTime;
			tmepSpriteRenderer.color = Color.Lerp(tempColor, targetColor, Easing.QuartInOut(tweenTime/time));
		}
        tweenEvent = null;
        if (callback != null) {
            callback();
        }
        yield return null;
    }
}
