using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Book : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Image puzzleImage, bookImage;
    Button button;
    CellData _data;
    const string PUZZLE_IMAGE_FORMAT = "puzzle/puzzle_{0}_{1}";
    const string BOOK_IMAGE_FORMAT = "UI/book_{0}";
    const string BOOK_IMAGE_UNABLED = "UI/book_unable";

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        UpdateUI();
    }
    void OnClick()
    {
        DataManager.instance.episodeId = _data.episodeId;
        MySceneManager.Instance.SetLoadSceneState(SceneState.SelectLevel);
        MySceneManager.Instance.LoadScene();
    }

    public void UpdateTitle(string title)
    {
        _data.title = title;
        titleText.text = title;
    }

    public void SetData(CellData data)
    {
        _data = data;        
    }

    void UpdateUI()
    {
        button.interactable = !_data.isLock;
        titleText.text = _data.title;

        string fileName;
        if (!_data.isLock)
        {
            
            if (_data.isUnlockCG)
            {
                List<Hashtable> levelList = LoadExcel.instance.getObjectList("all", "episodeId", _data.episodeId.ToString());
                fileName = string.Format(PUZZLE_IMAGE_FORMAT, _data.episodeId, levelList.Count + 1);
            }
            else
                fileName = string.Format(PUZZLE_IMAGE_FORMAT, _data.episodeId, 1);
            StartCoroutine(ResManager.asyncLoadSprite(fileName, (sprite) => {
                puzzleImage.sprite = sprite;
            }));
        }
        
        fileName = _data.isLock ? BOOK_IMAGE_UNABLED : string.Format(BOOK_IMAGE_FORMAT, _data.episodeId);
        StartCoroutine(ResManager.asyncLoadSprite(fileName, (sprite) => {
            bookImage.sprite = sprite;
        }));
    }
}
