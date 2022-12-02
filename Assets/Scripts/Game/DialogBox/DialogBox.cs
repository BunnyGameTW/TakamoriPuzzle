using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 選項資料
struct DialogBoxSelectData {
    public string selectText;   // 選項內文
    public int selectID;        // 選項ID
}

public class DialogBox : MonoBehaviour
{
    const string newlineChar = "<block>";    // 分段標記

    public Typewriter typewriter = null;        // 打字機物件
    public Button btnNext = null;               // 下一句按鈕物件
    public GameObject waitIcon = null;          // 等待下一句圖示
    public GameObject selectGroup = null;       // 選項群組
    public Button prefabDialogSelect = null;    // 選項物件

    private Coroutine waitIconEvent = null;                 // 等待圖示事件
    private bool isReadyPlay = false;                       // 是否準備播放就緒
    private List<string> messageList = null;                // 訊息List
    private List<DialogBoxSelectData> selectList = null;    // 選項List
    private System.Action<int> selectCallback = null;       // 選擇完成callback

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // 初始化
    public void init() {
        typewriter.init();
        typewriter.setFadeInFinishCallback(() => {
            showWaitNextIcon();
        });
        typewriter.setFadeOutFinishCallback(() => {
            handleShowNextMessage();
        });
        waitIcon.SetActive(false);
        clearSelectGroup();
        selectList = new List<DialogBoxSelectData>();
        btnNext.onClick.AddListener(onClickDialogBox);
        this.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

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

            } break;
            case (int)TYPEWRITER_STATE.WAIT: {
                hideWaitNextIcon();
                typewriter.startFadeOutEffect();
            } break;
            case (int)TYPEWRITER_STATE.FADE_IN:
            case (int)TYPEWRITER_STATE.FADE_OUT: {
                typewriter.skipTypewriterEffect();
            } break;
        }
    }

    /** 觸碰選項按鈕 */
    public void onClickSelect(int ID) {
        clearSelectGroup();
        clearAllSelectData();
        if (selectCallback != null) {
            selectCallback(ID);
        }
    }

    /** 設定選擇完成callback */
    public void setSelectCallback(System.Action<int> callback) {
        selectCallback = callback;
    }

    /** 開始對話 */
    public void playMessage() {
        if (!isReadyPlay) {
            return;
        }
        isReadyPlay = false;
        this.gameObject.SetActive(true);
        handleShowNextMessage();
    }

    /** 設定訊息資料 */
    public void setMessageData(string text) {
        messageList = new List<string>();
        if (parseMessage(text)) {
            isReadyPlay = true;
        }
    }

    /** 增加選項資料 */
    public void addSelectData(string text, int ID) {
        DialogBoxSelectData temp = new DialogBoxSelectData();
        temp.selectText = text;
        temp.selectID = ID;
        selectList.Add(temp);
    }

    /** 清空選項資料 */
    public void clearAllSelectData() {
        selectList.Clear();
    }

    /** 清除所有選項按鈕 */
    public void clearSelectGroup() {
        if (selectGroup.transform.childCount > 0) {
            for(int i = 0; i < selectGroup.transform.childCount; i++) {
                GameObject item = selectGroup.transform.GetChild(i).gameObject;
                Destroy(item);
            }
        }
        selectGroup.SetActive(false);
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
            StartCoroutine(runMessageFinishEffect());
        }
    }

    /** 結束對話效果 */
    private IEnumerator runMessageFinishEffect() {
        yield return new WaitForSeconds(0.5f);
        handleShowSelect();
    }

    /** 處理選項創造 */
    private void handleShowSelect() {
        if (selectList.Count <= 0) {
            Debug.LogError("Error DialogBox handleShowSelect: No slelect");
            return;
        }
        selectGroup.SetActive(true);
        for( int i = 0; i < selectList.Count; i++) {
            Button tmepButton = Instantiate(prefabDialogSelect, Vector3.zero, Quaternion.identity);
            GameObject textChild = tmepButton.transform.Find("Text (TMP)").gameObject;
            int selectID = selectList[i].selectID;

            tmepButton.onClick.AddListener(() => {
                this.onClickSelect(selectID);
            });
            textChild.GetComponent<TMP_Text>().text = selectList[i].selectText;
            tmepButton.transform.SetParent(selectGroup.transform);
            tmepButton.transform.localScale = selectGroup.transform.localScale;
        }
    }

    /** 解析訊息成陣列 */
    private bool parseMessage(string text) {
        string tempStr = text;
        int index = 0;
        while(tempStr.Length > 0) {
            index = tempStr.IndexOf(newlineChar);
            if (index > 0) {
                string result = tempStr.Substring(0, index);
                addMessageToList(result);
                tempStr = tempStr.Remove(0, index + newlineChar.Length);
            }
            else {
                addMessageToList(tempStr);
                tempStr = tempStr.Remove(0, tempStr.Length - 1);
                return true;
            }
        }
        return false;
    }

    /** 增加對話到陣列 */
    private void addMessageToList(string text) {
        messageList.Add(text);
    }

    /** 顯示等待下一句標示 */
    private void showWaitNextIcon() {
        waitIconEvent = StartCoroutine(runWaitIconEffect());
    }

    /** 隱藏等待下一句標示 */
    private void hideWaitNextIcon() {
        waitIcon.SetActive(false);
        StopCoroutine(waitIconEvent);
        waitIconEvent = null;
    }

    /** 結束對話效果 */
    private IEnumerator runWaitIconEffect() {
        yield return new WaitForSeconds(0.5f);
        waitIcon.SetActive(true);
        while(true) {
            yield return new WaitForSeconds(0.5f);
            waitIcon.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            waitIcon.SetActive(true);
        }
    }
}
