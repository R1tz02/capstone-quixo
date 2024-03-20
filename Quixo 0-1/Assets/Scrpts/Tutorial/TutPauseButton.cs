using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.Contracts;
//using UnityEditor.Overlays;

public class TutPauseButton : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas helpMenu;
    public Button pauseButton;
    public GameObject gameMaster;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.enabled = false;
        helpMenu.enabled = true;
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
        pauseMenu.enabled = true;
        closeMenu();
    }

    public void restartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
