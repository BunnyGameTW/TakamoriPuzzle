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
    SceneState sceneState;
    Dictionary<SceneState, string> sceneNameMap;
    const string DEFAULT_LOAD_SCENE_NAME = "SelectEpisodeScene";
    const string SELECT_LEVEL_SCENE_NAME = "SelectLevelScene";
    const string GAME_SCENE_NAME = "GameScene";
    const string DEFAULT_SCENE_NAME = "LoginScene";
    public Button buttonBack, buttonMenu;
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
    void OnClickMenu()
    {
        //TODO open setting
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
        buttonMenu.onClick.AddListener(OnClickMenu);
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (sceneState == SceneState.Login))
        {            
            SetLoadSceneState(SceneState.SelectEpisode);
            LoadScene();           
        }
    }

    public void LoadScene()
    {
        if (isLoading)
            return;

        isLoading = true;
        ani.SetInteger("state", 1);
        StartCoroutine(LoadYourAsyncScene());
    }
    
    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNameMap[sceneState]);
        asyncLoad.allowSceneActivation = false;
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {            
            yield return null;

            StartCoroutine(LoadSceneCallback(asyncLoad));

        }
    }


    IEnumerator LoadSceneCallback(AsyncOperation asyncLoad)
    {
     
        while (ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            yield return null;

            ani.SetInteger("state", 2);
            asyncLoad.allowSceneActivation = true;            
            gameObjectButtons.SetActive(sceneState != SceneState.Login);
            isLoading = false;
        }
    }
}
