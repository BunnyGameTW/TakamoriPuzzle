using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{
    public float charDuration = 0.005f;         // 顯示文字時間
    public float fadeInDuration = 0.05f;        // 淡入文字時間

    private TMP_Text textMeshPro;
    private TMP_TextInfo textInfo;
    private bool isActive = false;              // 是否正在播放
    private string words;                       // 文本字串
    
    private float timer = 0;                    // 計時器
    private int textCurrent = 0;                // 當前打字位置
    private bool isTextFinish = false;          // 打字機效果結束

    private float alphaTimer = 0;               // 特效計時器
    private int alphaCurrent = 0;               // 當前透明特效位置
    private bool isAlphaFinish = false;         // 透明效果結束

    private bool renewMeshVertices = false;     // 更新網格頂點
    private bool renewMeshColors32 = false;     // 更新網格顏色
    private System.Action finishCallback = null;    // 播放完成callback

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = this.GetComponent<TMP_Text>();
        isActive = false;

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
            handleFadeInEffect();
            if (isTextFinish && isAlphaFinish) {
                finishEffect();
            }
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
        alphaTimer = 0;
        alphaCurrent = 0;
        isAlphaFinish = false;
        isActive = true;

        for(int i = 0; i < textInfo.characterCount; i++) {
            setCharacterAlpha(i, 0);
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
            resetCharacterVertex(textCurrent);
            if (textCurrent >= textInfo.characterCount) {
                isTextFinish = true;
                break;
            }
            textCurrent++;
        }
    }

    /** 處理淡入效果 */
    private void handleFadeInEffect() {
        if (isAlphaFinish) {
            return;
        }
        alphaTimer = alphaTimer + Time.deltaTime;
        for(int i = alphaCurrent; i < textCurrent; i++) {
            float nowTween = (alphaTimer - i * charDuration);
            byte alpha = (byte)Mathf.Clamp(nowTween * 255, 0, 255);
            setCharacterAlpha(i, alpha);
            if (alpha >= 255) {
                alphaCurrent = i + 1;
            }
        }
        if (alphaCurrent >= textInfo.characterCount) {
            isAlphaFinish = true;
        }
    }

    /** 重設字元頂點位置 */
    private void resetCharacterVertex(int index) {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        int materialIndex = charInfo.materialReferenceIndex;
        int verticeIndex = charInfo.vertexIndex;
        TMP_MeshInfo meshInfo = textInfo.meshInfo[materialIndex];
        if (charInfo.elementType == TMP_TextElementType.Sprite) {
            verticeIndex = charInfo.spriteIndex;
        }

        if (charInfo.isVisible) {
            meshInfo.vertices[0 + verticeIndex] = charInfo.vertex_BL.position;
            meshInfo.vertices[1 + verticeIndex] = charInfo.vertex_TL.position;
            meshInfo.vertices[2 + verticeIndex] = charInfo.vertex_TR.position;
            meshInfo.vertices[3 + verticeIndex] = charInfo.vertex_BR.position;
            renewMeshVertices = true;
        }
    }

    /** 設定字元透明度 */
    private void setCharacterAlpha(int index, byte alpha) {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        int materialIndex = charInfo.materialReferenceIndex;
        int verticeIndex = charInfo.vertexIndex;
        TMP_MeshInfo meshInfo = textInfo.meshInfo[materialIndex];
        if (charInfo.elementType == TMP_TextElementType.Sprite) {
            verticeIndex = charInfo.spriteIndex;
        }

        if (charInfo.isVisible) {
            meshInfo.colors32[0 + verticeIndex].a = alpha;
            meshInfo.colors32[1 + verticeIndex].a = alpha;
            meshInfo.colors32[2 + verticeIndex].a = alpha;
            meshInfo.colors32[3 + verticeIndex].a = alpha;
            renewMeshColors32 = true;
        }
    }

    /** 結束效果播放 */
    private void finishEffect() {
        isActive = false;
        timer = 0;
        textCurrent = 0;
        isTextFinish = false;
        alphaTimer = 0;
        alphaCurrent = 0;
        isAlphaFinish = false;

        if (finishCallback != null) {
            finishCallback();
        }
    }
}
