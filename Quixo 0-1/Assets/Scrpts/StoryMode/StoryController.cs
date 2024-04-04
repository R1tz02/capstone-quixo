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



    void Start()
    {
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

    public void restart() 
    {
        //StoryGameCore gc;

        int lvl;
        GameObject gm = GameObject.Find("GameMaster");
        lvl = gm.GetComponent<StoryGameCore>().SMLvl;
        StoryGameCore comp;
        GameObject GM = GameObject.Find("GameMaster");
        comp = GM.GetComponent<StoryGameCore>();
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            StoryGameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<StoryGameCore>();
            Time.timeScale = 1;
            if (gcComponent != null)
            {
                gcComponent.SMLvl = lvl;
                gcComponent.StartStoryGame(comp.playHard);
                gcComponent.gamePaused = false;
                GameObject.Find("Menu Manager").GetComponent<StoryPauseButton>().pauseButton.gameObject.SetActive(false);
                //gcComponent.openDialogMenu();
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void StoryModeLevel1()
    {
        StoryGameCore comp;
        GameObject gm = GameObject.Find("GameMaster");
        comp = gm.GetComponent<StoryGameCore>();
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            Time.timeScale = 1;
            StoryGameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<StoryGameCore>();
            if (gcComponent != null)
            {
                gcComponent.SMLvl = 1;
                gcComponent.StartStoryGame(comp.playHard);
                gcComponent.gamePaused = false;
                GameObject.Find("Menu Manager").GetComponent<StoryPauseButton>().pauseButton.gameObject.SetActive(false);
                //gcComponent.openDialogMenu();
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }
    public void StoryModeLevel2()
    {
        StoryGameCore comp;
        GameObject gm = GameObject.Find("GameMaster");
        comp = gm.GetComponent<StoryGameCore>();
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            Time.timeScale = 1;
            StoryGameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<StoryGameCore>();
            if (gcComponent != null)
            {
                gcComponent.SMLvl = 2;
                gcComponent.StartStoryGame(comp.playHard);
                gcComponent.gamePaused = false;
                GameObject.Find("Menu Manager").GetComponent<StoryPauseButton>().pauseButton.gameObject.SetActive(false);
                //gcComponent.openDialogMenu();
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void StoryModeLevel3()
    {
        StoryGameCore comp;
        GameObject gm = GameObject.Find("GameMaster");
        comp = gm.GetComponent<StoryGameCore>();
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            Time.timeScale = 1;
            StoryGameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<StoryGameCore>();
            gcComponent.gamePaused = false;
            if (gcComponent != null)
            {
                gcComponent.SMLvl = 3;
                gcComponent.StartStoryGame(comp.playHard);
                gcComponent.gamePaused = false;
                GameObject.Find("Menu Manager").GetComponent<StoryPauseButton>().pauseButton.gameObject.SetActive(false);
                //gcComponent.openDialogMenu();
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void StoryModeLevel4()
    {
        StoryGameCore comp;
        GameObject gm = GameObject.Find("GameMaster");
        comp = gm.GetComponent<StoryGameCore>();
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            Time.timeScale = 1;
            StoryGameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<StoryGameCore>();
            gcComponent.gamePaused = false;
            if (gcComponent != null)
            {
                gcComponent.SMLvl = 4;
                gcComponent.StartStoryGame(comp.playHard);
                gcComponent.gamePaused = false;
                GameObject.Find("Menu Manager").GetComponent<StoryPauseButton>().pauseButton.gameObject.SetActive(false);
               // gcComponent.openDialogMenu();
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
