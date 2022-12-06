using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    // Start is called before the first frame update
    void Start()
    {
        //cellData
        storyDatas = new CellData[4];

        GameObject goCell, goBook, bookPrefab;
        goCell = Instantiate(cellPrefab, scrollContent.transform);
        bookPrefab = goCell.GetComponentInChildren<Button>().gameObject;

        for (int i = 0; i < storyDatas.Length; i++)
        {
            storyDatas[i].title = i.ToString();
            storyDatas[i].episodeId = i + 1;
            storyDatas[i].isLock = i != 0;

            if (i % 2 == 0)
            {
                if (i != 0)
                {
                    goCell = Instantiate(cellPrefab, scrollContent.transform);
                }
                goBook = goCell.GetComponentInChildren<Button>().gameObject;                
            }
            else
            {
                goBook = Instantiate(bookPrefab, goCell.transform.GetChild(0));
            }
            goBook.GetComponent<Book>().SetData(storyDatas[i]);
        }                
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonEvent(string name)
    {

    }
}
