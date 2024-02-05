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

    private bool hasBeenClicked = false;
    private float startTime;
    private float journeyLength;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;

        journeyLength = Vector3.Distance(currentCam.transform.position, endMarker.position);
    }

    void OnMouseDown()
    {
        hasBeenClicked = true;
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

            currentCam.transform.LookAt(lookAt.position, Vector3.back);
        }
    }
}
