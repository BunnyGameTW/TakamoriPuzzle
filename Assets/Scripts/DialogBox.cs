using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 選項資料
struct DialogBoxSelectData {
    public string selectText;   // 選項內文
    public int selectID;        // 選項ID
}

public class DialogBox : MonoBehaviour
{
    const string newlineChar = "<block>";    // 分段標記

    public Typewriter typewriter = null;    // 打字機物件
    public Button btnNext = null;           // 下一句按鈕物件

    private bool isReadyPlay = false;                       // 是否準備播放就緒
    private List<string> messageList = null;                // 訊息List
    private List<DialogBoxSelectData> selectList = null;    // 選項List

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        typewriter.init();
        typewriter.setFadeOutFinishCallback(() => {
            handleShowNextMessage();
        });

        selectList = new List<DialogBoxSelectData>();

        btnNext.onClick.AddListener(onClickDialogBox);
        // ---------------------------------------
        // TODO: 測試用
        messageList = new List<string>();
        addMessageToList("聖誕前夜，到處洋溢著歡樂的氣氛，但對TAKAMORI來説卻是例外");
        addMessageToList("Calli和Kiara之間充滿古怪與尷尬的氣氛，但她們之前並沒有吵過架");
        addMessageToList("Kiara最近總是很晚才回家，而且常常嘆氣，但當Calli試著跟她說話時\n她的反應卻總是一切都好");
        addMessageToList("Calli告訴她自己他們之間不能再這樣了，");
        addMessageToList("她擔心是因為自己太傲嬌讓Kiara忍受不了，想要離開她");
        addMessageToList("Calli: 我想讓她開心起來，但是我該怎麼做？");
        isReadyPlay = true;
        // var func = StartCoroutine(test());
        // ---------------------------------------
    }

    private IEnumerator test() {
        while(true) {
            typewriter.changeTypewriterState();
            yield return new WaitForSeconds(2.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 觸碰對話框 */
    public void onClickDialogBox() {
        int state = typewriter.getTypewriterState();
        switch(state) {
            case (int)TYPEWRITER_STATE.NONE: {
                // ---------------------------------------
                // TODO: 測試用
                playMessage();
                // ---------------------------------------
            } break;
            case (int)TYPEWRITER_STATE.FADE_IN: {
                typewriter.skipFadeInEffect();
            } break;
            case (int)TYPEWRITER_STATE.WAIT: {
                typewriter.startFadeOutEffect();
            } break;
            case (int)TYPEWRITER_STATE.FADE_OUT: {
                typewriter.skipFadeOutEffect();
            } break;
        }
    }

    /** 開始對話 */
    public void playMessage() {
        if (!isReadyPlay) {
            return;
        }
        isReadyPlay = false;
        handleShowNextMessage();
    }

    /** 設定全部訊息 */
    public void setMessageData(string text) {
        messageList = new List<string>();
        parseMessage(text);
    }

    /** 增加選項 */
    public void addSelectData(string text, int ID) {
        DialogBoxSelectData temp = new DialogBoxSelectData();
        temp.selectText = text;
        temp.selectID = ID;
        selectList.Add(temp);
    }

    /** 清空選項 */
    public void clearAllSelectData() {
        selectList.Clear();
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 處理下一句顯示 */
    private void handleShowNextMessage() {
        int state = typewriter.getTypewriterState();
        if (state != ((int)TYPEWRITER_STATE.NONE)) {
            Debug.LogError("Error DialogBox handleShowNextMessage");
            return;
        }
        if (messageList.Count > 0) {
            string str = messageList[0];
            messageList.RemoveAt(0);
            typewriter.setWord(str);
            typewriter.startFadeInEffect();
        }
        else {
            typewriter.clearWord();
            handleShowSelect();
        }
    }

    /** 處理選項創造 */
    private void handleShowSelect() {
        // TODO: 選擇選項處理
        Debug.Log("DialogBox handleShowSelect");
    }

    /** 解析訊息成陣列 */
    private void parseMessage(string text) {
        // TODO: 解析句子檢查有無分段標記
        addMessageToList(text);
        isReadyPlay = true;
    }

    /** 增加對話到陣列 */
    private void addMessageToList(string text) {
        messageList.Add(text);
    }

}
