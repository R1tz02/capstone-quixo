using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.Contracts;
//using UnityEditor.Overlays;

public class AiPauseButton : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas helpMenu;
    public Button pauseButton;
    public GameObject gameMaster;
    public Canvas drawAccepted;
    public Canvas drawDenied;

    // Start is called before the first frame update
    void Start()
    {
        drawAccepted.enabled = false;
        drawDenied.enabled = false;
        pauseMenu.enabled = false;
        helpMenu.enabled = false;
    }

    public void openMenu()
    { 
        pauseMenu.enabled = true;
        pauseButton.gameObject.SetActive(false);
        Time.timeScale = 0;
        gameMaster.GetComponent<AiGameCore>().gamePaused = true;
    }

    public void closeMenu() 
    { 
        pauseMenu.enabled = false;
        pauseButton.gameObject.SetActive(true);
        Time.timeScale = 1;
        gameMaster.GetComponent<AiGameCore>().gamePaused = false;
    }

    public async void returnToMain()
    {
        GameObject networkingManger = GameObject.Find("NetworkManager");

        // Disconnect from photon if the game is being played online
        if (networkingManger != null && networkingManger.GetComponent<NetworkingManager>()._runner)
        {
            await networkingManger.GetComponent<NetworkingManager>().DisconnectFromPhoton();
        }

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
    }

    public void restartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void requestDraw()
    {
        gameMaster.GetComponent<AiGameCore>().gamePaused = true;
        if (gameMaster.GetComponent<AiGameCore>().drawAccepted())
        {
            acceptDraw();
        }
        else
        {
            denyDraw();
        }
    }

    public void acceptDraw()
    {
        drawAccepted.enabled = true;
    }

    public void denyDraw()
    {
        drawDenied.enabled = true;
    }

    public void closeDrawMenu()
    {
        drawDenied.enabled = false;
        gameMaster.GetComponent<AiGameCore>().gamePaused = false;
    }
}
