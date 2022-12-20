using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public struct LevelData
{
    public int episodeId;
    public int levelId;
    public bool isLock;
}
public class LevelUI : MonoBehaviour
{
    LevelData[] levelDatas;
    public GameObject[] layouts;
    public GameObject levelButtonPrefab;
    public TextMeshProUGUI textTitle;
    public AwardPopup awardPopup;

    int[] sections = {1, 2, 4, 1};
    Hashtable data;
    const string EXCEL_TAG_NAME = "episodeTitle";
    const string INDEX_NAME = "id";
    const string TITLE_SUFFIX_NAME = "_title";
    // Start is called before the first frame update
    void Start()
    {
        DataManager.instance.LanguageChanged += OnLanguageChanged;
        int episodeId = DataManager.instance.episodeId;

        if (DataManager.instance.isUnlockLevel(episodeId, 1) == false) {
            DataManager.instance.unlockLevel(episodeId, 1);
        }

        //set title
        data = LoadExcel.instance.getObject(EXCEL_TAG_NAME, INDEX_NAME, episodeId);
        UpdateTitle(DataManager.instance.getLanguageCode());

        levelDatas = new LevelData[8];
        int index = 0, counter = 0;
        for (int i = 0; i < levelDatas.Length; i++)
        {
            levelDatas[i].episodeId = episodeId;
            levelDatas[i].levelId = i + 1;
            levelDatas[i].isLock = !DataManager.instance.isUnlockLevel(levelDatas[i].episodeId, levelDatas[i].levelId);

            //create buttons
            if (i == 0) 
            {
                FindObjectOfType<LevelButton>().SetData(levelDatas[i]);
            }
            else
            {            
                GameObject go = Instantiate(levelButtonPrefab, layouts[index].transform);
                go.GetComponent<LevelButton>().SetData(levelDatas[i]);
            }
            if (i != 0)
                counter++;
            if (counter >= sections[index + 1])
            {
                counter = 0;
                index++;
            }
        }

        if (DataManager.instance.episodeClear == true) {
            DataManager.instance.episodeClear = false;
            showAwardPopup();
        }
    }

    private void OnDestroy()
    {
        DataManager.instance.LanguageChanged -= OnLanguageChanged;
    }

    void OnLanguageChanged(object sender, string languageCode)
    {
        UpdateTitle(languageCode);
    }

    void UpdateTitle(string language)
    {
        string message = (string)data[language + TITLE_SUFFIX_NAME];
        textTitle.text = message;
    }

    /** 顯示獎勵CG彈窗 */
    private void showAwardPopup() {
        int episodeId = DataManager.instance.episodeId;
        Hashtable uiTextData = LoadExcel.instance.getObject("uiText", "name", "unlockCG");
        Hashtable contentData = LoadExcel.instance.getObject("episodeTitle", "id", episodeId);
        List<Hashtable> levelList = LoadExcel.instance.getObjectList("all", "episodeId", episodeId.ToString());
        string puzzleImagePath;
        int awardID;

        awardID = levelList.Count + 1;
        puzzleImagePath = ResManager.getPuzzleImagePath(episodeId, awardID);
        StartCoroutine(ResManager.asyncLoadSprite(puzzleImagePath, (sprite) => {
            MySceneManager.Instance.ShowButtons(false);
            awardPopup.init(sprite);
            awardPopup.show();
        }));
        awardPopup.setTouchCallback(() => {
            MySceneManager.Instance.SetLoadSceneState(SceneState.Login);
            MySceneManager.Instance.LoadScene();
        });
    }
}
