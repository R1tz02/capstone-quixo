using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilClick : MonoBehaviour
{
    public Transform startMarker;
    public Transform endMarker;
    public Transform lookAt;
    public Camera currentCam;

    public float speed = 1f;
    public float lerpDuration = 1f;

    private bool hasBeenClicked = false;
    private float startTime;
    private float journeyLength;
    
    bool rotating;

    // Start is called before the first frame update
    void Start()
    {
        

        journeyLength = Vector3.Distance(currentCam.transform.position, endMarker.position);
    }

    void OnMouseDown()
    {
        startTime = Time.time;
        hasBeenClicked = true;
        Debug.Log("Test");
    }

    IEnumerator Rotate90()
    {
        rotating = true;
        float timeElapsed = 0;
        Quaternion startRotation = currentCam.transform.rotation;
        Quaternion targetRotation = endMarker.transform.rotation;
        while (timeElapsed < lerpDuration)
        {
            currentCam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        currentCam.transform.rotation = targetRotation;
        rotating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasBeenClicked)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            currentCam.transform.position = Vector3.Lerp(currentCam.transform.position, endMarker.position, fractionOfJourney);
            if (!rotating)
            {
                StartCoroutine(Rotate90());
            }
        }
    }
}
