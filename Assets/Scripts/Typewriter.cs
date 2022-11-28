using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 打字機狀態
enum TYPEWRITER_STATE {
    NONE,       // 不顯示
    FADE_IN,    // 淡入中
    WAIT,       // 正常顯示
    FADE_OUT,   // 淡出中
}

public class Typewriter : MonoBehaviour
{
    private float charDuration = 0.005f;    // 顯示文字時間

    // 淡入文字資料
    static private class fadeInData {
        static public float duration = 0.5f;
        static public float alphaStart = 0f;
        static public float positionStart = 0f;
        static public float rotationStart = 0f;
        static public float scaleStart = 0f;

        static public byte alpha = 32;
        static public Vector2 position = new Vector2(0, -10);
        static public float rotation = 180.0f;
        static public float scale = 0.5f;
    }
    // 淡出文字資料
    static private class fadeOutData {
        static public float duration = 0.5f;
        static public float alphaStart = 0f;
        static public float positionStart = 0f;
        static public float rotationStart = 0f;
        static public float scaleStart = 0f;

        static public byte alpha = 32;
        static public Vector2 position = new Vector2(0, -10);
        static public float rotation = 180.0f;
        static public float scale = 0.5f;
    }

    private TMP_Text textMeshPro;
    private TMP_TextInfo textInfo;
    private TYPEWRITER_STATE typewriterState = TYPEWRITER_STATE.NONE; // 打字機狀態

    private string words;                       // 文本字串
    private float timer = 0;                    // 計時器
    private int textCurrent = 0;                // 當前打字位置
    private bool isTextFinish = false;          // 打字機效果結束
    private bool isAnimateFinish = false;       // 打字機動畫結束

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
        textMeshPro = this.GetComponent<TMP_Text>();
        typewriterState = TYPEWRITER_STATE.NONE;
        objectPoolCharData = new ObjectPool<CharacterData>();
        objectPoolCharData.init(() => {
            CharacterData temp = new CharacterData();
            TypewriterCharData.resetCharData(out temp);
            return temp;
        }, 256);

