using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LoginUI : MonoBehaviour
{
    public GameObject gameObjectProgress;
    public TextMeshProUGUI textProgress;
    const string PROGRESS_STRING_FORMAT = "Ep.{0}-{1}";
    // Start is called before the first frame update
    void Start()
    {
        int episodeId = DataManager.instance.episodeId;
        int levelId = DataManager.instance.levelId;
        bool needShowProgress = episodeId != 0 && levelId != 0;
        if (gameObjectProgress.activeSelf != needShowProgress)
            gameObjectProgress.SetActive(needShowProgress);
        if (needShowProgress)
            textProgress.text = string.Format(PROGRESS_STRING_FORMAT, episodeId, levelId);        
    }
}
