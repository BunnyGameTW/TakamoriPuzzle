using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public struct CellData
{
    public int episodeId;
    public string title;
    public bool isLock;
}

public class EpisodeUI : MonoBehaviour
{
    public GameObject cellPrefab, scrollContent;
    CellData[] storyDatas;
    Book[] books;
    const string EXCEL_TAG_NAME = "episodeTitle";
    const string INDEX_NAME = "id";
    const string IS_LOCK_NAME = "isLock";
    const string TITLE_SUFFIX_NAME = "_title";
    const int BOOK_PER_SHELF = 1;
    const int HEIGHT_PER_BOOK = 900;
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string, Hashtable> allTable = LoadExcel.instance.getTable(EXCEL_TAG_NAME);
        storyDatas = new CellData[allTable.Count];
        books = new Book[allTable.Count];

        DataManager.instance.LanguageChanged += OnLanguageChanged;

        GameObject goCell, goBook, bookPrefab;
        goCell = Instantiate(cellPrefab, scrollContent.transform);
        bookPrefab = goCell.GetComponentInChildren<Button>().gameObject;

        string language = DataManager.instance.getLanguageName();
        RectTransform rectTransform = scrollContent.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, HEIGHT_PER_BOOK * storyDatas.Length);

        for (int i = 0; i < storyDatas.Length; i++)
        {
            //set data
            Hashtable data = allTable[(i + 1).ToString()];
            storyDatas[i].episodeId = Convert.ToInt32(data[INDEX_NAME]);
            storyDatas[i].isLock = Convert.ToBoolean(data[IS_LOCK_NAME]);
            if (!storyDatas[i].isLock)
            {                
                string message = (string)data[language + TITLE_SUFFIX_NAME];
                storyDatas[i].title = message;
            }

            //create ui
            //if (i % 2 == 0)
            //{
            if (i != 0)
            {
                goCell = Instantiate(cellPrefab, scrollContent.transform);
            }
            goBook = goCell.GetComponentInChildren<Button>().gameObject;                
            //}
            //else
            //{
            //    goBook = Instantiate(bookPrefab, goCell.transform.GetChild(0));
            //}

            //set ui
            books[i] = goBook.GetComponent<Book>();
            books[i].SetData(storyDatas[i]);
        }                
    }
    private void OnDestroy()
    {
        DataManager.instance.LanguageChanged -= OnLanguageChanged;
    }

    void OnLanguageChanged(object sender, string languageCode)
    {
        for (int i = 0; i < books.Length; i++)
        {
            if (!storyDatas[i].isLock)
            {
                Hashtable data = LoadExcel.instance.getObject(EXCEL_TAG_NAME, INDEX_NAME, i + 1);
                string title = (string)data[languageCode + TITLE_SUFFIX_NAME];
                books[i].UpdateTitle(title);
            }
        }
        
    }
}
