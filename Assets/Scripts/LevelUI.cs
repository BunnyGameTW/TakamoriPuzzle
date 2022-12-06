using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    int[] sections = {1, 2, 4, 1};
    // Start is called before the first frame update
    void Start()
    {
        levelDatas = new LevelData[8];
        int index = 0, counter = 0;
        for (int i = 0; i < levelDatas.Length; i++)
        {
            //test data
            levelDatas[i].episodeId = 1;//TODO ask manager?
            levelDatas[i].levelId = i + 1;
            levelDatas[i].isLock = i > 2;

            //create buttons
            if (i == 0) 
            { 
                //直接在場景上擺好?
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
    }

    private void Update()
    {
        
    }
}
