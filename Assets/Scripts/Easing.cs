using UnityEngine;

/**
    部分捕間算式，參考以下網站：
    https://easings.net/
*/

// Easing function
static class Easing
{
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

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
