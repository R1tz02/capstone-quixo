using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.Contracts;
using System;
//using UnityEditor.Overlays;

public class TutPauseButton : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas helpMenu;
    public Button pauseButton;
    public GameObject gameMaster;
    public Canvas learningModes;

    // Start is called before the first frame update
    void Start()
    {
        learningModes.enabled = true;
        pauseMenu.enabled = false;
        pauseButton.gameObject.SetActive(false);
        helpMenu.enabled = true;

        gameMaster.GetComponent<TutGameCore>().gamePaused = true;
        Time.timeScale = 0;
    }

    public void tryLeft()
    {
        gameMaster.GetComponent<TutGameCore>().tutLvl = 0;
        learningModes.enabled = false;
        closeMenu();
    }

    public void tryRight()
    {
        gameMaster.GetComponent<TutGameCore>().tutLvl = 1;
        learningModes.enabled = false;
        closeMenu();
    }

    public void tryHori()
    {
        gameMaster.GetComponent<TutGameCore>().tutLvl = 2;
        learningModes.enabled = false;
        closeMenu();
    }

    public void tryVer()
    {
        gameMaster.GetComponent<TutGameCore>().tutLvl = 3;
        learningModes.enabled = false;
        closeMenu();
    }

    public void tryDifMode()
    {
        restartGame();
        learningModes.enabled = true;
        gameMaster.GetComponent<TutGameCore>().winScreen.enabled = false;
    }

    public void openMenu()
    { 
        pauseMenu.enabled = true;
        pauseButton.gameObject.SetActive(false);
        Time.timeScale = 0;
        gameMaster.GetComponent<TutGameCore>().gamePaused = true;
    }

    public void closeMenu() 
    { 
        pauseMenu.enabled = false;
        pauseButton.gameObject.SetActive(true);
        Time.timeScale = 1;
        gameMaster.GetComponent<TutGameCore>().gamePaused = false;
    }

    public async void returnToMain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void openHelpMenu()
    {
        pauseMenu.enabled = false;
        helpMenu.enabled = true;
    }

    public void closeHelpMenu()
    {
        helpMenu.enabled = false;
        pauseButton.gameObject.SetActive(true);
    }

    public void restartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        closeHelpMenu();
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
