using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.Contracts;

public class PauseButton : MonoBehaviour
{
    public Canvas pauseMenu;
    public Button pauseButton;
    public GameObject gameMaster;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.enabled = false;
    }

    public void openMenu()
    { 
        pauseMenu.enabled = true;
        pauseButton.gameObject.SetActive(false);
        Time.timeScale = 0;
        gameMaster.GetComponent<GameCore>().gamePaused = true;
    }

    public void closeMenu() 
    { 
        pauseMenu.enabled = false;
        pauseButton.gameObject.SetActive(true);
        Time.timeScale = 1;
        gameMaster.GetComponent<GameCore>().gamePaused = false;
    }

    public void returnToMain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);   
    }

    public void restartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void openHelp()
    { 
    }
}
