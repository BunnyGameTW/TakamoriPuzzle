using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 打字機狀態
enum TYPEWRITER_STATE: int {
    NONE,       // 不顯示
    FADE_IN,    // 淡入中
    WAIT,       // 正常顯示
    FADE_OUT,   // 淡出中
}

public class Typewriter : MonoBehaviour
{
    // 淡入文字資料
    static private class fadeInData {
        static public float charDuration = 0.016f;
        static public float duration = 0.3f;
        static public float alphaStart = duration * 0;
        static public float colorStart = duration * 0;
        static public float positionStart = duration * 0.25f;
        static public float rotationStart = duration * 0;
        static public float scaleStart = duration * 0;
        static public EASE_TYPE alphaEase = EASE_TYPE.QuadIn;
        static public EASE_TYPE colorEase = EASE_TYPE.QuartIn;
        static public EASE_TYPE positionEase = EASE_TYPE.OutBack;
        static public EASE_TYPE rotationEase = EASE_TYPE.CubicIn;
        static public EASE_TYPE scaleEase = EASE_TYPE.OutBack;

        static public Color32 color = new Color32(220, 100, 160, 32);
        static public Vector2 position = new Vector2(8, -20);
        static public Vector2 positionXRange = new Vector2(-8, 8);
        static public Vector2 positionYRange = new Vector2(0, -4);
        static public float rotation = 10.0f;
        static public float scale = 0.75f;
    }
    // 淡出文字資料
    static private class fadeOutData {
        static public float charDuration = 0.002f;
        static public float duration = 0.3f;
        static public float alphaStart = duration * 0;
        static public float colorStart = duration * 0;
        static public float positionStart = duration * 0.33f;
        static public float rotationStart = duration * 0;
        static public float scaleStart = duration * 0;
        static public EASE_TYPE alphaEase = EASE_TYPE.QuadOut;
        static public EASE_TYPE colorEase = EASE_TYPE.QuartOut;
        static public EASE_TYPE positionEase = EASE_TYPE.QuadOut;
        static public EASE_TYPE rotationEase = EASE_TYPE.CubicIn;
        static public EASE_TYPE scaleEase = EASE_TYPE.ElasticOut;

        static public Color32 color = new Color32(255, 64, 16, 32);
        static public Vector2 position = new Vector2(-8, 16);
        static public Vector2 positionXRange = new Vector2(-12, 12);
        static public Vector2 positionYRange = new Vector2(0, 8);
        static public float rotation = -10.0f;
        static public float scale = 1.25f;
    }

    private TMP_Text textMeshPro;
    private TMP_TextInfo textInfo;
    private TYPEWRITER_STATE typewriterState = TYPEWRITER_STATE.NONE; // 打字機狀態

    private string words;                       // 文本字串
    private float charDuration = 0f;            // 顯示文字時間
    private float timer = 0;                    // 計時器
    private int textCurrent = 0;                // 當前打字位置
    private bool isTextFinish = false;          // 打字機效果結束
    private bool isAnimateFinish = false;       // 打字機動畫結束
    private bool isTextSkip = false;            // 打字機快進

    private EASE_TYPE alphaEase;
    private EASE_TYPE colorEase;
    private EASE_TYPE positionEase;
    private EASE_TYPE rotationEase;
    private EASE_TYPE scaleEase;

