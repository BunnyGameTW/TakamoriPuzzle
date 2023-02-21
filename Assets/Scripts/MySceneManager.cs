using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum SceneState
{
    Login = 0,
    SelectEpisode = 1,
    SelectLevel = 2,
    Game = 3
}
public class MySceneManager : MonoBehaviour
{
    bool isLoading;
    Animator ani;
    SceneState sceneState, preSceneState;
    Dictionary<SceneState, string> sceneNameMap;
    const string DEFAULT_LOAD_SCENE_NAME = "SelectEpisodeScene";
    const string SELECT_LEVEL_SCENE_NAME = "SelectLevelScene";
    const string GAME_SCENE_NAME = "GameScene";
    const string DEFAULT_SCENE_NAME = "LoginScene";
    public Button buttonBack;
    public GameObject gameObjectButtons;
    static MySceneManager instance;
    public static MySceneManager Instance
    {
        get
        {
            //if (instance == null)
            return instance;
        }
    }
    public void SetLoadSceneState(SceneState _state)
    {
        if (isLoading)
            return;
        preSceneState = sceneState;
        sceneState = _state;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void OnClickBack()
    {
        if (isLoading)
            return;
        switch (sceneState)
        {
            case SceneState.Login:
                return;
            case SceneState.SelectEpisode:
                SetLoadSceneState(SceneState.Login);
                break;
            case SceneState.SelectLevel:
                SetLoadSceneState(SceneState.SelectEpisode);
                break;
            case SceneState.Game:
                SetLoadSceneState(SceneState.SelectLevel);
                break;
            default:
                return;                
        }
        
        LoadScene();
    }

    // Start is called before the first frame update
    void Start()
    {       
        isLoading = false;
        sceneNameMap = new Dictionary<SceneState, string>();
        sceneNameMap.Add(SceneState.Login, DEFAULT_SCENE_NAME);
        sceneNameMap.Add(SceneState.SelectEpisode, DEFAULT_LOAD_SCENE_NAME);
        sceneNameMap.Add(SceneState.SelectLevel, SELECT_LEVEL_SCENE_NAME);
        sceneNameMap.Add(SceneState.Game, GAME_SCENE_NAME);

        sceneState = SceneState.Login;
        gameObjectButtons.SetActive(false);
        ani = GetComponentInChildren<Animator>();
        buttonBack.onClick.AddListener(OnClickBack);

        LoadExcel.instance.loadFile(RES_PATH.PUZZLE_EXCEL);
        SoundManager.instance.playBGM(SoundManager.instance.BGM_title);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (sceneState == SceneState.Login))
        {            
            SetLoadSceneState(SceneState.SelectEpisode);
            LoadScene();           
        }
    }

    public void ShowButtons(bool show)
    {
        gameObjectButtons.SetActive(show);
    }
    public void LoadScene()
    {
        if (isLoading)
            return;

        isLoading = true;
        StartCoroutine(LoadYourAsyncScene());
        SoundManager.instance.playSE(SoundManager.instance.SE_transitionIn, 0.5f);
    }
    
    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNameMap[sceneState]);
        yield return updateLoadSceneStart(asyncLoad);
        yield return updateLoadSceneComplete(asyncLoad);
        yield return updateLoadSceneEnd();
        isLoading = false;
    }

    /** 更新切換場景開始 */
    IEnumerator updateLoadSceneStart(AsyncOperation asyncLoad) {
        asyncLoad.allowSceneActivation = false;
        ani.SetInteger("state", 1);
        yield return null;
        while (ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) {
            yield return null;
        }
    }

    /** 更新切換場景成功 */
    IEnumerator updateLoadSceneComplete(AsyncOperation asyncLoad) {
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone) {            
            yield return null;
        }

        gameObjectButtons.SetActive(sceneState != SceneState.Login);
        switch(sceneState) {
            case SceneState.Login:
            case SceneState.SelectEpisode: {
                SoundManager.instance.playBGM(SoundManager.instance.BGM_title);
            } break;
            case SceneState.SelectLevel:
            case SceneState.Game: {
                SoundManager.instance.playBGM(Config.instance.episodeList[DataManager.instance.episodeId].bgm);
            } break;
            default: {
                SoundManager.instance.playBGM(SoundManager.instance.BGM_title);
            } break;
        }
        SoundManager.instance.playSE(SoundManager.instance.SE_transitionOut, 0.5f);
    }

    /** 更新切換場景結束 */
    IEnumerator updateLoadSceneEnd() {
        ani.SetInteger("state", 2);
        yield return null;
        while (ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f) {
            yield return null;
        }
    }
}
