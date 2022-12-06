using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    Button button;
    const string CONNECT_SCENE_NAME = "GameScene";
    LevelData _data;
    public Image levelImage;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        UpdateUI();
    }
    void OnClick()
    {
        MySceneManager.Instance.SetLoadSceneParameter(_data.levelId);
        MySceneManager.Instance.SetLoadSceneName(CONNECT_SCENE_NAME);//TODO set game state?
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
            string fileName = string.Format("puzzle/puzzle_{0}_{1}", _data.episodeId, _data.levelId);
            levelImage.sprite = Resources.Load<Sprite>(fileName);
        }
    }
}
