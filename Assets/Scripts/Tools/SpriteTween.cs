using System.Collections;
using UnityEngine;

/**
    簡易的物件位移/淡入等補間功能
*/


// Easing function
static class SpriteTween
{
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 更新補間位移 */
    static public IEnumerator updatePositionTween(GameObject obj, Vector3 targetPos, float time, EASE_TYPE type = EASE_TYPE.Linear, System.Action callback = null) {
        float tweenTime = 0;
        while(obj.transform.localPosition != targetPos) {
			yield return null;
            tweenTime += Time.deltaTime;
			obj.transform.localPosition = Vector3.Lerp(obj.transform.localPosition, targetPos, Easing.Tween(tweenTime/time, type));
		}
        if (callback != null) {
            callback();
        }
        yield return null;
    }

    /** 更新補間淡入 */
    static public IEnumerator updateFadeInTween(GameObject obj, float time, EASE_TYPE type = EASE_TYPE.Linear, System.Action callback = null) {
        float tweenTime = 0;
        SpriteRenderer tmepSpriteRenderer = obj.GetComponent<SpriteRenderer>();
        Color tempColor = tmepSpriteRenderer.color;
        Color targetColor = tmepSpriteRenderer.color;
        tempColor.a = 0;
        tmepSpriteRenderer.color = tempColor;
        while(tweenTime < time) {
			yield return null;
            tweenTime += Time.deltaTime;
			tmepSpriteRenderer.color = Color.Lerp(tempColor, targetColor, Easing.Tween(tweenTime/time, type));
		}
        if (callback != null) {
            callback();
        }
        yield return null;
    }

    /** 更新補間淡出 */
    static public IEnumerator updateFadeOutTween(GameObject obj, float time, EASE_TYPE type = EASE_TYPE.Linear, System.Action callback = null) {
        float tweenTime = 0;
        SpriteRenderer tmepSpriteRenderer = obj.GetComponent<SpriteRenderer>();
        Color tempColor = tmepSpriteRenderer.color;
        Color targetColor = tmepSpriteRenderer.color;
        targetColor.a = 0;
        while(tweenTime < time) {
			yield return null;
            tweenTime += Time.deltaTime;
			tmepSpriteRenderer.color = Color.Lerp(tempColor, targetColor, Easing.Tween(tweenTime/time, type));
		}
        if (callback != null) {
            callback();
        }
        yield return null;
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------
}
