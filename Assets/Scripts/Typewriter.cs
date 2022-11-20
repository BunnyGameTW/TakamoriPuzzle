using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{
    public float charPauseTime = 0.1f;  // 顯示間隔時間

    private TMP_Text textMeshPro;
    private bool isActive = false;
    private string words;               // 文本字串
    private float timer;                //計時器
    private int currentPos = 0;         //當前打字位置

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = this.GetComponent<TMP_Text>();
        words = textMeshPro.text;
        textMeshPro.text = "";
        isActive = false;
        timer = 0;
        currentPos = 0;

        // ---------------------------------------
        // TODO: 測試用，之後由更外層的manager控制
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
    }

    /** 開始播放 */
    public void startEffect() {
        textMeshPro.text = "";
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
        while(timer >= charPauseTime) {
            timer = timer - charPauseTime;
            currentPos++;
            textMeshPro.text = words.Substring(0, currentPos);
            if (currentPos >= words.Length) {
                finishTypewriter();
                break;
            }
        }
    }

    /** 結束處理 */
    private void finishTypewriter() {
        textMeshPro.text = words;
        isActive = false;
        timer = 0;
        currentPos = 0;
    }
}
