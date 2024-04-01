using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;

public class MenuController : MonoBehaviour
{
    public Transform mMenuLocation;
    public Camera currentCam;
    public Canvas quickCanvas;
    public Canvas multiCanvas;
    public Canvas storyCanvas;
    public Canvas hostJoinCanvas;
    public Canvas HelpCanvas;
    public Canvas overlayCanvas;
    public Canvas joinLobbyCanvas;
    public Canvas tutorialCanvas;
    public Canvas labelCanvas;
    public Canvas errorCanvas;
    public Text errorText;

    public float moveDuration;
    public float rotaionDuration;

    bool rotating;
    bool moving;

    
    void Start()
    {
        if(labelCanvas)
            labelCanvas.enabled = true;
        if(tutorialCanvas)
            tutorialCanvas.enabled = false;
        if (quickCanvas)
            quickCanvas.enabled = false;
        if (multiCanvas)
            multiCanvas.enabled = false;
        if(storyCanvas)
            storyCanvas.enabled = false;
        if(hostJoinCanvas)
            hostJoinCanvas.enabled = false;
        if(HelpCanvas)
            HelpCanvas.enabled = false;
        if(joinLobbyCanvas)
            joinLobbyCanvas.enabled = false;
        if(errorCanvas)
            errorCanvas.enabled = false;
    }


    public void displayError(string error)
    {
        errorText.text = error;
        errorCanvas.enabled = true;
    }

    public void closeError()
    { 
        errorCanvas.enabled = false;
    }


    public void HostJoin()
    {
        multiCanvas.enabled = false;
        hostJoinCanvas.enabled = true;
    }

    public void JoinLobby()
    {
        hostJoinCanvas.enabled = false;
        joinLobbyCanvas.enabled = true;
    }

    public void openTutorialMenu()
    {
        tutorialCanvas.enabled = true;
        Time.timeScale = 0;
        overlayCanvas.enabled = false;
    }

    public void closeTutorialMenu()
    {
        tutorialCanvas.enabled = false;
        Time.timeScale = 1;
        overlayCanvas.enabled = true;
    }

    public void openHelpMenu()
    {
        Time.timeScale = 0;
        HelpCanvas.enabled = true;
        overlayCanvas.enabled = false;
    }

    public void closeHelpMenu()
    {
        Time.timeScale = 1;
        HelpCanvas.enabled = false;
        overlayCanvas.enabled = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator MoveToLocation()
    {
        moving = true;
        float timeElapsed = 0;

        while (timeElapsed < 1)
        {
            currentCam.transform.position = Vector3.Lerp(currentCam.transform.position, mMenuLocation.position, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            Debug.Log("1");
            yield return null;
        }
        currentCam.transform.position = mMenuLocation.position;
        moving = false;
   }

    IEnumerator RotateUp()
    {
        rotating = true;
        float timeElapsed = 0;

        Quaternion startRotation = currentCam.transform.rotation;
        Quaternion targetRotation = mMenuLocation.transform.rotation;
        while (timeElapsed < rotaionDuration)
        {
            currentCam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / rotaionDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        currentCam.transform.rotation = targetRotation;
        rotating = false;
    }
    public void goBack()
    {
        quickCanvas.enabled = false;
        multiCanvas.enabled = false;
        storyCanvas.enabled = false;
        joinLobbyCanvas.enabled = false;
        hostJoinCanvas.enabled = false;
        labelCanvas.enabled = true;

        if (!moving)
        {
            StartCoroutine(MoveToLocation());
        }
        if (!rotating)
        {
            StartCoroutine(RotateUp());
        }
    }

    public void loadTutorial() //need Jack to map this to the scene I need
    {
        StartCoroutine(AsyncLoadGameScene(2, () =>
        {
            GameObject gameMaster = GameObject.Find("GameMaster");
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

    public void NewEasyGame()
    {
        StartCoroutine(AsyncLoadGameScene(4, () =>
        {
            GameObject gameMaster = GameObject.Find("GameMaster");
            if (gameMaster != null)
            {
                gameMaster.GetComponent<AiGameCore>().StartAIGame();
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void StoryModeLevel1()
    {
        StartCoroutine(AsyncLoadGameScene(3, () =>
        {
            StoryGameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<StoryGameCore>();
            if (gcComponent != null)
            {
                gcComponent.StartStoryGame();
                gcComponent.SMLvl=1;
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }
    public void LocalGame()
    {
        StartCoroutine(AsyncLoadGameScene(1, () =>
        {
            GameObject gameMaster = GameObject.Find("GameMaster");
            if (gameMaster != null)
            {
                gameMaster.GetComponent<GameCore>().StartLocalGame();
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void JoinNetworkedGame()
    {
        string code = GameObject.Find("enterCode").GetComponent<InputField>().text;
        StartCoroutine(AsyncLoadGameScene(1, () =>
        {
            Debug.Log("Looking for GameMaster object...");
            GameObject gameMaster = GameObject.Find("GameMaster");
            if (gameMaster != null)
            {
                Debug.Log("GameMaster found. Starting networked game...");
                gameMaster.GetComponent<GameCore>().StartNetworkedGame("Client", code);
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void HostNetworkedGame()
    {
        StartCoroutine(AsyncLoadGameScene(1, () =>
        {
            Debug.Log("Looking for GameMaster object...");
            GameObject gameMaster = GameObject.Find("GameMaster");
            if (gameMaster != null)
            {
                Debug.Log("GameMaster found. Starting networked game...");
                gameMaster.GetComponent<GameCore>().StartNetworkedGame("Host");
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

    public void QuickPlayGame()
    {
        StartCoroutine(AsyncLoadGameScene(1, () =>
        {
            Debug.Log("Looking for GameMaster object...");
            GameObject gameMaster = GameObject.Find("GameMaster");
            if (gameMaster != null)
            {
                Debug.Log("GameMaster found. Starting networked game...");
                gameMaster.GetComponent<GameCore>().StartNetworkedGame("AutoHostOrClient");
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }
}