    private ObjectPool<CharacterData> objectPoolCharData;   // 字元資料物件池
    private CharacterData[] charDataArray = null;           // 字元資料陣列
    private bool renewMeshVertices = false;                 // 更新網格頂點
    private bool renewMeshColors32 = false;                 // 更新網格顏色
    private System.Action fadeInFinishCallback = null;      // 淡入完成callback
    private System.Action fadeOutFinishCallback = null;     // 淡出完成callback

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
    }
    
    // 初始化
    public void init() {
        textMeshPro = this.GetComponent<TMP_Text>();
        textMeshPro.text = "";
        typewriterState = TYPEWRITER_STATE.NONE;
        objectPoolCharData = new ObjectPool<CharacterData>();
        objectPoolCharData.init(() => {
            CharacterData temp = new CharacterData();
            TypewriterCharData.resetCharData(out temp);
            return temp;
        }, 256);
        clearWord();
    }

    // Update is called once per frame
    void Update()
    {
        switch(typewriterState) {
            case TYPEWRITER_STATE.FADE_IN:
            case TYPEWRITER_STATE.FADE_OUT: {
                handleTypewriter();
                handleTypewriterEffect();
                checkEffectFinish();
            } break;
            default: {
            } break;
        }
        if (renewMeshVertices) {
            renewMeshVertices = false;
            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
        if (renewMeshColors32) {
            renewMeshColors32 = false;
            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
    
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 設定文本(僅在NONE狀態，播放中會出錯) */
    public void setWord(string str) {
        if (typewriterState != TYPEWRITER_STATE.NONE) {
            return;
        }

        words = str;
        textMeshPro.text = words;
        textMeshPro.ForceMeshUpdate();
        textInfo = textMeshPro.textInfo;
        for(int i = 0; i < textInfo.materialCount; i++) {
            textInfo.meshInfo[i].Clear();
        }
        renewMeshVertices = true;
        clearCharDataArray();
        createCharDataArray();
    }

    /** 清除文本 */
    public void clearWord() {
        clearCharDataArray();
        textMeshPro.text = "";
        textMeshPro.ForceMeshUpdate();
        typewriterState = TYPEWRITER_STATE.NONE;
    }

    /** 取得打字機狀態 */
    public int getTypewriterState() {
        return (int)typewriterState;
    }

    /** 開始淡入效果 */
    public void startFadeInEffect() {
        if (typewriterState != TYPEWRITER_STATE.NONE) {
            return;
        }
        timer = 0;
        textCurrent = 0;
        charDuration = fadeInData.charDuration;
        alphaEase = fadeInData.alphaEase;
        colorEase = fadeInData.colorEase;
        positionEase = fadeInData.positionEase;
        rotationEase = fadeInData.rotationEase;
        scaleEase = fadeInData.scaleEase;
        isTextFinish = false;
        isAnimateFinish = false;
        isTextSkip = false;
        typewriterState = TYPEWRITER_STATE.FADE_IN;

        for(int i = 0; i < textInfo.characterCount; i++) {
            handleFadeInData(i);
        }
        for(int i = 0; i < textInfo.materialCount; i++) {
            textInfo.meshInfo[i].Clear();
        }
        renewMeshVertices = true;
    }

    /** 開始淡出效果 */
    public void startFadeOutEffect() {
        if (typewriterState != TYPEWRITER_STATE.WAIT) {
            return;
        }
        timer = 0;
        textCurrent = 0;
        charDuration = fadeOutData.charDuration;
        alphaEase = fadeOutData.alphaEase;
        colorEase = fadeOutData.colorEase;
        positionEase = fadeOutData.positionEase;
        rotationEase = fadeOutData.rotationEase;
        scaleEase = fadeOutData.scaleEase;
        isTextFinish = false;
        isAnimateFinish = false;
        isTextSkip = false;
        typewriterState = TYPEWRITER_STATE.FADE_OUT;

        for(int i = 0; i < textInfo.characterCount; i++) {
            handleFadeOutData(i);
        }
    }

    /** 跳過打字機效果 */
    public void skipTypewriterEffect() {
        if (typewriterState != TYPEWRITER_STATE.FADE_IN
        && typewriterState != TYPEWRITER_STATE.FADE_OUT) {
            return;
        }
        isTextSkip = true;
    }

    /** 設定淡入完成callback */
    public void setFadeInFinishCallback(System.Action callback) {
        fadeInFinishCallback = callback;
    }

    /** 設定淡出完成callback */
    public void setFadeOutFinishCallback(System.Action callback) {
        fadeOutFinishCallback = callback;
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 檢查效果是否結束 */
    private void checkEffectFinish() {
        if (!isTextFinish || !isAnimateFinish) {
            return;
        }
        
        if (typewriterState == TYPEWRITER_STATE.FADE_IN) {
            handleFadeInFinish();
        }
        else if (typewriterState == TYPEWRITER_STATE.FADE_OUT) {
            handleFadeOutFinish();
        }
    }

    /** 結束淡入效果播放 */
    private void handleFadeInFinish() {
        typewriterState = TYPEWRITER_STATE.WAIT;
        timer = 0;
        textCurrent = 0;
        isTextFinish = false;
        isAnimateFinish = false;
        isTextSkip = false;

        if (fadeInFinishCallback != null) {
            fadeInFinishCallback();
        }
    }

    /** 結束淡出效果播放 */
    private void handleFadeOutFinish() {
        typewriterState = TYPEWRITER_STATE.NONE;
        timer = 0;
        textCurrent = 0;
        isTextFinish = false;
        isAnimateFinish = false;
        isTextSkip = false;

        if (fadeOutFinishCallback != null) {
            fadeOutFinishCallback();
        }
    }

    /** 創造字元陣列資料 */
    private void createCharDataArray() {
        charDataArray = new CharacterData[textInfo.characterCount];
        for(int i = 0; i < textInfo.characterCount; i++) {
            charDataArray[i] = objectPoolCharData.getObject();
            TypewriterCharData.resetCharData(out charDataArray[i]);
        }
    }

    /** 清除字元陣列資料 */
    private void clearCharDataArray() {
        if (charDataArray == null || charDataArray.Length <= 0) {
            return;
        }
        for(int i = 0; i < charDataArray.Length; i++) {
            objectPoolCharData.recovery(charDataArray[i]);
        }
        charDataArray = null;
    }

    /** 更新打字機 */
    private void handleTypewriter() {
        if (isTextFinish) {
            return;
        }
        float duration = getTypewriterRenewSpeed();

        timer += Time.deltaTime;
        while(timer >= duration) {
            timer = timer - duration;

            if (typewriterState == TYPEWRITER_STATE.FADE_IN
            && TypewriterCharData.resetCharacterVertex(textInfo, textCurrent)) {
                renewMeshVertices = true;
            }
            charDataArray[textCurrent].isAnimateFinish = false;

            textCurrent++;
            if (textCurrent >= textInfo.characterCount) {
                isTextFinish = true;
                break;
            }
            duration = getTypewriterRenewSpeed();
        }
    }

    /** 取得打字機更新速度 */
    private float getTypewriterRenewSpeed() {
        float duration = charDuration;
        int charByte = System.Text.Encoding.Default.GetByteCount(textInfo.characterInfo[textCurrent].character.ToString());
        if (isTextSkip) {
            return 0;
        }
        return charDuration * charByte;
    }

    /** 更新打字機效果 */
    private void handleTypewriterEffect() {
        if (isAnimateFinish) {
            return;
        }
        int count = 0;
        for(int i = 0; i < textInfo.characterCount; i++) {
            if (charDataArray[i].isAnimateFinish == false) {
                handleCharTween(i);
            }
            else {
                count = count + 1;
            }
        }
        if (isTextFinish && count >= textInfo.characterCount) {
            isAnimateFinish = true;
        }
    }

    /** 更新捕間動畫 */
    private void handleCharTween(int index) {
        CharacterData item = charDataArray[index];
        Vector2 position = item.basePosition;
        float rotation = item.baseRotation;
        float scale = item.baseScale;
        Color32 color = item.baseColor;
        float positionRange, rotationRange, scaleRange, colorRange, alphaRange;

        charDataArray[index].timer = charDataArray[index].timer + Time.deltaTime;
        if (charDataArray[index].timer >= item.duration) {
            if (typewriterState == TYPEWRITER_STATE.FADE_IN) {
                showCharAnimation(index);
            }
            else if (typewriterState == TYPEWRITER_STATE.FADE_OUT) {
                hideCharAnimation(index);
            }
        }
        else {
            positionRange = Mathf.Clamp((charDataArray[index].timer - item.positionStart) / (item.duration - item.positionStart), 0f, 1.0f);
            rotationRange = Mathf.Clamp((charDataArray[index].timer - item.rotationStart) / (item.duration - item.rotationStart), 0f, 1.0f);
            scaleRange = Mathf.Clamp((charDataArray[index].timer - item.scaleStart) / (item.duration - item.scaleStart), 0f, 1.0f);
            colorRange = Mathf.Clamp((charDataArray[index].timer - item.colorStart) / (item.duration - item.colorStart), 0f, 1.0f);
            alphaRange = Mathf.Clamp((charDataArray[index].timer - item.alphaStart) / (item.duration - item.alphaStart), 0f, 1.0f);

            position.x = item.basePosition.x + (item.targetPosition.x - item.basePosition.x) * Easing.Tween(positionRange, positionEase);
            position.y = item.basePosition.y + (item.targetPosition.y - item.basePosition.y) * Easing.Tween(positionRange, positionEase);
            rotation = item.baseRotation + (item.targetRotation - item.baseRotation) * Easing.Tween(rotationRange, rotationEase);
            scale = item.baseScale + (item.targetScale - item.baseScale) * Easing.Tween(scaleRange, scaleEase);
            color.r = (byte)(item.baseColor.r + (item.targetColor.r - item.baseColor.r) * Easing.Tween(colorRange, colorEase));
            color.g = (byte)(item.baseColor.g + (item.targetColor.g - item.baseColor.g) * Easing.Tween(colorRange, colorEase));
            color.b = (byte)(item.baseColor.b + (item.targetColor.b - item.baseColor.b) * Easing.Tween(colorRange, colorEase));
            color.a = (byte)(item.baseColor.a + (item.targetColor.a - item.baseColor.a) * Easing.Tween(alphaRange, alphaEase));

            if (TypewriterCharData.setCharacterTransform(textInfo, index, position, rotation, scale)) {
                renewMeshVertices = true;
            }
            if (TypewriterCharData.setCharacterColor(textInfo, index, color)) {
                renewMeshColors32 = true;
            }
        }
    }

    /** 瞬間顯示 */
    private void showCharAnimation(int index) {
        CharacterData item = charDataArray[index];
        Vector2 position = item.targetPosition;
        float rotation = item.targetRotation;
        float scale = item.targetScale;
        Color32 color = item.targetColor;
        
        charDataArray[index].isAnimateFinish = true;
        if (TypewriterCharData.setCharacterTransform(textInfo, index, position, rotation, scale)) {
            renewMeshVertices = true;
        }
        if (TypewriterCharData.setCharacterColor(textInfo, index, color)) {
            renewMeshColors32 = true;
        }
    }

    /** 瞬間消失 */
    private void hideCharAnimation(int index) {
        CharacterData item = charDataArray[index];
        Vector2 position = item.targetPosition;
        float rotation = item.targetRotation;
        float scale = item.targetScale;
        Color32 color = item.baseColor;
        color.a = 0;

        charDataArray[index].isAnimateFinish = true;
        if (TypewriterCharData.setCharacterTransform(textInfo, index, position, rotation, scale)) {
            renewMeshVertices = true;
        }
        if (TypewriterCharData.setCharacterColor(textInfo, index, color)) {
            renewMeshColors32 = true;
        }
    }

    /** 處理淡入資料 */
    private void handleFadeInData(int index) {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        // 設定淡入動畫資料
        charDataArray[index].duration = fadeInData.duration;
        charDataArray[index].timer = 0;
        charDataArray[index].alphaStart = fadeInData.alphaStart;
        charDataArray[index].colorStart = fadeInData.colorStart;
        charDataArray[index].positionStart = fadeInData.positionStart;
        charDataArray[index].rotationStart = fadeInData.rotationStart;
        charDataArray[index].scaleStart = fadeInData.scaleStart;

        charDataArray[index].targetPosition = Vector3.zero;
        charDataArray[index].targetRotation = 0f;
        charDataArray[index].targetScale = 1.0f;
        charDataArray[index].targetColor = charInfo.color;
        charDataArray[index].targetColor.a = 255;

        charDataArray[index].basePosition = fadeInData.position;
        charDataArray[index].baseRotation = fadeInData.rotation;
        charDataArray[index].baseScale = fadeInData.scale;
        charDataArray[index].baseColor = fadeInData.color;
        
        charDataArray[index].basePosition.x += Random.Range(fadeInData.positionXRange.x, fadeInData.positionXRange.y);
        charDataArray[index].basePosition.y += Random.Range(fadeInData.positionYRange.x, fadeInData.positionYRange.y);
    }

    /** 處理淡出資料 */
    private void handleFadeOutData(int index) {
        // 設定淡出動畫資料
        charDataArray[index].duration = fadeOutData.duration;
        charDataArray[index].timer = 0;
        charDataArray[index].alphaStart = fadeOutData.alphaStart;
        charDataArray[index].colorStart = fadeOutData.colorStart;
        charDataArray[index].positionStart = fadeOutData.positionStart;
        charDataArray[index].rotationStart = fadeOutData.rotationStart;
        charDataArray[index].scaleStart = fadeOutData.scaleStart;

        charDataArray[index].basePosition = charDataArray[index].targetPosition;
        charDataArray[index].baseRotation = charDataArray[index].targetRotation;
        charDataArray[index].baseScale = charDataArray[index].targetScale;
        charDataArray[index].baseColor.a = 255;

        charDataArray[index].targetPosition = fadeOutData.position;
        charDataArray[index].targetRotation = fadeOutData.rotation;
        charDataArray[index].targetScale = fadeOutData.scale;
        charDataArray[index].targetColor = fadeOutData.color;
        
        charDataArray[index].targetPosition.x += Random.Range(fadeOutData.positionXRange.x, fadeOutData.positionXRange.y);
        charDataArray[index].targetPosition.y += Random.Range(fadeOutData.positionYRange.x, fadeOutData.positionYRange.y);
    }
}
