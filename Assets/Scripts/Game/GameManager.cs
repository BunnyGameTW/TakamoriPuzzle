using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SlidingPuzzle slidingPuzzle = null;
    public DialogBox dialogBox = null;
    
    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        initSlidingPuzzle();
        initDialogBox();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 初始化謎題 */
    private void initSlidingPuzzle() {
        int episodeId = DataManager.instance.episodeId;
        int levelId = DataManager.instance.levelId;
        int puzzleGridX = DataManager.instance.puzzleGridX;
        int puzzleGridY = DataManager.instance.puzzleGridY;
        string puzzleImagePath = ResManager.getPuzzleImagePath(episodeId, levelId);
        Sprite puzzleImage = ResManager.loadSprite(puzzleImagePath);

        slidingPuzzle.init(puzzleImage, puzzleGridX, puzzleGridY);
        slidingPuzzle.startPuzzle();
        slidingPuzzle.setFinishPuzzleCallback(handleFinishPuzzle);
    }

    /** 初始化對話 */
    private void initDialogBox() {
        int episodeId = DataManager.instance.episodeId;
        int levelId = DataManager.instance.levelId;
        string language = DataManager.instance.language;
        Dictionary<string, Hashtable> allTable = null;
        Hashtable allData = null;

        dialogBox.init();
        LoadExcel.instance.loadFile(RES_PATH.PUZZLE_EXCEL);
        allTable = LoadExcel.instance.getTable("all");

        foreach(KeyValuePair<string, Hashtable> item in allTable) {
            bool episodeIdCheck = false;
            bool levelIdCheck = false;
            foreach(DictionaryEntry data in item.Value) {
                if ((string)data.Key == "episodeId" && (string)data.Value == episodeId.ToString()) {
                    episodeIdCheck = true;
                }
                if ((string)data.Key == "levelId" && (string)data.Value == levelId.ToString()) {
                    levelIdCheck = true;
                }
            }
            if (episodeIdCheck && levelIdCheck) {
                allData = item.Value;
            }
        }
        initDialogData(allData);
    }

    /** 初始化對話資料 */
    private void initDialogData(Hashtable allData) {
        string language = DataManager.instance.language;
        Hashtable contentData = null;
        string contentId = null;
        string message = null;
        int selectCount = 0;

        if (allData == null) {
            Debug.LogError("Error initDialogBox : episodeId & levelId can not find.");
            return;
        }
        contentId = (string)allData["contentId"];
        contentData = LoadExcel.instance.getObject("content", "id", contentId);
        message = (string)contentData[language + "_story"];
        dialogBox.setMessageData(message);

        while(true) {
            int count = selectCount + 1;
            string chioceKey = "chioceId_" + count.ToString();
            if ((string)allData[chioceKey] != "") {
                string contentKey = language + "_chioce_" + count.ToString();
                dialogBox.addSelectData((string)contentData[contentKey], int.Parse((string)allData[chioceKey]));
                selectCount++;
            }
            else {
                break;
            }
        }
        if (selectCount > 0) {
            dialogBox.setSelectCallback((ID) => {
                handleDialogSelect(ID);
            });
        }
        else {
            object unlockLevel = allData["unlockLevelId"];
            int value = 0;
            if ((string)unlockLevel != "") {
                value = int.Parse((string)unlockLevel);
                dialogBox.setFinishCallback(() => {
                    handleDialogFinish(value);
                });
            }
        }
    }

    /** 處理完成謎題 */
    private void handleFinishPuzzle() {
        dialogBox.playMessage();
    }

    /** 處理選擇選項 */
    private void handleDialogSelect(int ID) {
        Debug.Log("選擇解鎖關卡:" + ID);
        handleUnlockLevel(ID);
        MySceneManager.Instance.SetLoadSceneName("SelectLevelScene");
        MySceneManager.Instance.LoadScene();
    }
    
    /** 處理對話結束 */
    private void handleDialogFinish(int ID) {
        Debug.Log("直接解鎖關卡:" + ID);
        handleUnlockLevel(ID);
        MySceneManager.Instance.SetLoadSceneName("SelectLevelScene");
        MySceneManager.Instance.LoadScene();
    }

    /** 處理解鎖關卡 */
    private void handleUnlockLevel(int ID) {
        int episode = DataManager.instance.episodeId;
        int level = ID;
        DataManager.instance.unlockLevel(episode, level);
    }
}
