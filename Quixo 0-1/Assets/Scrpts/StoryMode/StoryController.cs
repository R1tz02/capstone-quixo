using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;

public class StoryController : MonoBehaviour
{
    public Camera currentCam;
    public Canvas HelpCanvas;

    void Start(){
        HelpCanvas.enabled = false;
    }

    public void openHelpMenu()
    {
        Time.timeScale = 0;
        HelpCanvas.enabled = true;
    }

    public void closeHelpMenu()
    {
        Time.timeScale = 1;
        HelpCanvas.enabled = false;
    }
    public void StoryModeLevel2()
    {
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            StoryGameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<StoryGameCore>();
            gcComponent.gamePaused = false;
            if (gcComponent != null)
            {
                gcComponent.StartStoryGame();
                gcComponent.SMLvl=2;
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void StoryModeLevel3()
    {
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            StoryGameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<StoryGameCore>();
            gcComponent.gamePaused = false;
            if (gcComponent != null)
            {
                gcComponent.StartStoryGame();
                gcComponent.SMLvl=3;
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void StoryModeLevel4()
    {
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            StoryGameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<StoryGameCore>();
            gcComponent.gamePaused = false;
            if (gcComponent != null)
            {
                gcComponent.StartStoryGame();
                gcComponent.SMLvl=4;
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public IEnumerator AsyncLoadGameScene(Action onSceneLoaded)
    {
        // Needed so that the callbacks can be called after the scene is loaded
        DontDestroyOnLoad(this.gameObject);

        Debug.Log("Loading game scene...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3);

        while (!asyncLoad.isDone){
            yield return null;
        }

        onSceneLoaded?.Invoke();

        Destroy(this.gameObject);
    }
    

}