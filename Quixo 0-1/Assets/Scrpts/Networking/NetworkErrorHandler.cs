using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkErrorHandler : MonoBehaviour
{
    void Start()
    {
        NetworkingManager.OnNetworkError += HandleError;
    }

    private void HandleError(ShutdownReason shutdownReason)
    {
        DontDestroyOnLoad(this.gameObject);

        if (shutdownReason == ShutdownReason.DisconnectedByPluginLogic)
        {
            LoadScene("Host disconnected");
        }
        else if (shutdownReason == ShutdownReason.GameNotFound)
        {
            LoadScene("Game not found");
        }
        else if (shutdownReason == ShutdownReason.OperationCanceled)
        {
            LoadScene("Client disconnected");
        }
        else if (shutdownReason == ShutdownReason.IncompatibleConfiguration)
        {
            LoadScene("A draw request was in progress but a player left");
        }
        else
        {
            LoadScene("A network error occurred");
        }

    }

    private void LoadScene(string text)
    {
        StartCoroutine(AsyncLoadGameScene(1, () =>
        {
            MenuController menuController = GameObject.Find("Game Manager").GetComponent<MenuController>();

            menuController.displayError(text);
        }));
    }

    public IEnumerator AsyncLoadGameScene(int sceneToLoad, Action onSceneLoaded)
    {
        DontDestroyOnLoad(this.gameObject);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Scene load operation completed");

        onSceneLoaded?.Invoke();
    }

    void OnDestroy()
    {
        NetworkingManager.OnNetworkError -= HandleError;
    }
}
