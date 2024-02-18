using System.Collections;
using UnityEngine;

public class AnvilClick : MonoBehaviour
{
    public Transform endMarker;
    public Camera currentCam;
    public Canvas currentCanvas;

    public float moveDuration = 1f;
    public float rotaionDuration = 1f;

    private bool hasBeenClicked = false;
    
    bool rotating;
    bool moving;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnMouseDown()
    {
        if (currentCam.transform.position != endMarker.position)
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
        currentCanvas.enabled = true;
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
