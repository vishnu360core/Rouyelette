using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    static SceneController instance;
    public static SceneController Instance { get { return instance; } }

    Loader _loader;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);  
    }

    public void LoadLoadingScene()
    {
        SceneManager.LoadScene("LoadScene");
    }


    public void LoadGameScene(Loader loader)
    {
        _loader = loader;

        StartCoroutine(LoadLevel("GameScene"));
    }

    IEnumerator LoadLevel(string levelName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelName);


        while (!loadOperation.isDone) 
        { 
           float progressValue =  Mathf.Clamp01(loadOperation.progress / 0.5f);

            if(_loader != null)
            {
                _loader.SliderValueAction(progressValue);
            }

            yield return null; 
           
        }
    }
}
