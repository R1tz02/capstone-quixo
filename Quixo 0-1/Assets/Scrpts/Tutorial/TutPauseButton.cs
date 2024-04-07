using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//using UnityEditor.Overlays;

public class TutPauseButton : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas helpMenu;
    public Button pauseButton;
    public GameObject gameMaster;
    public Canvas learningModes;
    public Canvas stepOne;
    public Canvas stepTwo;
    public int stepDisabled;

    // Start is called before the first frame update
    void Start()
    {
        learningModes.enabled = true;
        pauseMenu.enabled = false;
        pauseButton.gameObject.SetActive(false);
        stepOne.enabled = false;
        stepTwo.enabled = false;

        gameMaster.GetComponent<TutGameCore>().gamePaused = true;
        Time.timeScale = 0;
    }

    public void tryLeft()
    {
        gameMaster.GetComponent<TutGameCore>().tutLvl = 0;
        learningModes.enabled = false;
        closeMenu();
        stepOne.enabled = true;
    }

    public void tryRight()
    {
        gameMaster.GetComponent<TutGameCore>().tutLvl = 1;
        learningModes.enabled = false;
        closeMenu();
        stepOne.enabled = true;
    }

    public void tryHori()
    {
        gameMaster.GetComponent<TutGameCore>().tutLvl = 2;
        learningModes.enabled = false;
        closeMenu();
        stepOne.enabled = true;
    }

    public void tryVer()
    {
        gameMaster.GetComponent<TutGameCore>().tutLvl = 3;
        learningModes.enabled = false;
        closeMenu();
        stepOne.enabled = true;
    }

    public void tryDifMode()
    {
        bool difMode = true;
        StartCoroutine(AsyncLoadGameScene(3, () =>
        {
            if (gameMaster != null)
            {
                learningModes.enabled = true;
                gameMaster.GetComponent<TutGameCore>().StartTutorial(difMode);

            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void openMenu()
    {
        if (stepOne.enabled)
        {
            stepOne.enabled = false;
            stepDisabled = 1;
        }
        else if (stepTwo.enabled)
        {
            stepTwo.enabled = false;
            stepDisabled = 2;
        }
        pauseMenu.enabled = true;

        pauseButton.gameObject.SetActive(false);
       // Time.timeScale = 0;
        gameMaster.GetComponent<TutGameCore>().gamePaused = true;
        GameObject.Find("GameMaster").GetComponent<TutGameCore>().buttonCanvas.enabled = false;
    }

    public void closeMenu() 
    { 
        if(stepDisabled == 1) { stepOne.enabled = true; }
        else if (stepDisabled == 2) { stepTwo.enabled = true; GameObject.Find("GameMaster").GetComponent<TutGameCore>().buttonCanvas.enabled = true; }
        pauseMenu.enabled = false;
        Time.timeScale = 1;
        pauseButton.gameObject.SetActive(true);
        gameMaster.GetComponent<TutGameCore>().gamePaused = false;
        GameObject.Find("GameMaster").GetComponent<TutGameCore>().buttonCanvas.enabled = true;
    }

    public async void returnToMain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void openHelpMenu()
    {
       // pauseMenu.enabled = false;
        helpMenu.enabled = true;
    }

    public void closeHelpMenu()
    {
        helpMenu.enabled = false;
    }

    public void restartGame()
    {
        StartCoroutine(AsyncLoadGameScene(3, () =>
        {
            if (gameMaster != null)
            {
                gameMaster.GetComponent<TutGameCore>().StartTutorial();
                
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public IEnumerator AsyncLoadGameScene(int sceneToLoad, Action onSceneLoaded)
        {
            // Needed so that the callbacks can be called after the scene is loaded
            DontDestroyOnLoad(this.gameObject);

            Debug.Log("Loading game scene...");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            onSceneLoaded?.Invoke();


            Destroy(this.gameObject);
        }
}