        // ---------------------------------------
        // TODO: 測試用，之後由更外層的manager控制
        setWord(textMeshPro.text);
        setFadeInFinishCallback(() => {
            Debug.Log("Typewriter show complete!");
        });
        setFadeOutFinishCallback(() => {
            Debug.Log("Typewriter hide complete!");
        });
        // startFadeInEffect();
        // ---------------------------------------
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
        clearCharDataArray();
        createCharDataArray();
    }

    /** 切換打字機狀態機(萬用觸碰按鈕) */
    public void changeTypewriterState() {
        switch(typewriterState) {
            case TYPEWRITER_STATE.NONE: {
                startFadeInEffect();
            } break;
            case TYPEWRITER_STATE.FADE_IN: {
                // skipFadeInEffect();
            } break;
            case TYPEWRITER_STATE.WAIT: {
                // startFadeOutEffect();
            } break;
            case TYPEWRITER_STATE.FADE_OUT: {
                // skipFadeOutEffect();
            } break;
        }
    }

    /** 取得打字機狀態 */
    public int getTypewriterState() {
        return (int)typewriterState;
    }

    /** 開始淡入效果 */
    public void startFadeInEffect() {
        timer = 0;
        textCurrent = 0;
        isTextFinish = false;
        isAnimateFinish = false;
        typewriterState = TYPEWRITER_STATE.FADE_IN;
    }

    /** 開始淡出效果 */
    public void startFadeOutEffect() {
        timer = 0;
        textCurrent = 0;
        isTextFinish = false;
        isAnimateFinish = false;
        typewriterState = TYPEWRITER_STATE.FADE_OUT;
    }

    /** 跳過淡入效果 */
    public void skipFadeInEffect() {
        isTextFinish = true;
        textCurrent = textInfo.characterCount;
        for(int i = 0; i < textInfo.characterCount; i++) {
            showCharAnimation(i);
        }
    }

    /** 跳過淡出效果 */
    public void skipFadeOutEffect() {
        isTextFinish = true;
        textCurrent = textInfo.characterCount;
        for(int i = 0; i < textInfo.characterCount; i++) {
            hideCharAnimation(i);
        }
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
        timer += Time.deltaTime;
        while(timer >= charDuration) {
            timer = timer - charDuration;

            if (typewriterState == TYPEWRITER_STATE.FADE_IN) {
                startFadeInAnimation(textCurrent);
            }
            else if (typewriterState == TYPEWRITER_STATE.FADE_OUT) {
                startFadeOutAnimation(textCurrent);
            }

            textCurrent++;
            if (textCurrent >= textInfo.characterCount) {
                isTextFinish = true;
                break;
            }
        }
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
        if (count >= textInfo.characterCount) {
            isAnimateFinish = true;
        }
    }

    /** 更新捕間動畫 */
    private void handleCharTween(int index) {
        CharacterData item = charDataArray[index];
        Vector2 position = Vector2.zero;
        float rotation = 0f;
        float scale = 1.0f;
        byte alpha = 255;
        float positionRange, rotationRange, scaleRange, alphaRange;

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
            alphaRange = Mathf.Clamp((charDataArray[index].timer - item.alphaStart) / (item.duration - item.alphaStart), 0f, 1.0f);

            position.x = item.basePosition.x + (item.targetPosition.x - item.basePosition.x) * Easing.Tween(positionRange, EASE_TYPE.Linear);
            position.y = item.basePosition.y + (item.targetPosition.y - item.basePosition.y) * Easing.Tween(positionRange, EASE_TYPE.Linear);
            rotation = item.baseRotation + (item.targetRotation - item.baseRotation) * Easing.Tween(rotationRange, EASE_TYPE.Linear);
            scale = item.baseScale + (item.targetScale - item.baseScale) * Easing.Tween(scaleRange, EASE_TYPE.Linear);
            alpha = (byte)(item.baseColor.a + (item.targetColor.a - item.baseColor.a) * Easing.Tween(alphaRange, EASE_TYPE.Linear));

            if (TypewriterCharData.setCharacterTransform(textInfo, index, position, rotation, scale)) {
                renewMeshVertices = true;
            }
            if (TypewriterCharData.setCharacterAlpha(textInfo, index, alpha)) {
                renewMeshColors32 = true;
            }
        }
    }

    /** 瞬間顯示 */
    private void showCharAnimation(int index) {
        charDataArray[index].isAnimateFinish = true;
        TypewriterCharData.resetCharacterVertex(textInfo, index);
        TypewriterCharData.setCharacterAlpha(textInfo, index, charDataArray[index].targetColor.a);
        renewMeshVertices = true;
        renewMeshColors32 = true;
    }

    /** 瞬間消失 */
    private void hideCharAnimation(int index) {
        charDataArray[index].isAnimateFinish = true;
        TypewriterCharData.resetCharacterVertex(textInfo, index);
        TypewriterCharData.setCharacterAlpha(textInfo, index, 0);
        renewMeshVertices = true;
        renewMeshColors32 = true;
    }

    /** 開始淡入動畫 */
    private void startFadeInAnimation(int index) {
        if (TypewriterCharData.resetCharacterVertex(textInfo, index)) {
            renewMeshVertices = true;
        }
        // 設定淡入動畫資料
        charDataArray[index].isAnimateFinish = false;
        charDataArray[index].duration = fadeInData.duration;
        charDataArray[index].timer = 0;
        charDataArray[index].alphaStart = fadeInData.alphaStart;
        charDataArray[index].positionStart = fadeInData.positionStart;
        charDataArray[index].rotationStart = fadeInData.rotationStart;
        charDataArray[index].scaleStart = fadeInData.scaleStart;

        charDataArray[index].targetPosition = Vector3.zero;
        charDataArray[index].targetRotation = 0f;
        charDataArray[index].targetScale = 1.0f;
        charDataArray[index].targetColor = new Color32(255, 255, 255, 255);

        charDataArray[index].basePosition = fadeInData.position;
        charDataArray[index].baseRotation = fadeInData.rotation;
        charDataArray[index].baseScale = fadeInData.scale;
        charDataArray[index].baseColor.a = fadeInData.alpha;
    }

    /** 開始淡出動畫 */
    private void startFadeOutAnimation(int index) {
        // 設定淡出動畫資料
        charDataArray[index].isAnimateFinish = false;
        charDataArray[index].duration = fadeOutData.duration;
        charDataArray[index].timer = 0;
        charDataArray[index].alphaStart = fadeOutData.alphaStart;
        charDataArray[index].positionStart = fadeOutData.positionStart;
        charDataArray[index].rotationStart = fadeOutData.rotationStart;
        charDataArray[index].scaleStart = fadeOutData.scaleStart;

        charDataArray[index].basePosition = charDataArray[index].targetPosition;
        charDataArray[index].baseRotation = charDataArray[index].targetRotation;
        charDataArray[index].baseScale = charDataArray[index].targetScale;
        charDataArray[index].baseColor.a = charDataArray[index].targetColor.a;

        charDataArray[index].targetPosition = fadeOutData.position;
        charDataArray[index].targetRotation = fadeOutData.rotation;
        charDataArray[index].targetScale = fadeOutData.scale;
        charDataArray[index].targetColor.a = fadeOutData.alpha;
    }
}
