using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClick : MonoBehaviour
{
    public Transform startMarker;
    public Transform endMarker;
    public Transform lookAt;

    public float speed = 1f;

    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 center = (startMarker.position + endMarker.position) * 0.5f;

        center -= new Vector3(0, 1, 0);

        Vector3 startRelCenter = startMarker.position - center;
        Vector3 endRelCenter = endMarker.position - center;

        float fracComplete = (Time.time - startTime) / speed;

        transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete);
        transform.position += center;

        

        transform.LookAt(new Vector3(endMarker.position.x, lookAt.position.y, endMarker.position.z - 1f));
    }
}
