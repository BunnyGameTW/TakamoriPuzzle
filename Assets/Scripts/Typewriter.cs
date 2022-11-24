using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{
    private float charDuration = 0.005f;    // 顯示文字時間
    
    private float fadeInDuration = 0.05f;   // 淡入文字時間
    private byte fadeInAlpha = 0;
    private Vector2 fadeInPosition = new Vector2(0, 10);
    private float fadeInRotation = 0f;
    private float fadeInScale = 0f;

    private TMP_Text textMeshPro;
    private TMP_TextInfo textInfo;
    private bool isActive = false;              // 是否正在播放
    private string words;                       // 文本字串
    
    private float timer = 0;                    // 計時器
    private int textCurrent = 0;                // 當前打字位置
    private bool isTextFinish = false;          // 打字機效果結束

    private ObjectPool<CharacterData> objectPoolCharData;   // 字元資料物件池
    private CharacterData[] charDataArray = null;           // 字元資料陣列
    private bool renewMeshVertices = false;                 // 更新網格頂點
    private bool renewMeshColors32 = false;                 // 更新網格顏色
    private System.Action finishCallback = null;            // 播放完成callback

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = this.GetComponent<TMP_Text>();
        isActive = false;
        objectPoolCharData = new ObjectPool<CharacterData>();
        objectPoolCharData.init(() => {
            CharacterData temp = new CharacterData();
            TypewriterCharData.resetCharData(temp);
            return temp;
        }, 256);

        // ---------------------------------------
        // TODO: 測試用，之後由更外層的manager控制
        setWord(textMeshPro.text);
        setFinishCallback(() => {
            Debug.Log("Typewriter complete!");
        });
        startEffect();
        // ---------------------------------------
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) {
            return;
        }
        else {
            handleTypewriter();
            checkFinishEffect();
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

    /** 設定文本 */
    public void setWord(string str) {
        words = str;
        textMeshPro.text = words;
        textMeshPro.ForceMeshUpdate();
        textInfo = textMeshPro.textInfo;
        for(int i = 0; i < textInfo.materialCount; i++) {
            textInfo.meshInfo[i].Clear();
        }
    }

    /** 開始效果播放 */
    public void startEffect() {
        timer = 0;
        textCurrent = 0;
        isTextFinish = false;
        isActive = true;

        charDataArray = new CharacterData[textInfo.characterCount];
        for(int i = 0; i < textInfo.characterCount; i++) {
            charDataArray[i] = objectPoolCharData.getObject();
            TypewriterCharData.resetCharData(charDataArray[i]);
        }
    }

    /** 跳過效果播放 */
    public void skipEffect() {
    }

    /** 設定謎題完成callback */
    public void setFinishCallback(System.Action callback) {
        finishCallback = callback;
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 更新打字機效果 */
    private void handleTypewriter() {
        if (isTextFinish) {
            return;
        }
        timer += Time.deltaTime;
        while(timer >= charDuration) {
            timer = timer - charDuration;
            startFadeInAnimation(textCurrent);
            if (textCurrent >= textInfo.characterCount) {
                isTextFinish = true;
                break;
            }
            textCurrent++;
        }
    }

    /** 開始淡入動畫 */
    private void startFadeInAnimation(int index) {
        if (TypewriterCharData.resetCharacterVertex(textInfo, index)) {
            renewMeshVertices = true;
            charDataArray[index].isAnimateFinish = false;
            // TODO: 設定動畫資料
        }
    }

    /** 重設字元頂點位置 */
    private void resetCharacterVertex(int index) {
        if (TypewriterCharData.resetCharacterVertex(textInfo, index)) {
            renewMeshVertices = true;
        }
    }

    /** 設定字元透明度 */
    private void setCharacterAlpha(int index, byte alpha) {
        if (TypewriterCharData.setCharacterAlpha(textInfo, index, alpha)) {
            renewMeshColors32 = true;
        }
    }

    /** 檢查是否結束 */
    private void checkFinishEffect() {
        if (!isTextFinish) {
            return;
        }
        finishEffect();
    }

    /** 結束效果播放 */
    private void finishEffect() {
        isActive = false;
        timer = 0;
        textCurrent = 0;
        isTextFinish = false;

        for(int i = 0; i < textInfo.characterCount; i++) {
            objectPoolCharData.recovery(charDataArray[i]);
        }
        charDataArray = null;

        if (finishCallback != null) {
            finishCallback();
        }
    }
}
