using System.Collections;
using UnityEngine;

public class AnvilClick : MonoBehaviour
{
    public Transform endMarker;
    public Camera currentCam;
    public Canvas currentCanvas;
    public Material highlightMaterial;

    GameObject menuItem;

    public float moveDuration = 1f;
    public float rotaionDuration = 1f;

    private bool hasBeenClicked = false;
    
    bool rotating;
    bool moving;

    // Start is called before the first frame update
    void Start()
    {
        
        menuItem = GameObject.FindGameObjectWithTag("MenuItem");
    }

    //void OnMouseOver()
    //{
    //    // Change the material to the highlight material when the mouse is over the object
    //    if (!hasBeenClicked)
    //    {
    //        Renderer rend = menuItem.GetComponent<Renderer>();
    //        if (rend != null && highlightMaterial != null)
    //        {
    //            rend.material = highlightMaterial;
    //        }
    //    }
    //}

    void OnMouseDown()
    {
        if (currentCam.transform.position != endMarker.position && !GameObject.Find("Game Manager").GetComponent<MenuController>().isError)
        {
            GameObject.Find("Game Manager").GetComponent<MenuController>().labelCanvas.enabled = false;
            GameObject.Find("Game Manager").GetComponent<MenuController>().overlayCanvas.enabled = false;
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

    public void buttonMover()
    {
        if (currentCam.transform.position != endMarker.position && !GameObject.Find("Game Manager").GetComponent<MenuController>().isError)
        {
            GameObject.Find("Game Manager").GetComponent<MenuController>().labelCanvas.enabled = false;
            GameObject.Find("Game Manager").GetComponent<MenuController>().overlayCanvas.enabled = false;
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

        while (timeElapsed < 1) 
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
