using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    Button button;    
    LevelData _data;
    public Image levelImage;
    const string IMAGE_FORMAT = "puzzle/puzzle_{0}_{1}";

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        UpdateUI();
    }
    void OnClick()
    {
        DataManager.instance.setLevelId(_data.levelId);
        DataManager.instance.puzzleGridX = 3;
        DataManager.instance.puzzleGridY = 3;
        MySceneManager.Instance.SetLoadSceneState(SceneState.Game);
        MySceneManager.Instance.LoadScene();
    }
    public void SetData(LevelData data)
    {
        _data = data;      
    }

    void UpdateUI()
    {
        button.interactable = !_data.isLock;

        if (!_data.isLock)
        {
            string fileName = string.Format(IMAGE_FORMAT, _data.episodeId, _data.levelId);
            levelImage.sprite = Resources.Load<Sprite>(fileName);
        }
    }
}
