using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.Contracts;
//using UnityEditor.Overlays;

public class PauseButton : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas helpMenu;
    public Button pauseButton;
    public GameObject gameMaster;
    public Canvas drawReqScreen;
    public Canvas drawAccepted;
    public Canvas drawDenied;

    // Start is called before the first frame update
    void Start()
    {
        drawDenied.enabled = false;
        drawReqScreen.enabled = false;
        drawAccepted.enabled = false;
        pauseMenu.enabled = false;
        helpMenu.enabled = false;
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
        drawReqScreen.enabled = true;
        gameMaster.GetComponent<GameCore>().gamePaused = true;
    }

    public void acceptDraw()
    {
        drawReqScreen.enabled = false;
        drawAccepted.enabled = true;
    }

    public void denyDraw()
    {
        drawReqScreen.enabled = false;
        drawDenied.enabled = true;
    }

    public void closeDrawMenu()
    {
        drawDenied.enabled = false;
        gameMaster.GetComponent<GameCore>().gamePaused = false;
    }
}
