using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;

public class StoryController : MonoBehaviour
{
    public Camera currentCam;
    public void StoryModeLevel2()
    {
        StartCoroutine(AsyncLoadGameScene(() =>
        {
            GameCore gcComponent;
            GameObject gameMaster = GameObject.Find("GameMaster");
            gcComponent = gameMaster.GetComponent<GameCore>();
            if (gcComponent != null)
            {
                gcComponent.StartAIGame();
                gcComponent.SMLvl=2;
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
