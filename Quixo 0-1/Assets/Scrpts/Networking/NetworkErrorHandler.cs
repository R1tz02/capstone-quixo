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

    private bool errorHandled = false;

    private void HandleError(ShutdownReason shutdownReason)
    {
        DontDestroyOnLoad(this.gameObject);

        if (errorHandled) return;

        errorHandled = true;

        if (shutdownReason == ShutdownReason.DisconnectedByPluginLogic)
        {
            if (Data.CURRENT_LANGUAGE == "English")
            {
                LoadScene("Host disconnected");
            }
            else if (Data.CURRENT_LANGUAGE == "Español")
            {
                LoadScene("Anfitrion desconectado");
            }
        }
        else if (shutdownReason == ShutdownReason.GameNotFound)
        {
            if (Data.CURRENT_LANGUAGE == "English")
            {
                LoadScene("Game not found");
            }
            else if (Data.CURRENT_LANGUAGE == "Español")
            {
                LoadScene("Juego no encontrado");
            }
        }
        else if (shutdownReason == ShutdownReason.OperationCanceled)
        {
            if (Data.CURRENT_LANGUAGE == "English")
            {
                LoadScene("Client disconnected");
            }
            else if (Data.CURRENT_LANGUAGE == "Español")
            {
                LoadScene("Cliente desconectado");
            }
        }
        else if (shutdownReason == ShutdownReason.IncompatibleConfiguration)
        {
            if (Data.CURRENT_LANGUAGE == "English")
            {
                LoadScene("A draw request was in progress but a player left");
            }
            else if (Data.CURRENT_LANGUAGE == "Español")
            {
                LoadScene("Una solicitud de empate estaba en progreso, pero un jugador se fue");
            }
        }
        else
        {
            if (Data.CURRENT_LANGUAGE == "English")
            {
                LoadScene("A network error occurred");
            }
            else if (Data.CURRENT_LANGUAGE == "Español")
            {
                LoadScene("Un error de red ocurrio");//
            }
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
