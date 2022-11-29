using UnityEngine;

/**
    部分補間算式，有需要其他的算式請自行新增，參考以下網站：
    https://easings.net/
*/

// Ease 種類
enum EASE_TYPE {
    Linear,

    QuadIn,
    QuadOut,
    QuadInOut,

    CubicIn,
    CubicOut,
    CubicInOut,

    QuartIn,
    QuartOut,
    QuartInOut,

    QuintIn,
    QuintOut,
    QuintInOut,
    
    ElasticOut,
};

// Easing function
static class Easing
{
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    // 通用Easing接口
    static public float Tween(float t, EASE_TYPE type) {
        float value = 0.0f;
        switch(type) {
            case EASE_TYPE.Linear: {
                value = Linear(t);
            }
            break;
            case EASE_TYPE.QuadIn: {
                value = QuadIn(t);
            }
            break;
            case EASE_TYPE.QuadOut: {
                value = QuadOut(t);
            }
            break;
            case EASE_TYPE.QuadInOut: {
                value = QuadInOut(t);
            }
            break;
            case EASE_TYPE.CubicIn: {
                value = CubicIn(t);
            }
            break;
            case EASE_TYPE.CubicOut: {
                value = CubicOut(t);
            }
            break;
            case EASE_TYPE.CubicInOut: {
                value = CubicInOut(t);
            }
            break;
            case EASE_TYPE.QuartIn: {
                value = QuartIn(t);
            }
            break;
            case EASE_TYPE.QuartOut: {
                value = QuartOut(t);
            }
            break;
            case EASE_TYPE.QuartInOut: {
                value = QuartInOut(t);
            }
            break;
            case EASE_TYPE.QuintIn: {
                value = QuintIn(t);
            }
            break;
            case EASE_TYPE.QuintOut: {
                value = QuintOut(t);
            }
            break;
            case EASE_TYPE.QuintInOut: {
                value = QuintInOut(t);
            }
            break;
            case EASE_TYPE.ElasticOut: {
                value = ElasticOut(t);
            }
            break;
            default: {
                Debug.Log("Error: Easing.Tween type not implementation.");
            }
            break;
        }
        return value;
    }

    // Linear
    static public float Linear(float t) {
        return t;
    }

    // Quad
    static public float QuadIn(float t) {
        return EasingIn(t, 2);
    }

    static public float QuadOut(float t) {
        return EasingOut(t, 2);
    }

    static public float QuadInOut(float t) {
        return EasingInOut(t, 2);
    }

    // Cubic
    static public float CubicIn(float t) {
        return EasingIn(t, 3);
    }

    static public float CubicOut(float t) {
        return EasingOut(t, 3);
    }

    static public float CubicInOut(float t) {
        return EasingInOut(t, 3);
    }

    // Quart
    static public float QuartIn(float t) {
        return EasingIn(t, 4);
    }

    static public float QuartOut(float t) {
        return EasingOut(t, 4);
    }
    
    static public float QuartInOut(float t) {
        return EasingInOut(t, 4);
    }

    // Quint
    static public float QuintIn(float t) {
        return EasingIn(t, 5);
    }

    static public float QuintOut(float t) {
        return EasingOut(t, 5);
    }
    
    static public float QuintInOut(float t) {
        return EasingInOut(t, 5);
    }

    static public float ElasticOut(float t) {
        const float c4 = (2 * Mathf.PI) / 3;

        return t == 0
        ? 0
        : t == 1
        ? 1
        : Mathf.Pow(2, -10 * t) * Mathf.Sin((float)(t * 10 - 0.75) * c4) + 1;
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    static private float EasingIn(float t, int state) {
        return Mathf.Pow(t, state);
    }
    
    static private float EasingOut(float t, int state) {
        return 1 - Mathf.Pow(1 - t, state);
    }

    static private float EasingInOut(float t, int state) {
        return t < 0.5 ? Mathf.Pow(2, state - 1)*Mathf.Pow(t, state) : 1 - Mathf.Pow(-2 * t + 2, state)/2;
    }
}
