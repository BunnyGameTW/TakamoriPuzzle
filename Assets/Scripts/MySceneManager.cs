using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MySceneManager : MonoBehaviour
{
    string loadSceneName;
    int loadSceneParameterInt;
    bool isLoading;
    Animator ani;
    const string DEFAULT_LOAD_SCENE_NAME = "SelectEpisodeScene";
    const string DEFAULT_SCENE_NAME = "LoginScene";
    static MySceneManager instance;
    public static MySceneManager Instance
    {
        get
        {
            //if (instance == null)
            return instance;
        }
    }

    public void SetLoadSceneName(string name)
    {
        loadSceneName = name;
    }
    public void SetLoadSceneParameter(int i)
    {
        loadSceneParameterInt = i;
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        loadSceneName = DEFAULT_LOAD_SCENE_NAME;
        isLoading = false;
        ani = GetComponentInChildren<Animator>();               
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) && (SceneManager.GetActiveScene().name == DEFAULT_SCENE_NAME))
        {
            LoadScene();           
        }
    }

    public void LoadScene()
    {
        if (isLoading)
            return;

        isLoading = true;
        //ani.SetTrigger("In");        
        ani.SetInteger("state", 1);
        StartCoroutine(LoadYourAsyncScene());
    }
    
    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneName);
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
            isLoading = false;
        }
    }
}
