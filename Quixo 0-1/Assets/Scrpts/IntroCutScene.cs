using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutScene : MonoBehaviour
{
    public GameObject logoText;
    public GameObject logo;
    public Camera curCamera;
    public GameObject targetObject;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startCutScene());
    }

    IEnumerator startCutScene()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(showLogo(logo));
        yield return new WaitForSeconds(1);
        StartCoroutine(showLogo(logoText));
        yield return new WaitForSeconds(1);
        StartCoroutine(cameraDown());
        yield return new WaitForSeconds(3);
        StartCoroutine(AsyncLoadGameScene());

    }
    IEnumerator showLogo(GameObject invisObject)
    {
        Color visable = Color.white;
        Color transparent = Color.white;
        transparent.a = 0f;
        float duration = 1f;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            invisObject.GetComponent<SpriteRenderer>().color = Color.Lerp(transparent, visable, normalizedTime);
            yield return null;
        }
        invisObject.GetComponent<SpriteRenderer>().color = visable; //without this, the value will end at something like 0.9992367
    }

    IEnumerator cameraDown()
    {
        Quaternion startRotation = curCamera.transform.rotation;
        Quaternion targetRotation = targetObject.transform.rotation;

        float duration = 3f;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            curCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, normalizedTime);
            yield return null;
        }
        curCamera.transform.rotation = targetRotation;
    }

    public IEnumerator AsyncLoadGameScene()
    {
        // Needed so that the callbacks can be called after the scene is loaded
        DontDestroyOnLoad(this.gameObject);

        Debug.Log("Loading game scene...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Destroy(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
