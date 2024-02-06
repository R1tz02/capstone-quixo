using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class AnvilClick : MonoBehaviour
{
    public Transform startMarker;
    public Transform endMarker;
    public Transform lookAt;
    public Camera currentCam;

    public float moveDuration = 1f;
    public float rotaionDuration = 1f;

    private bool hasBeenClicked = false;
    private bool isEnabled = true;
    
    bool rotating;
    bool moving;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnMouseDown()
    {
        if (isEnabled)
        {
            if (!rotating)
            {
                StartCoroutine(RotateDown());
            }
            if (!moving)
            {
                StartCoroutine(MoveToLocation());
            }
        }
        isEnabled = false;
    }


    IEnumerator MoveToLocation() 
    { 
        moving = true;
        float timeElapsed = 0;

        while (timeElapsed < moveDuration) 
        { 
            currentCam.transform.position = Vector3.Lerp(currentCam.transform.position, endMarker.position, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            Debug.Log("0");
            yield return null;
        }
        currentCam.transform.position = endMarker.position;
        moving = false;
    }
    IEnumerator RotateDown()
    {
        rotating = true;
        float timeElapsed = 0;

        Quaternion startRotation = currentCam.transform.rotation;
        Quaternion targetRotation = endMarker.transform.rotation;
        while (timeElapsed < rotaionDuration)
        {
            currentCam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / rotaionDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        currentCam.transform.rotation = targetRotation;
        rotating = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
