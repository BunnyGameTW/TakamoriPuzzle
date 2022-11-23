using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{
    public float charDuration = 0.01f;   // 顯示間隔時間

    private TMP_Text textMeshPro;
    private TMP_TextInfo textInfo;
    private bool isActive = false;      // 是否正在播放
    private string words;               // 文本字串
    private float timer;                // 計時器
    private int currentPos = 0;         // 當前打字位置

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = this.GetComponent<TMP_Text>();
        isActive = false;
        timer = 0;
        currentPos = 0;

        // ---------------------------------------
        // TODO: 測試用，之後由更外層的manager控制
        setWord(textMeshPro.text);
        startEffect();
        // ---------------------------------------
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) {
            return;
        }
        handleTypewriter();
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
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    /** 開始播放 */
    public void startEffect() {
        timer = 0;
        currentPos = 0;
        isActive = true;
    }

    /** 跳過播放 */
    public void skipEffect() {
        finishTypewriter();
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 更新打字機效果 */
    private void handleTypewriter() {
        timer += Time.deltaTime;
        while(timer >= charDuration) {
            timer = timer - charDuration;
            handleTypewriterEffect();
            if (currentPos >= textInfo.characterCount) {
                finishTypewriter();
                break;
            }
            currentPos++;
        }
    }

    /** 處理打字機效果 */
    private void handleTypewriterEffect() {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[currentPos];
        int materialIndex = charInfo.materialReferenceIndex;
        int verticeIndex = charInfo.vertexIndex;
        if (charInfo.elementType == TMP_TextElementType.Sprite) {
            verticeIndex = charInfo.spriteIndex;
        }
        if (charInfo.isVisible) {
            textInfo.meshInfo[materialIndex].vertices[0 + verticeIndex] = charInfo.vertex_BL.position;
            textInfo.meshInfo[materialIndex].vertices[1 + verticeIndex] = charInfo.vertex_TL.position;
            textInfo.meshInfo[materialIndex].vertices[2 + verticeIndex] = charInfo.vertex_TR.position;
            textInfo.meshInfo[materialIndex].vertices[3 + verticeIndex] = charInfo.vertex_BR.position;
            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }

    /** 結束處理 */
    private void finishTypewriter() {
        isActive = false;
        timer = 0;
        currentPos = 0;
    }
}
