using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MenuController : MonoBehaviour
{
    public Transform mMenuLocation;
    public Camera currentCam;
    public Canvas quickCanvas;
    public Canvas multiCanvas;

    public float moveDuration = 1f;
    public float rotaionDuration = 1f;

    bool rotating;
    bool moving;

    
    void Start()
    {
        quickCanvas.enabled = false;
        multiCanvas.enabled = false;
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
        SceneManager.LoadScene(1);
    }
}
