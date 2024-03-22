using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEditor.Overlays;

public class PauseButton : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas helpMenu;
    public Button pauseButton;
    public GameObject gameMaster;

    // Start is called before the first frame update
    void Start()
    {        
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
        if (pauseMenu)
        {
            pauseMenu.enabled = false;
            pauseButton.gameObject.SetActive(true);
            Time.timeScale = 1;
            gameMaster.GetComponent<GameCore>().gamePaused = false;
        }
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
        MenuController menuController = gameObject.GetComponent("MenuController") as MenuController;
        Time.timeScale = 1;
        switch (gameMaster.GetComponent<GameCore>().currentGameMode)
        {
            case GameType.AIEasy:
                menuController.NewEasyGame();
                break;
            case GameType.AIHard:
                break;
            case GameType.Local:
                menuController.LocalGame();
                break;
            case GameType.Online:
                break;
        }
       //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
