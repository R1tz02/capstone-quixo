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

    public float moveDuration = 1f;
    public float rotaionDuration = 1f;

    bool rotating;
    bool moving;

    
    void Start()
    {
        quickCanvas.enabled = false;
        multiCanvas.enabled = false;
        storyCanvas.enabled = false;
        hostJoinCanvas.enabled = false;
        HelpCanvas.enabled = false;
    }

    public void HostJoin()
    {
        multiCanvas.enabled = false;
        hostJoinCanvas.enabled = true;
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

        while (timeElapsed < moveDuration)
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
        if (!moving)
        {
            StartCoroutine(MoveToLocation());
        }
        if (!rotating)
        {
            StartCoroutine(RotateUp());
        }
    }

    public void NewEasyGame()
    {
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            GameObject gameMaster = GameObject.Find("GameMaster");
            if (gameMaster != null)
            {
                gameMaster.GetComponent<GameCore>().StartAIGame();
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void StoryModeLevel1()
    {
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            GameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<GameCore>();
            if (gcComponent != null)
            {
                gcComponent.StartAIGame();
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
        StartCoroutine(AsyncLoadGameScene(() =>
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
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            Debug.Log("Looking for GameMaster object...");
            GameObject gameMaster = GameObject.Find("GameMaster");
            if (gameMaster != null)
            {
                Debug.Log("GameMaster found. Starting networked game...");
                gameMaster.GetComponent<GameCore>().StartNetworkedGame("Client");
            }
            else
            {
                Debug.Log("GameMaster not found.");
            }
        }));
    }

    public void HostNetworkedGame()
    {
        StartCoroutine(AsyncLoadGameScene(() =>
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

    public IEnumerator AsyncLoadGameScene(Action onSceneLoaded)
    {
        // Needed so that the callbacks can be called after the scene is loaded
        DontDestroyOnLoad(this.gameObject);

        Debug.Log("Loading game scene...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone){
            yield return null;
        }

        onSceneLoaded?.Invoke();

        Destroy(this.gameObject);
    }
}
