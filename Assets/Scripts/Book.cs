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
        MySceneManager.Instance.SetLoadSceneName("SelectLevelScene");
        MySceneManager.Instance.LoadScene();
    }
    public void SetData(CellData data)
    {
        _data = data;
        MySceneManager.Instance.SetLoadSceneParameter(data.episodeId);
        
    }
    void UpdateUI()
    {
        button.interactable = !_data.isLock;
        titleText.text = _data.title;

        string fileName;
        if (!_data.isLock)
        {
            fileName = string.Format("puzzle/puzzle_{0}_1", _data.episodeId);
            puzzleImage.sprite = Resources.Load<Sprite>(fileName);
        }
        fileName = _data.isLock ? "UI/book_unable" : string.Format("UI/book_{0}", _data.episodeId);
        bookImage.sprite = Resources.Load<Sprite>(fileName);        
    }
}
